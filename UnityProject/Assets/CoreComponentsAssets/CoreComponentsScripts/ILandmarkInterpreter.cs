using System.Collections.Generic;
using UnityEngine;

namespace HandsForMobileAR
{
    namespace CoreComponents
    {
        public interface ILandmarkInterpreter
        {
            public void OnNewLandmarksGenerated(List<Vector3> newLandmarks, IImageProvider imageProvider);
        }
    }
}
