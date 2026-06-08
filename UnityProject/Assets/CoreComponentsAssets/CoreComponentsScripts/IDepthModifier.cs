using UnityEngine;

namespace HandsForMobileAR
{
    namespace CoreComponents
    {
        public interface IDepthModifier
        {
            public void Process(Vector3[] landmarks, IImageProvider imageProvider);
        }
    }
}
