using System;
using System.Drawing;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using static UnityEngine.XR.ARSubsystems.XRCpuImage;

namespace Assets.Scripts
{
    internal class CameraImageProvider : MonoBehaviour
    {
        [SerializeField]
        private ARCameraManager manager;
        [SerializeField]
        private Camera arCamera;

        private Texture2D _textureToProcess;
        private ConversionParams? _conversionParams;

        public TextMeshProUGUI text;
        public Vector2Int GetScreenResolution() => new Vector2Int(Screen.width, Screen.height);
        public Vector2Int GetCameraResolution() => new Vector2Int(_conversionParams.Value.outputDimensions.x, _conversionParams.Value.outputDimensions.y);

        public Vector2Int GetScaledCameraResolution()
        {
            var widthScale = (float) Screen.width / _conversionParams.Value.outputDimensions.x;
            var scaledHeight = (int)(_conversionParams.Value.outputDimensions.y * widthScale);

            var a = GetCameraResolution();

            text.text = $"camera:{_conversionParams.Value.outputDimensions.x}x{_conversionParams.Value.outputDimensions.y}\nscreen:{Screen.width}x{Screen.height}scaled:{Screen.width}x{scaledHeight}";

            return new Vector2Int(Screen.width, scaledHeight);
        }

        public bool TryGetLastCameraTexture(out Texture2D cameraTexture)
        {
            cameraTexture = null;
            var isImageExist = manager.TryAcquireLatestCpuImage(out XRCpuImage cpuImage);
            if (isImageExist)
            {
                cameraTexture = GetTexture2DFromCpuImage(cpuImage);
                return isImageExist;
            }

            return isImageExist;
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
    }
}
