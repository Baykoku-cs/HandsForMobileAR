using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity.CoordinateSystem;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace HandsForMobileAR
{
    public class HandLandmarkerTest : MonoBehaviour
    {
        [SerializeField] private ARCameraBackground aRCameraBackground;
        [SerializeField] private int width;
        [SerializeField] private int height;
        [SerializeField] private int fps;

        [SerializeField] private RectTransform annotationParent;

        [SerializeField] private TextAsset modelAsset;

        private WebCamTexture webCamTexture;

        private IEnumerator Start()
        {
            if (WebCamTexture.devices.Length == 0)
            {
                throw new System.Exception("Web Camera devices are not found");
            }
            var webCamDevice = WebCamTexture.devices[0];
            webCamTexture = new WebCamTexture(webCamDevice.name, width, height, fps);
            webCamTexture.Play();

            // NOTE: On macOS, the contents of webCamTexture may not be readable immediately, so wait until it is readable
            yield return new WaitUntil(() => webCamTexture.width > 16);

            //screen.rectTransform.sizeDelta = new Vector2(width, height);
            //screen.texture = webCamTexture;

            var options = new HandLandmarkerOptions(
            baseOptions: new Mediapipe.Tasks.Core.BaseOptions(
              Mediapipe.Tasks.Core.BaseOptions.Delegate.CPU,
              modelAssetBuffer: modelAsset.bytes
            ),
            runningMode: Mediapipe.Tasks.Vision.Core.RunningMode.VIDEO
            );

            var handLandmarker = HandLandmarker.CreateFromOptions(options);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var waitForEndOfFrame = new WaitForEndOfFrame();





            /*

            // Получаем текстуру из ARCameraBackground
            Texture texture = aRCameraBackground.customMaterial.mainTexture;
            // Проверяем, что это Texture2D
            if (!(texture is Texture2D))
            {
                UnityEngine.Debug.LogWarning("Texture is not a Texture2D");
            }
            Texture2D texture2D = texture as Texture2D;

            using var textureFrame = new Mediapipe.Unity.Experimental.TextureFrame(texture2D.width, texture2D.height, TextureFormat.RGBA32);

            textureFrame.ReadTextureOnCPU(texture2D, flipHorizontally: false, flipVertically: true);
            using var image = textureFrame.BuildCPUImage();

            var result = handLandmarker.Detect(image);
            UnityEngine.Debug.Log(result);

            var screenRect = screen.rectTransform.rect;
            foreach (var landmark in result.handLandmarks[0].landmarks) {
                var position = screenRect.GetPoint(in landmark);

                var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.SetParent(screen.transform);
                sphere.transform.localScale = new Vector3(10f, 10f, 10f);
                sphere.transform.localPosition = position;
            }
            yield return waitForEndOfFrame;
            */

            while (true)
            {
                // Получаем текстуру из ARCameraBackground
                Texture texture = aRCameraBackground.customMaterial.mainTexture;
                // Проверяем, что это Texture2D
                if (!(texture is Texture2D))
                {
                    UnityEngine.Debug.LogWarning("Texture is not a Texture2D");
                }
                Texture2D texture2D = texture as Texture2D;

                using var textureFrame = new Mediapipe.Unity.Experimental.TextureFrame(texture2D.width, texture2D.height, TextureFormat.RGBA32);

                textureFrame.ReadTextureOnCPU(texture2D, flipHorizontally: false, flipVertically: true);
                using var image = textureFrame.BuildCPUImage();

                var result = handLandmarker.DetectForVideo(image, stopwatch.ElapsedMilliseconds);

                var screenRect = annotationParent.rect;
                foreach (var landmark in result.handLandmarks[0].landmarks)
                {
                    var position = screenRect.GetPoint(in landmark);

                    var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    sphere.transform.SetParent(annotationParent.transform);
                    sphere.transform.localScale = new Vector3(10f, 10f, 10f);
                    sphere.transform.localPosition = position;
                }

                yield return waitForEndOfFrame;
            }
        }
        private void OnDestroy()
        {
            if (webCamTexture != null)
            {
                webCamTexture.Stop();
            }
        }
    }
}