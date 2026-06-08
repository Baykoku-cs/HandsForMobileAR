using UnityEngine;

namespace HandsForMobileAR
{
    namespace CoreComponents
    {
        public interface IImageProvider
        {
            public bool TryGetLastImage(out Texture2D image);
        }
    }
}