using UnityEngine;
using UnityEngine.UI;
using HandsForMobileAR.DefaultComponents;

namespace HandsForMobileAR
{
    namespace ExtraComponents
    {
        public class GestureVisualizerUI : MonoBehaviour
        {
            [SerializeField] Sprite _iconSprite;
            [SerializeField] private string _poseName;
            [SerializeField] private PoseDetectBus _poseDetectBus;

            [SerializeField] private Color YELLOW_COLOR;
            [SerializeField] private Color RED_COLOR;
            [SerializeField] private Color GREEN_COLOR;

            [SerializeField] Image fillImage;
            [SerializeField] Image iconImage;

            private void Start()
            {
                iconImage.sprite = _iconSprite;
                _poseDetectBus.SubscribeOnPoseDetected(DefaultComponents.EventType.OnPoseDetected, _poseName, OnPoseDetected);
                _poseDetectBus.SubscribeOnPoseDetected(DefaultComponents.EventType.OnPoseDetectionStart, _poseName, OnPoseDetectionStart);
            }
            private void OnDestroy()
            {
                _poseDetectBus.UnSubscribeOnPoseDetected(DefaultComponents.EventType.OnPoseDetected, _poseName, OnPoseDetected);
                _poseDetectBus.UnSubscribeOnPoseDetected(DefaultComponents.EventType.OnPoseDetectionStart, _poseName, OnPoseDetectionStart);
            }
            private void Update()
            {
                fillImage.fillAmount = _poseDetectBus.GetPoseDetectTimerNormalized(_poseName);
            }

            private void OnPoseDetected()
            {
                fillImage.color = GREEN_COLOR;
            }
            private void OnPoseDetectionStart()
            {
                fillImage.color = YELLOW_COLOR;
            }
        }
    }
}
