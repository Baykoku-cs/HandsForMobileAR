using Assembly_CSharp;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    internal class LandmarkInterpreter : MonoBehaviour, ILandmarkInterpreter
    {
        [SerializeField] private HandTrackingProvider _handTrackingProvider;
        [SerializeField] private HandVisualizer _handVisualizer;
        private JitterFilter _filter;

        public IDepthModifier DepthModifier;
        public Vector3[] LastProcessedLandmarks { get; private set; }
        public Vector3[] LastRawLandmarks { get; private set; }
        public Vector3 ForwardVector { get; private set; }

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

            _handVisualizer.SendNewLandMarks(filteredData);
        }
    }
}
