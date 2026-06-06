using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

namespace Assembly_CSharp
{
    internal interface ILandmarkInterpreter
    {
        public void OnNewLandmarksGenerated(List<Vector3> newLandmarks, IImageProvider imageProvider);
    }
}
