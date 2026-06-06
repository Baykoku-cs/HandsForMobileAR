using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PoseDetectBus : MonoBehaviour, IGestureInterpreter
{
    [SerializeField] private HandTrackingProvider _handTrackingProvider;

    private IDetectablePose[] _poses;
    private IDetectablePose _lastPose;

    private void Awake()
    {
        int poseCount = Enum.GetNames(typeof(PoseType)).Length;

        List<IDetectablePose> posesList = new List<IDetectablePose>();

        var pickPose = new PickPose();

        for (int i = 0; i < poseCount; i++)
        {
            if ((PoseType)i == PoseType.Pick)
                posesList.Add(new PickPose());
            else
                posesList.Add(new SimplePose((PoseType)i));
        }
        _poses = posesList.ToArray();
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
        foreach (var pose in _poses.Reverse())
        {
            if (pose.Check(newLandmarks.ToArray(), detectedGestureName))
            {
                if (_lastPose is not null)
                {
                    _lastPose.Pause();
                }
                pose.Resume();
                _lastPose = pose;
                return;
            }
        }
    }
}