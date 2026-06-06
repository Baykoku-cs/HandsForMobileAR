using UnityEngine;

namespace HandsForMobileAR
{
    namespace CoreComponents
    {
        internal interface IDepthModifier
        {
            public void Process(Vector3[] landmarks, IImageProvider imageProvider);
        }
    }
}
