using System.Collections.Generic;
using UnityEngine;
using HandsForMobileAR.CoreComponents;

namespace HandsForMobileAR
{
    namespace DefaultComponents
    {
        internal class LandmarkInterpreter : MonoBehaviour, ILandmarkInterpreter
        {
            [SerializeField] private HandTrackingProvider _handTrackingProvider;
            private JitterFilter _filter;

            public IDepthModifier DepthModifier;
            public Vector3[] LastProcessedLandmarks { get; private set; }
            public Vector3[] LastRawLandmarks { get; private set; }
            public Vector3 PalmNormal { get; private set; }
            public Vector3 PalmCenter { get; private set; }

            private void Awake()
            {
                _filter = new JitterFilter(1f);
            }
            private void Start()
            {
                _handTrackingProvider.LandmarkInterpreter = this;
            }

            public void OnNewLandmarksGenerated(List<Vector3> newLandmarks, IImageProvider imageProvider)
            {
                LastRawLandmarks = newLandmarks.ToArray();

                Vector3[] filteredData = _filter.Filter(newLandmarks.ToArray());
                DepthModifier.Process(filteredData, imageProvider);

                LastProcessedLandmarks = filteredData;
                RefreshPalmNormalVector();
            }

            public void RefreshPalmNormalVector()
            {
                var hr = LastProcessedLandmarks[17] - LastProcessedLandmarks[5];
                var vr = LastProcessedLandmarks[9] - LastProcessedLandmarks[0];

                PalmNormal = Vector3.Cross(hr, vr).normalized;
                PalmCenter = (LastProcessedLandmarks[0] + LastProcessedLandmarks[9]) * 0.5f;
            }
        }
    }
}