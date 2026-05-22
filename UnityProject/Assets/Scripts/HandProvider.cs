using Assets.Scripts;
using Mediapipe.Tasks.Core;
using Mediapipe.Tasks.Vision.Core;
using Mediapipe.Tasks.Vision.GestureRecognizer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class HandProvider : MonoBehaviour
{
    [SerializeField]
    private TextAsset model;
    [SerializeField]
    private HandVisualizer handVisualizer;
    [SerializeField]
    private CameraImageProvider _imageProvider;

    private const int TARGET_FPS = 30;
    private GestureRecognizer _gestureRecognizer;

    private bool _isReady = true;
    private bool _areNewLandmarksReady;

    private JitterFilter _filter;
    private DepthModifier _depthModifier;

    private (List<Vector3> landmarkVectors, long timestampMillisec) _lastRecognizerResult = (new List<Vector3>(), 0);
    private Vector3[] _lastLandmarksWorldPosition = new Vector3[21];
    private Stopwatch _stopwatch = new Stopwatch();

    private const string NONE_POSE_NAME = "None";
    private string LastDetectedPose = NONE_POSE_NAME;
    private string _poseTmp = NONE_POSE_NAME;
    // To Do: get rid of this. Needs as buffer to remove all event into the main thread

    public event EventHandler<string> OnPoseChanged;

    private void Awake()
    {
        _filter = new JitterFilter(1f);
        _depthModifier = new DepthModifier(0.5f);
    }
    private void Start()
    {
        _gestureRecognizer = GestureRecognizer.CreateFromOptions(new GestureRecognizerOptions(
            new BaseOptions(delegateCase: BaseOptions.Delegate.CPU,
                            modelAssetBuffer: model.bytes), 
            RunningMode.LIVE_STREAM,
            resultCallback: ProcessLandmarksCallback
        ));

        _stopwatch.Start();
    }
    private void Update()
    {
        if (_areNewLandmarksReady)
        {
            Vector3[] filteredData = _filter.Filter(_lastRecognizerResult.landmarkVectors.ToArray());
            Vector3[] finalPoints = _depthModifier.Process(filteredData, _imageProvider.GetScaledCameraResolution());

            _lastLandmarksWorldPosition = finalPoints;

            handVisualizer.SendNewLandMarks(finalPoints);
            // To Do: inverse dependence.

            if (!LastDetectedPose.Equals(_poseTmp))
            {
                LastDetectedPose = _poseTmp;
                OnPoseChanged?.Invoke(this, LastDetectedPose);
            }


            _areNewLandmarksReady = false;
        }
        if (_isReady)
        {
            GenerateLandmarks();
            StartCoroutine(Rest(1f / TARGET_FPS));
        }
    }
    private void ProcessLandmarksCallback(GestureRecognizerResult gestureRecognizerResult, Mediapipe.Image image, long timestampMillisec)
    {
        if (gestureRecognizerResult.handLandmarks is null)
        {
            _poseTmp = NONE_POSE_NAME;
            return;
        }

        _lastRecognizerResult.landmarkVectors.Clear();
        
        for (int i = 0; i < gestureRecognizerResult.handLandmarks[0].landmarks.Count; i++)
        {
            Vector3 screenPoint = new Vector3(gestureRecognizerResult.handLandmarks[0].landmarks[i].x,
                                              1 - gestureRecognizerResult.handLandmarks[0].landmarks[i].y,
                                              gestureRecognizerResult.handLandmarks[0].landmarks[i].z);

            _lastRecognizerResult.landmarkVectors.Add(screenPoint);
        }

        if (!_poseTmp.Equals(gestureRecognizerResult.gestures[0].categories[0].categoryName))
        {
            _poseTmp = gestureRecognizerResult.gestures[0].categories[0].categoryName;
        }

        _areNewLandmarksReady = true;

        image.Dispose();
    }

    private IEnumerator Rest(float restTime)
    {
        _isReady = false;        
        yield return new WaitForSeconds(restTime);
        _isReady = true;
    }

    private void GenerateLandmarks()
    {
        if (_imageProvider.TryGetLastCameraTexture(out Texture2D texture))
        {
            _gestureRecognizer.RecognizeAsync(new Mediapipe.Image(texture), _stopwatch.ElapsedMilliseconds);
        }
    }

    public void Calibrate()
    {
        _depthModifier.Calibrate(_lastRecognizerResult.landmarkVectors.ToArray());
    }

    public Vector3[] GetLastLandmarks()
    {
        return _lastRecognizerResult.landmarkVectors.ToArray();
    }
    public Vector3[] GetLastLandmarksWorldPosition()
    {
        return _lastLandmarksWorldPosition;
    }
}
