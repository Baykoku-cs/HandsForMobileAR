using Assets.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PoseDetectBus : MonoBehaviour, IGestureInterpreter
{
    [SerializeField] private HandTrackingProvider _handTrackingProvider;

    private PoseState[] _poses;
    private PoseType _lastPoseType = PoseType.None;

    private void Awake()
    {
        int poseCount = Enum.GetNames(typeof(PoseType)).Length;
        _poses = new PoseState[poseCount];

        for (int i = 0; i < poseCount; i++)
        {
            _poses[i] = new PoseState((PoseType)i);
        }
    }
    private void Start()
    {
        _handTrackingProvider.GestureInterpreter = this;
    }
    private void Update()
    {
        float dt = Time.deltaTime;
        for (int i = 0; i < _poses.Length; i++)
        {
            _poses[i].Tick(dt);
        }
    }

    public void SubscribeOnPoseDetected(EventType eventType, PoseType poseType, UnityAction action)
    {
        switch (eventType)
        {
            case EventType.OnPoseDetectionStart:
                _poses[(int)poseType].OnPoseDetectionStart += (s, e) => action();
                break;

            case EventType.OnPoseLost:
                _poses[(int)poseType].OnPoseLost += (s, e) => action();
                break;

            case EventType.OnPoseDetected:
                {
                    _poses[(int)poseType].OnPoseDetected += (s, e) => action();
                    break;
                }

            case EventType.OnPoseCanceled:
                _poses[(int)poseType].OnPoseCanceled += (s, e) => action();
                break;
        }
    }

    public void UnSubscribeOnPoseDetected(EventType eventType, PoseType poseType, Action action)
    {
        switch (eventType)
        {
            case EventType.OnPoseDetectionStart:
                _poses[(int)poseType].OnPoseDetectionStart -= (s, e) => action();
                break;

            case EventType.OnPoseLost:
                _poses[(int)poseType].OnPoseLost -= (s, e) => action();
                break;

            case EventType.OnPoseDetected:
                _poses[(int)poseType].OnPoseDetected -= (s, e) => action();
                break;

            case EventType.OnPoseCanceled:
                _poses[(int)poseType].OnPoseCanceled -= (s, e) => action();
                break;
        }
    }
    public float GetPoseDetectTimerNormalized(PoseType poseType)
    {
        return _poses[(int)poseType].GetActivationTimerNormalized();
    }

    public void OnNewGestureGenerated(List<Vector3> newLandmarks, string detectedGestureName)
    {
        if (Enum.TryParse(detectedGestureName, out PoseType newPoseType))
        {
            if (_lastPoseType != newPoseType)
            {
                _poses[(int)_lastPoseType].Pause();
                _poses[(int)newPoseType].Resume();

                _lastPoseType = newPoseType;
            }
        }
        else
        {
            Debug.LogWarning($"Unknown pose: {detectedGestureName}");
        }
    }
}