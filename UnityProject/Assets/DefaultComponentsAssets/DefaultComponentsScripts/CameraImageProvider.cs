using HandsForMobileAR.CoreComponents;
using Mediapipe;
using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using static UnityEngine.XR.ARSubsystems.XRCpuImage;

namespace HandsForMobileAR
{
    namespace DefaultComponents
    {
        /// <summary>
        /// This class goal is to provide image feed to HandTrackingProvider
        /// </summary>
        public class CameraImageProvider : MonoBehaviour, IImageProvider
        {
            [SerializeField] private ARCameraManager _manager;
            [SerializeField] HandTrackingProvider _handTrackingProvider;

            private Texture2D _textureToProcess;
            private ConversionParams? _conversionParams;

            private void Start()
            {
                _handTrackingProvider.ImageProvider = this;
            }

            public CameraImageProvider(ARCameraManager manager)
            {
                _manager = manager;
            }
            public Vector2Int GetScreenResolution() => new Vector2Int(Screen.width, Screen.height);
            public Vector2Int GetCameraResolution() => new Vector2Int(_conversionParams.Value.outputDimensions.x, _conversionParams.Value.outputDimensions.y);
            
            /// <summary>
            /// This function calculates resolution of scaled camera feed
            /// </summary>
            public Vector2Int GetScaledCameraResolution()
            {
                var widthScale = (float)Screen.width / _conversionParams.Value.outputDimensions.x;
                var scaledHeight = (int)(_conversionParams.Value.outputDimensions.y * widthScale);

                return new Vector2Int(Screen.width, scaledHeight);
            }
            private Texture2D GetTexture2DFromCpuImage(XRCpuImage cpuImage)
            {
                if (_conversionParams is null || _conversionParams.Value.inputRect.width != cpuImage.width)
                {
                    _conversionParams = new ConversionParams
                    {
                        inputRect = new RectInt(0, 0, cpuImage.width, cpuImage.height),

                        outputDimensions = new Vector2Int(cpuImage.width, cpuImage.height),

                        outputFormat = TextureFormat.RGBA32,

                        transformation = Transformation.MirrorX | Transformation.MirrorY
                    };
                }


                // TODO: proparly manage memory. This way is cursed: sometimes rewrites texture that is processing by mediapipe. Results are unpredictable. Needs fix
                if (_textureToProcess is null || _textureToProcess.width != cpuImage.width)
                    _textureToProcess = new Texture2D(cpuImage.width, cpuImage.height, TextureFormat.RGBA32, false);

                try
                {
                    unsafe
                    {
                        cpuImage.Convert(
                          _conversionParams.Value,
                          new IntPtr(_textureToProcess.GetRawTextureData<byte>().GetUnsafePtr()),
                          _textureToProcess.GetRawTextureData<byte>().Length);
                    }
                }
                finally
                {
                    cpuImage.Dispose();
                }

                _textureToProcess.Apply();

                return _textureToProcess;
            }

            public bool TryGetLastImage(out Texture2D image)
            {
                image = null;

                var isImageExist = _manager.TryAcquireLatestCpuImage(out XRCpuImage cpuImage);
                if (isImageExist)
                {
                    image = GetTexture2DFromCpuImage(cpuImage);
                    return isImageExist;
                }

                return isImageExist;
            }
        }
    }
}