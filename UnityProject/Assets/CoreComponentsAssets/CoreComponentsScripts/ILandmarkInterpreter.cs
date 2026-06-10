using System.Collections.Generic;
using UnityEngine;

namespace HandsForMobileAR
{
    namespace CoreComponents
    {
        public interface ILandmarkInterpreter
        {
            /// <summary>
            /// This function will by invoked by HandTrackingProvider after new landmark is recived from mediapipe
            /// </summary>
            public void OnNewLandmarksGenerated(List<Vector3> newLandmarks, IImageProvider imageProvider);
        }
    }
}
