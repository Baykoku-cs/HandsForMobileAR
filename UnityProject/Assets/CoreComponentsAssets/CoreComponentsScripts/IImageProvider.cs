using UnityEngine;

namespace HandsForMobileAR
{
    namespace CoreComponents
    {
        public interface IImageProvider
        {
            /// <summary>
            /// Via this function HandTrackingProvider will get image feed for mediapipe
            /// </summary>
            public bool TryGetLastImage(out Texture2D image);
        }
    }
}