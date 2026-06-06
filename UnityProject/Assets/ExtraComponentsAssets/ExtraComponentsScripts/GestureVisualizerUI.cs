using UnityEngine;
using UnityEngine.UI;

public class GestureVisualizerUI : MonoBehaviour
{
    [SerializeField] Sprite iconSprite;
    [SerializeField] private PoseType _poseType;
    [SerializeField] private PoseDetectBus _poseDetectBus;

    [SerializeField] private Color YELLOW_COLOR;
    [SerializeField] private Color RED_COLOR;
    [SerializeField] private Color GREEN_COLOR;

    [SerializeField] Image fillImage;
    [SerializeField] Image iconImage;

    private void Start()
    {
        iconImage.sprite = iconSprite;
        _poseDetectBus.SubscribeOnPoseDetected(EventType.OnPoseDetected, _poseType, OnPoseDetected);
        _poseDetectBus.SubscribeOnPoseDetected(EventType.OnPoseDetectionStart, _poseType, OnPoseDetectionStart);
    }
    private void OnDestroy()
    {
        _poseDetectBus.UnSubscribeOnPoseDetected(EventType.OnPoseDetected, _poseType, OnPoseDetected);
        _poseDetectBus.UnSubscribeOnPoseDetected(EventType.OnPoseDetectionStart, _poseType, OnPoseDetectionStart);
    }
    private void Update()
    {
        fillImage.fillAmount = _poseDetectBus.GetPoseDetectTimerNormalized(_poseType);
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
