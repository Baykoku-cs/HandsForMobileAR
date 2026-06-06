using Assembly_CSharp;
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

namespace Assets.Scripts
{
    internal class HandTrackingProvider : MonoBehaviour
    {
        [SerializeField] private ModelMode _modelMode = ModelMode.GestureRecognition;
        [SerializeField] private TextAsset _model;
        private BaseVisionTaskApi _taskApi;

        private const int TARGET_FPS        = 30;
        private const string NONE_POSE_NAME = "None";

        private bool _isReady               = true;
        private bool _areNewLandmarksReady  = false;
        private Stopwatch _stopwatch        = new Stopwatch();

        private (List<Vector3> landmarkVectors, string gestureName, long timestampMillisec) _lastModelResult = (new List<Vector3>(), NONE_POSE_NAME, 0);

        public ILandmarkInterpreter LandmarkInterpreter;
        public IGestureInterpreter GestureInterpreter;
        public IImageProvider ImageProvider;

        public enum ModelMode
        {
            LandmarksOnly,
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
                LandmarkInterpreter.OnNewLandmarksGenerated(_lastModelResult.landmarkVectors, ImageProvider);
                GestureInterpreter.OnNewGestureGenerated(_lastModelResult.landmarkVectors, _lastModelResult.gestureName);
                _areNewLandmarksReady = false;
            }

            if (_isReady)
            {
                GenerateLandmarks();
                new Task(() => { Thread.Sleep(1000 / TARGET_FPS); _isReady = true; }).Start();
            }
        }
        private void ProcessGesturesCallback(GestureRecognizerResult gestureRecognizerResult, Mediapipe.Image image, long timestampMillisec) => 
            ProcessTaskApiResult(gestureRecognizerResult.handLandmarks, gestureRecognizerResult.gestures, image, timestampMillisec);
        private void ProcessLandmarksOnlyCallback(HandLandmarkerResult handRecognizerResult, Mediapipe.Image image, long timestampMillisec) =>
            ProcessTaskApiResult(handRecognizerResult.handLandmarks, null, image, timestampMillisec);
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
        private void GenerateLandmarks()
        {
            if (ImageProvider.TryGetLastImage(out Mediapipe.Image image))
            {
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
