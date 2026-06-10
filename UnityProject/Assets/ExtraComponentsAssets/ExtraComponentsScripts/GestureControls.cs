using UnityEngine;
using HandsForMobileAR.DefaultComponents;

namespace HandsForMobileAR
{
    namespace ExtraComponents
    {
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
                _poseDetectBus.SubscribeOnPoseDetected(DefaultComponents.EventType.OnPoseDetected, DefaultPoseNames.Victory.ToString(), OnVictoryDetected);
                _poseDetectBus.SubscribeOnPoseDetected(DefaultComponents.EventType.OnPoseDetected, DefaultPoseNames.ILoveYou.ToString(), OnLoveYouDetected);
            }

            private void OnDestroy()
            {
                _poseDetectBus.UnSubscribeOnPoseDetected(DefaultComponents.EventType.OnPoseDetected, DefaultPoseNames.Victory.ToString(), OnVictoryDetected);
                _poseDetectBus.UnSubscribeOnPoseDetected(DefaultComponents.EventType.OnPoseDetected, DefaultPoseNames.ILoveYou.ToString(), OnLoveYouDetected);
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
    }
}