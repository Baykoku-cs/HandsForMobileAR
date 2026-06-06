using System.Collections.Generic;
using UnityEngine;

namespace HandsForMobileAR
{
    namespace CoreComponents
    {
        internal interface ILandmarkInterpreter
        {
            public void OnNewLandmarksGenerated(List<Vector3> newLandmarks, IImageProvider imageProvider);
        }
    }
}
