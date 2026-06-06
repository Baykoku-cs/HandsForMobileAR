using Assets.Scripts;
using UnityEngine;

public class GestureControls : MonoBehaviour
{
    [SerializeField]
    private VrMode _vrMode;

    [SerializeField]
    private LandmarkInterpreter landmarkInterpreter;

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

    public void OnVictoryDetected()
    {
        (landmarkInterpreter.DepthModifier as CameraDepthModifier).Calibrate(landmarkInterpreter.LastRawLandmarks);
    }
}
