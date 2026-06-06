using UnityEngine;
using HandsForMobileAR.DefaultComponents;

namespace HandsForMobileAR
{
    namespace PuzzleSampleComponents
    {
        public class SnapPointGrabber : MonoBehaviour
        {
            [SerializeField] private PoseDetectBus _poseDetectBus;
            [SerializeField] private MeshRenderer _grabberVisual;
            [SerializeField] private Color _collidedColor;
            [SerializeField] private Color _calmColor;

            private SnapPoint _triggedSnapPoint;
            
            private bool _isHolding;

            private void Awake()
            {
                _poseDetectBus.RegisterPose("Pick", new PickPose());
            }
            private void Start()
            {
                _poseDetectBus.SubscribeOnPoseDetected(DefaultComponents.EventType.OnPoseDetected, "Pick", OnFistDetected);
                _poseDetectBus.SubscribeOnPoseDetected(DefaultComponents.EventType.OnPoseLost, "Pick", OnFistLost);
            }

            private void OnDestroy()
            {
                _poseDetectBus.UnSubscribeOnPoseDetected(DefaultComponents.EventType.OnPoseDetected, "Pick", OnFistDetected);
                _poseDetectBus.UnSubscribeOnPoseDetected(DefaultComponents.EventType.OnPoseLost, "Pick", OnFistLost);
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

                if (other.TryGetComponent(out _triggedSnapPoint))
                {
                    _grabberVisual.material.color = _collidedColor;
                    _triggedSnapPoint.Show();
                }
            }

            private void OnTriggerExit(Collider other)
            {
                if (!_isHolding)
                {
                    if (other.TryGetComponent(out SnapPoint point) && point == _triggedSnapPoint)
                    {
                        _triggedSnapPoint.Hide();
                        _triggedSnapPoint = null;
                        _grabberVisual.material.color = _calmColor;
                    }
                }
            }
        }
    }
}