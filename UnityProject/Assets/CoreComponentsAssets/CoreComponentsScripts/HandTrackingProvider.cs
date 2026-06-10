using Mediapipe.Tasks.Components.Containers;
using Mediapipe.Tasks.Core;
using Mediapipe.Tasks.Vision.Core;
using Mediapipe.Tasks.Vision.GestureRecognizer;
using Mediapipe.Tasks.Vision.HandLandmarker;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace HandsForMobileAR
{
    namespace CoreComponents
    {
        /// <summary>
        /// Core class that handles all Mediapipe related logic
        /// </summary>
        public class HandTrackingProvider : MonoBehaviour
        {
            /// <summary>
            /// Select type of running model. 
            /// </summary>
            /// <remarks>
            /// You should provide proper text asset
            /// </remarks>
            [SerializeField] private ModelMode _modelMode = ModelMode.GestureRecognition;

            /// <summary>
            /// Text asset with model (.byte file)  
            /// </summary>
            [SerializeField] private TextAsset _model;
            
            /// <summary>
            /// Reference to running mediapipe task
            /// </summary>
            private BaseVisionTaskApi _taskApi;

            /// <summary>
            /// Fps lock for mediapipe calls
            /// </summary>
            private const int TARGET_FPS = 30;

            /// <summary>
            /// Default name for no gestures state.
            /// Mediapipe sends this gestureName, when detected hand doesn't look like any known gesture.
            /// </summary>
            private const string NONE_POSE_NAME = "None";

            /// <summary>
            /// Indicator that shows when you can send data to mediapipe (fps lock)
            /// </summary>
            private bool _isReady = true;

            /// <summary>
            /// Indicator that shows when data from mediapipe where recived
            /// </summary>
            /// <remarks>
            /// We are using async methods to send data into mediapipe. But we can't use callback as place where we can send data into other classes, because callback executes into subthread where we have a lot restrictions 
            /// (for example, we can't update any UI).
            /// </remarks>
            private bool _areNewLandmarksReady = false;

            /// <summary>
            /// Timestamps source for mediapipe
            /// </summary>
            private Stopwatch _stopwatch = new Stopwatch();

            /// <summary>
            /// Refrenece to last model result.
            /// </summary>
            /// <param name="landmarkVectors">List of normalized landmarks</param>
            /// <param name="gestureName">String gesture identificator</param>
            /// <param name="timestampMillisec">Mediapipe pocket timestamp</param>
            private (List<Vector3> landmarkVectors, string gestureName, long timestampMillisec) _lastModelResult = (new List<Vector3>(), NONE_POSE_NAME, 0);

            /// <summary>
            /// Property for implementation of ILandmarkIntepreter 
            /// </summary>
            public ILandmarkInterpreter LandmarkInterpreter;
            /// <summary>
            /// Property for implementation of IGestureInterpreter
            /// </summary>
            public IGestureInterpreter GestureInterpreter;
            /// <summary>
            /// Property for implementation of IImageProvider
            /// </summary>
            public IImageProvider ImageProvider;

            /// <summary>
            /// Mediapipe model options
            /// </summary>
            public enum ModelMode
            {
                /// <summary>
                /// There will be only Hand Landmark model running. So there will be no gesture-related date. 
                /// You can use this to improve application performance in case you don't need gestures.
                /// </summary>
                LandmarksOnly,

                /// <summary>
                /// This mode uses Gesture Recognition model that is enhanced version of Hand Landmark model. 
                /// Adds embeding and classification models into computation pipeline.
                /// </summary>
                GestureRecognition
            }
            private void Awake()
            {
                _taskApi = _modelMode switch
                {
                    ModelMode.LandmarksOnly => HandLandmarker.CreateFromOptions(new HandLandmarkerOptions(
                    new BaseOptions(delegateCase: BaseOptions.Delegate.CPU,
                                    modelAssetBuffer: _model.bytes),
                    RunningMode.LIVE_STREAM,
                    resultCallback: ProcessLandmarksOnlyCallback)),

                    ModelMode.GestureRecognition => GestureRecognizer.CreateFromOptions(new GestureRecognizerOptions(
                    new BaseOptions(delegateCase: BaseOptions.Delegate.CPU,
                                    modelAssetBuffer: _model.bytes),
                    RunningMode.LIVE_STREAM,
                    resultCallback: ProcessGesturesCallback))
                };

                _stopwatch.Start();
            }
            private void Update()
            {
                if (_areNewLandmarksReady)
                {
                    LandmarkInterpreter?.OnNewLandmarksGenerated(_lastModelResult.landmarkVectors, ImageProvider);
                    GestureInterpreter?.OnNewGestureGenerated(_lastModelResult.landmarkVectors, _lastModelResult.gestureName);
                    _areNewLandmarksReady = false;
                }

                if (_isReady)
                {
                    GenerateLandmarks();
                    new Task(() => { Thread.Sleep(1000 / TARGET_FPS); _isReady = true; }).Start();
                }
            }

            /// <summary>
            /// Callback from gesture recognition model
            /// </summary>
            /// <remarks>
            /// This method will invoke ProcessTaskApiResult
            /// </remarks>
            private void ProcessGesturesCallback(GestureRecognizerResult gestureRecognizerResult, Mediapipe.Image image, long timestampMillisec) =>
                ProcessTaskApiResult(gestureRecognizerResult.handLandmarks, gestureRecognizerResult.gestures, image, timestampMillisec);

            /// <summary>
            /// Callback from hand landmark model
            /// </summary>
            /// <remarks>
            /// This method will invoke ProcessTaskApiResult
            /// </remarks>
            private void ProcessLandmarksOnlyCallback(HandLandmarkerResult handRecognizerResult, Mediapipe.Image image, long timestampMillisec) =>
                ProcessTaskApiResult(handRecognizerResult.handLandmarks, null, image, timestampMillisec);

            /// <summary>
            /// Method that handles callbacks from Mediapipe.
            /// </summary>
            /// <remarks>
            /// Keep in mind that this will be invoked in a sub-thread
            /// </remarks>
            /// <param name="landmarks">List of predicted landmarks for all detected hands</param>
            /// <param name="gestures">Gesture information for all detected hands (there is string id of gesture and model confidence in result)</param>
            /// <param name="image">Image that were processed. This needs to be disposed to prevent memorry leaks</param>
            /// <param name="timestampMillisec">Pocket timestamp from Mediapipe</param>
            private void ProcessTaskApiResult(List<NormalizedLandmarks> landmarks, List<Classifications> gestures, Mediapipe.Image image, long timestampMillisec)
            {
                if (timestampMillisec < _lastModelResult.timestampMillisec)
                    return;

                if (_modelMode == ModelMode.GestureRecognition)
                {
                    if (landmarks is null)
                    {
                        _lastModelResult.gestureName = NONE_POSE_NAME;
                    }
                    else if (!_lastModelResult.gestureName.Equals(gestures[0].categories[0].categoryName))
                    {
                        _lastModelResult.gestureName = gestures[0].categories[0].categoryName;
                    }
                }

                if (landmarks is not null)
                {
                    _lastModelResult.landmarkVectors.Clear();

                    for (int i = 0; i < landmarks[0].landmarks.Count; i++)
                    {
                        Vector3 screenPoint = new Vector3(landmarks[0].landmarks[i].x,
                                                          1 - landmarks[0].landmarks[i].y,
                                                          landmarks[0].landmarks[i].z);

                        _lastModelResult.landmarkVectors.Add(screenPoint);
                    }

                }

                if (_lastModelResult.landmarkVectors.Count > 0)
                    // Нужно для случая, когда на самом первом доступном кадре нет изображения руки. 
                    // Стоит как-то додумать систему, вырезав landmarks из gestures.
                    _areNewLandmarksReady = true;

                image.Dispose();
            }

            /// <summary>
            /// Method that sends data to mediapipe
            /// </summary>
            private void GenerateLandmarks()
            {
                if (ImageProvider.TryGetLastImage(out Texture2D texture))
                {
                    var image = new Mediapipe.Image(texture);
                    if (_taskApi is HandLandmarker)
                    {
                        (_taskApi as HandLandmarker).DetectAsync(image, _stopwatch.ElapsedMilliseconds);
                    }
                    else if (_taskApi is GestureRecognizer)
                    {
                        (_taskApi as GestureRecognizer).RecognizeAsync(image, _stopwatch.ElapsedMilliseconds);
                    }
                }
            }
        }
    }
}