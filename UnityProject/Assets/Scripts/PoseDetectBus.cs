using System;
using UnityEngine;

public class PoseDetectBus : MonoBehaviour
{
    [SerializeField] private HandProvider _handProvider;

    private PoseState[] _poses;
    private PoseType _lastPoseType = PoseType.None;

    private void Awake()
    {
        int poseCount = Enum.GetNames(typeof(PoseType)).Length;
        _poses = new PoseState[poseCount];

        for (int i = 0; i < poseCount; i++)
        {
            _poses[i] = new PoseState((PoseType)i);

            _poses[i].OnPoseDetected += (sender, args) => Debug.Log($"DETECTED: {((PoseState)sender).Type}");
            _poses[i].OnPoseLost += (sender, args) => Debug.Log($"LOST: {((PoseState)sender).Type}");
        }
    }

    private void OnEnable()
    {
        if (_handProvider != null)
            _handProvider.OnPoseChanged.AddListener(HandlePoseChange);
    }

    private void OnDisable()
    {
        if (_handProvider != null)
            _handProvider.OnPoseChanged.RemoveListener(HandlePoseChange);
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        for (int i = 0; i < _poses.Length; i++)
        {
            _poses[i].Tick(dt);
        }
    }

    private void HandlePoseChange(string newPoseName)
    {
        if (Enum.TryParse(newPoseName, out PoseType newPoseType))
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
            Debug.LogWarning($"Unknown pose: {newPoseName}");
        }
    }
}

public enum PoseType
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

public class PoseState
{
    public PoseType Type { get; private set; }

    public event EventHandler OnPoseDetectionStart;
    public event EventHandler OnPoseDetected; 
    public event EventHandler OnPoseCanceled; 
    public event EventHandler OnPoseLost;      

    public float TimeToDetectSeconds = 1f;
    public float TimeToExpireSeconds = 0.3f;

    private float _activateTimer;
    private float _expireTimer;

    public bool IsDetecting { get; private set; }
    public bool IsDetected { get; private set; }
    public bool IsPaused { get; private set; }

    public PoseState(PoseType type)
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
            if (_expireTimer > TimeToExpireSeconds)
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
                if (_activateTimer > TimeToDetectSeconds)
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
}