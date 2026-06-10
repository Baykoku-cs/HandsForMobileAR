using System.Collections.Generic;
using UnityEngine;

namespace HandsForMobileAR
{
    namespace CoreComponents
    {
        /// <summary>
        /// Interface for a class that will handle all gesture feed from ML model
        /// </summary>
        public interface IGestureInterpreter
        {
            /// <summary>
            /// This function will by invoked by HandTrackingProvider after new landmark and gesturename recived from mediapipe
            /// </summary>
            public void OnNewGestureGenerated(List<Vector3> newLandmarks, string detectedGestureName);
        }
    }
}
