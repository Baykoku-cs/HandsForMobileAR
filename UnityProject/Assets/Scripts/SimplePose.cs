using System;
using UnityEngine;
public enum PoseType
{
    None,
    Closed_Fist,
    Open_Palm,
    Pointing_Up,
    Thumb_Down,
    Thumb_Up,
    Victory,
    ILoveYou,
    Pick
}
public enum EventType
{
    OnPoseDetectionStart,
    OnPoseDetected,
    OnPoseCanceled,
    OnPoseLost
}

public class SimplePose : IDetectablePose
{
    public PoseType Type { get; private set; }

    public event EventHandler OnPoseDetectionStart;
    public event EventHandler OnPoseDetected;
    public event EventHandler OnPoseCanceled;
    public event EventHandler OnPoseLost;

    private float _timeToDetectSeconds = 1f;
    private float _timeToExpireSeconds = 0.5f;

    private float _activateTimer;
    private float _expireTimer;

    public bool IsDetecting { get; private set; }
    public bool IsDetected { get; private set; }
    public bool IsPaused { get; private set; }

    public SimplePose(PoseType type)
    {
        Type = type;
    }

    public void Resume()
    {
        IsPaused = false;
        _expireTimer = 0f;

        if (!IsDetected && !IsDetecting)
        {
            IsDetecting = true;
            OnPoseDetectionStart?.Invoke(this, EventArgs.Empty);
        }
    }

    public void Pause()
    {
        IsPaused = true;
    }

    public void Tick(float dt)
    {
        if (!IsDetecting && !IsDetected) return;

        if (IsPaused)
        {
            _expireTimer += dt;
            if (_expireTimer > _timeToExpireSeconds)
            {
                if (IsDetected)
                {
                    OnPoseLost?.Invoke(this, EventArgs.Empty);
                }
                else if (IsDetecting)
                {
                    OnPoseCanceled?.Invoke(this, EventArgs.Empty);
                }
                Reset();
            }
        }
        else
        {
            if (IsDetecting && !IsDetected)
            {
                _activateTimer += dt;
                if (_activateTimer > _timeToDetectSeconds)
                {
                    IsDetected = true;
                    IsDetecting = false;
                    OnPoseDetected?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }

    private void Reset()
    {
        IsDetecting = false;
        IsDetected = false;
        IsPaused = false;
        _activateTimer = 0f;
        _expireTimer = 0f;
    }

    public float GetActivationTimerNormalized()
    {
        return _activateTimer / _timeToDetectSeconds;
    }

    public bool Check(Vector3[] landmarks, string name)
    {
        if (Enum.TryParse(name, out PoseType poseType))
        {
            return poseType == Type;
        }
        else
        {
            return false;
        }
    }
}