using UnityEngine;

namespace HandsForMobileAR
{
    namespace CoreComponents
    {
        public interface IDepthModifier
        {
            /// <summary>
            /// Converts normalized landmarks into world-position
            /// </summary>
            /// <param name="landmarks">Normalized landmarks</param>
            /// <param name="imageProvider">Image provider that was source of image that was used to get landmarks</param>
            public void Process(Vector3[] landmarks, IImageProvider imageProvider);
        }
    }
}
