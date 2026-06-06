using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using HandsForMobileAR.CoreComponents;

namespace HandsForMobileAR
{
    namespace DefaultComponents
    {
        public enum DefaultPoseNames
        {
            None,
            Closed_Fist,
            Open_Palm,
            Pointing_Up,
            Thumb_Down,
            Thumb_Up,
            Victory,
            ILoveYou
        }

        public enum EventType
        {
            OnPoseDetectionStart,
            OnPoseDetected,
            OnPoseCanceled,
            OnPoseLost
        }
        public class PoseDetectBus : MonoBehaviour, IGestureInterpreter
        {
            [SerializeField] private HandTrackingProvider _handTrackingProvider;

            private Dictionary<string, IDetectablePose> _poses = new Dictionary<string, IDetectablePose>();
            private IDetectablePose _lastPose;

            private void Awake()
            {
                RegisterPose("Closed_Fist", new SimplePose("Closed_Fist"));
                RegisterPose("Open_Palm",   new SimplePose("Open_Palm"));
                RegisterPose("Pointing_Up", new SimplePose("Pointing_Up"));
                RegisterPose("Thumb_Down",  new SimplePose("Thumb_Down"));
                RegisterPose("Thumb_Up",    new SimplePose("Thumb_Up"));
                RegisterPose("Victory",     new SimplePose("Victory"));
                RegisterPose("ILoveYou",    new SimplePose("ILoveYou"));
            }
            private void Start()
            {
                _handTrackingProvider.GestureInterpreter = this;
            }
            private void Update()
            {
                float dt = Time.deltaTime;
                foreach (var pose in _poses.Values)
                {
                    pose.Tick(dt);
                }
            }

            public void RegisterPose(string poseName, IDetectablePose pose)
            {
                _poses[poseName] = pose;
            }
            public void UnRegisterPose(string poseName)
            {
                _poses.Remove(poseName);
            }

            public void SubscribeOnPoseDetected(EventType eventType, string poseName, UnityAction action)
            {
                switch (eventType)
                {
                    case EventType.OnPoseDetectionStart:
                        _poses[poseName].OnPoseDetectionStart += (s, e) => action();
                        break;

                    case EventType.OnPoseLost:
                        _poses[poseName].OnPoseLost += (s, e) => action();
                        break;

                    case EventType.OnPoseDetected:
                        {
                            _poses[poseName].OnPoseDetected += (s, e) => action();
                            break;
                        }

                    case EventType.OnPoseCanceled:
                        _poses[poseName].OnPoseCanceled += (s, e) => action();
                        break;
                }
            }
            public void UnSubscribeOnPoseDetected(EventType eventType, string poseName, Action action)
            {
                switch (eventType)
                {
                    case EventType.OnPoseDetectionStart:
                        _poses[poseName].OnPoseDetectionStart -= (s, e) => action();
                        break;

                    case EventType.OnPoseLost:
                        _poses[poseName].OnPoseLost -= (s, e) => action();
                        break;

                    case EventType.OnPoseDetected:
                        _poses[poseName].OnPoseDetected -= (s, e) => action();
                        break;

                    case EventType.OnPoseCanceled:
                        _poses[poseName].OnPoseCanceled -= (s, e) => action();
                        break;
                }
            }
            public float GetPoseDetectTimerNormalized(string poseName)
            {
                return _poses[poseName].GetActivationTimerNormalized();
            }

            public void OnNewGestureGenerated(List<Vector3> newLandmarks, string detectedGestureName)
            {
                foreach (var pose in _poses.Values.Reverse())
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
    }   
}