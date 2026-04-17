using UnityEngine;

public class GestureControls : MonoBehaviour
{
    [SerializeField]
    private VrMode _vrMode;

    [SerializeField]
    private HandProvider _handProvider;

    [SerializeField]
    private PoseDetectBus _poseDetectBus;
    
    private void Start()
    {
        _poseDetectBus.SubscribeOnPoseDetected(EventType.OnPoseDetected, PoseType.Victory, OnVictoryDetected);
        _poseDetectBus.SubscribeOnPoseDetected(EventType.OnPoseDetected, PoseType.ILoveYou, OnLoveYouDetected);
    }

    private void OnDestroy()
    {
        _poseDetectBus.UnSubscribeOnPoseDetected(EventType.OnPoseDetected, PoseType.Victory, OnVictoryDetected);
        _poseDetectBus.UnSubscribeOnPoseDetected(EventType.OnPoseDetected, PoseType.ILoveYou, OnLoveYouDetected);
    }

    private void OnLoveYouDetected()
    {
        _vrMode.ChangeMode();
    }

    private void OnVictoryDetected()
    {
        _handProvider.Calibrate();
    }
}
