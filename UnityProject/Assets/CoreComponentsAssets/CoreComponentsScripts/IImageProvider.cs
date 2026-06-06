namespace Assets.Scripts
{
    internal interface IImageProvider
    {
        public bool TryGetLastImage(out Mediapipe.Image image);
    }
}
