namespace HandsForMobileAR
{
    namespace CoreComponents
    {
        public interface IImageProvider
        {
            public bool TryGetLastImage(out Mediapipe.Image image);
        }
    }
}