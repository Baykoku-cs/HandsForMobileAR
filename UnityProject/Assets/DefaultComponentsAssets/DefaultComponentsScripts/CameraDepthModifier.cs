using UnityEngine;
using HandsForMobileAR.CoreComponents;

namespace HandsForMobileAR
{
    namespace DefaultComponents
    {
        /// <summary>
        /// This class goal is to convert normalized landmarks into world-position.
        /// It hardly depend on CameraImageProvider
        /// </summary>
        public class CameraDepthModifier : MonoBehaviour, IDepthModifier
        {
            [SerializeField] private LandmarkInterpreter _landmarkInterpreter;

            private float _calibratedHandSize;
            private float _zMultiplier = 1f;

            private void Start()
            {
                _landmarkInterpreter.DepthModifier = this;
            }

            public void Calibrate(Vector3[] landmarks)
            {
                _calibratedHandSize = CalculateHandSize(landmarks);
            }
            public void Process(Vector3[] landmarks, IImageProvider imageProvider)
            {
                if (imageProvider is not CameraImageProvider)
                    throw new System.Exception("This component requires CameraImageProvider");

                var resolution = (imageProvider as CameraImageProvider).GetScaledCameraResolution();

                Vector3[] worldPoints = new Vector3[landmarks.Length];

                float currentSize = CalculateHandSize(landmarks);
                float baseDistance = (_calibratedHandSize / currentSize) * 0.75f;

                for (int i = 0; i < landmarks.Length; i++)
                {
                    Vector3 screenPoint = new Vector3(
                        landmarks[i].x * resolution.x,
                        landmarks[i].y * resolution.y - (resolution.y - Screen.height) * 0.5f,
                        baseDistance + (landmarks[i].z * _zMultiplier)
                    );

                    landmarks[i] = Camera.main.ScreenToWorldPoint(screenPoint);
                }
            }
            private float CalculateHandSize(Vector3[] landmarks)
            {
                float size = 0;
                size += Vector3.Distance(landmarks[0], landmarks[5]);
                size += Vector3.Distance(landmarks[5], landmarks[6]);
                size += Vector3.Distance(landmarks[6], landmarks[7]);
                size += Vector3.Distance(landmarks[7], landmarks[8]);
                return size;
            }
        }
    }
}
