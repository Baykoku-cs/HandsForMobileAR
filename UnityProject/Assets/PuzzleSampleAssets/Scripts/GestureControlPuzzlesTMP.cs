using Assets.Scripts;
using UnityEngine;

public class GestureControlPuzzlesTMP : MonoBehaviour
{
    [SerializeField]
    private GameObject gameSetup;

    [SerializeField]
    private PoseDetectBus _poseDetectBus;
    [SerializeField]
    private LandmarkInterpreter _landmarkInterpreter;

    private void Start()
    {
        _poseDetectBus.SubscribeOnPoseDetected(EventType.OnPoseDetected, PoseType.Pointing_Up, SpawnGameSetup);
    }

    private void OnDestroy()
    {
        _poseDetectBus.UnSubscribeOnPoseDetected(EventType.OnPoseDetected, PoseType.Pointing_Up, SpawnGameSetup);
    }

    private void SpawnGameSetup()
    {
        gameSetup.transform.position = _landmarkInterpreter.LastProcessedLandmarks[7];

        gameSetup.SetActive(true);
    }
}
