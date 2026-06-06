using System.Collections.Generic;
using UnityEngine;

namespace HandsForMobileAR
{
    namespace CoreComponents
    {
        internal interface IGestureInterpreter
        {
            public void OnNewGestureGenerated(List<Vector3> newLandmarks, string detectedGestureName);
        }
    }
}
