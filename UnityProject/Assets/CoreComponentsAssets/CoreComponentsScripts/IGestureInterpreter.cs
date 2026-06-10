using System.Collections.Generic;
using UnityEngine;

namespace HandsForMobileAR
{
    namespace CoreComponents
    {
        public interface IGestureInterpreter
        {
            /// <summary>
            /// This function will by invoked by HandTrackingProvider after new landmark and gesturename recived from mediapipe
            /// </summary>
            public void OnNewGestureGenerated(List<Vector3> newLandmarks, string detectedGestureName);
        }
    }
}
