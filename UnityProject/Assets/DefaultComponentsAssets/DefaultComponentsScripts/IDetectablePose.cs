using System;
using UnityEngine;

internal interface IDetectablePose
{
    public event EventHandler OnPoseDetectionStart;
    public event EventHandler OnPoseDetected;
    public event EventHandler OnPoseCanceled;
    public event EventHandler OnPoseLost;
    public bool Check(Vector3[] landmarks, string name);
    public void Tick(float dt);
    public void Pause();
    public void Resume();
    public float GetActivationTimerNormalized();
}

