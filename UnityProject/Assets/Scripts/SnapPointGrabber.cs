using TMPro;
using UnityEngine;

public class SnapPointGrabber : MonoBehaviour
{
    [SerializeField]
    private PoseDetectBus _poseDetectBus;

    private SnapPoint _triggedSnapPoint;

    [SerializeField]
    private MeshRenderer _grabberVisual;

    [SerializeField]
    private Color _collidedColor;

    [SerializeField]
    private Color _calmColor;

    private bool _isHolding;

    private void Start()
    {
        _poseDetectBus.SubscribeOnPoseDetected(EventType.OnPoseDetected, PoseType.Pick, OnFistDetected);
        _poseDetectBus.SubscribeOnPoseDetected(EventType.OnPoseLost, PoseType.Pick, OnFistLost);
    }

    private void OnDestroy()
    {
        _poseDetectBus.UnSubscribeOnPoseDetected(EventType.OnPoseDetected, PoseType.Pick, OnFistDetected);
        _poseDetectBus.UnSubscribeOnPoseDetected(EventType.OnPoseLost, PoseType.Pick, OnFistLost);
    }

    private void OnFistDetected()
    {
        if (_triggedSnapPoint is not null)
        {
            _triggedSnapPoint?.Snap(transform);
            _isHolding = true;
        }
    }
    private void OnFistLost()
    {
        if (_triggedSnapPoint is not null && _isHolding)
        {
            _triggedSnapPoint?.UnSnap();
            _triggedSnapPoint = null;
            _grabberVisual.material.color = _calmColor;
            _isHolding = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_triggedSnapPoint != null)
            return;

        if (other.TryGetComponent<SnapPoint>(out _triggedSnapPoint))
        {
            _grabberVisual.material.color = _collidedColor;
            _triggedSnapPoint.Show();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_isHolding)
        {
            if (other.TryGetComponent<SnapPoint>(out SnapPoint point) && point == _triggedSnapPoint)
            {
                _triggedSnapPoint.Hide();
                _triggedSnapPoint = null;
                _grabberVisual.material.color = _calmColor;
            }
        }
    }
}
