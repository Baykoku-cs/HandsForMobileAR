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
    private Color CollidedColor;

    [SerializeField]
    private Color CalmColor;

    private void Start()
    {
        _poseDetectBus.SubscribeOnPoseDetected(EventType.OnPoseDetected, PoseType.Closed_Fist, OnFistDetected);
        _poseDetectBus.SubscribeOnPoseDetected(EventType.OnPoseLost, PoseType.Closed_Fist, OnFistLost);
    }

    private void OnDestroy()
    {
        _poseDetectBus.UnSubscribeOnPoseDetected(EventType.OnPoseDetected, PoseType.Closed_Fist, OnFistDetected);
        _poseDetectBus.UnSubscribeOnPoseDetected(EventType.OnPoseLost, PoseType.Closed_Fist, OnFistLost);
    }

    private void OnFistDetected()
    {
        _triggedSnapPoint?.Snap(transform);
    }
    private void OnFistLost()
    {
        _triggedSnapPoint?.UnSnap();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (_triggedSnapPoint != null)
            return;

        if (other.TryGetComponent<SnapPoint>(out _triggedSnapPoint))
        {
            _grabberVisual.material.color = CollidedColor;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<SnapPoint>(out SnapPoint point) && point == _triggedSnapPoint)
        {
            _triggedSnapPoint = null;
            _grabberVisual.material.color = CalmColor;
        }
    }
}
