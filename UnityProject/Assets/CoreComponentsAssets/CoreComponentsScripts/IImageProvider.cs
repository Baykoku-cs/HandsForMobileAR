using UnityEngine;

namespace HandsForMobileAR
{
    namespace CoreComponents
    {
        /// <summary>
        /// Interface for a class that will provide image feed for mediapipe model
        /// </summary>
        public interface IImageProvider
        {
            /// <summary>
            /// Via this function HandTrackingProvider will get image feed for mediapipe
            /// </summary>
            public bool TryGetLastImage(out Texture2D image);
        }
    }
}