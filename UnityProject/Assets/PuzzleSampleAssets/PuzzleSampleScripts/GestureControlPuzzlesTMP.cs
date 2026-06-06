using HandsForMobileAR.DefaultComponents;
using UnityEngine;

namespace HandsForMobileAR
{
    namespace PuzzleSampleComponents
    {
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
                _poseDetectBus.SubscribeOnPoseDetected(DefaultComponents.EventType.OnPoseDetected, DefaultPoseNames.Pointing_Up.ToString(), SpawnGameSetup);
            }

            private void OnDestroy()
            {
                _poseDetectBus.UnSubscribeOnPoseDetected(DefaultComponents.EventType.OnPoseDetected, DefaultPoseNames.Pointing_Up.ToString(), SpawnGameSetup);
            }

            private void SpawnGameSetup()
            {
                gameSetup.transform.position = _landmarkInterpreter.LastProcessedLandmarks[7];

                gameSetup.SetActive(true);
            }
        }
    }
}