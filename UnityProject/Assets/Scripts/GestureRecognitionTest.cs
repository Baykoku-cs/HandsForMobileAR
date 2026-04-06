using Mediapipe.Tasks.Vision.Core;
using Mediapipe.Tasks.Vision.GestureRecognizer;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using static UnityEngine.XR.ARSubsystems.XRCpuImage;

public class GestureRecognitionTest : MonoBehaviour
{
    [SerializeField]
    private TextAsset model;
    [SerializeField]
    private ARCameraManager manager;
    [SerializeField]
    private Camera arCamera;

    [SerializeField]
    private int targetFps = 30;

    [SerializeField]
    private HandVisualizer handVisualizer;

    private GestureRecognizer gestureRecognizer;

    private System.Diagnostics.Stopwatch stopwatch = new();
    private GestureRecognizerResult _gestureResult = default;
    private ConversionParams? conversionParams;

    ///
    private JitterFilter _filter = new JitterFilter(0.6f);
    private DepthModifier _depthModifier;
    ///

    private void Start()
    {
        ///
        _depthModifier = new DepthModifier(arCamera, 0.5f);
        ///

        gestureRecognizer = GestureRecognizer.CreateFromOptions(new GestureRecognizerOptions(
            new Mediapipe.Tasks.Core.BaseOptions(delegateCase: Mediapipe.Tasks.Core.BaseOptions.Delegate.CPU,
                                                 modelAssetBuffer: model.bytes),
            Mediapipe.Tasks.Vision.Core.RunningMode.VIDEO,cannedGestureClassifierOptions: new ClassifierOptions()));

        stopwatch.Start();
    }

    private bool IsReady = true;
    private void FixedUpdate()
    {
        if (IsReady)
        {
            GenerateLandmarks();
            StartCoroutine(Rest(1f / targetFps));
        }
    }

    private IEnumerator Rest(float restTime)
    {
        IsReady = false;
        yield return new WaitForSeconds(restTime);
        IsReady = true;
    }

    Vector3[] proxy;
    private void GenerateLandmarks()
    {
        if (manager.TryAcquireLatestCpuImage(out XRCpuImage cpuImage))
        {
            if (gestureRecognizer.TryRecognizeForVideo(new Mediapipe.Image(GetTexture2DFromCpuImage(cpuImage)), stopwatch.ElapsedMilliseconds, default(ImageProcessingOptions), result: ref _gestureResult))
            {
                List<Vector3> landmarkVectors = new();

                for (int i = 0; i < _gestureResult.handLandmarks[0].landmarks.Count; i++)
                {
                    Vector3 screenPoint = new Vector3(_gestureResult.handLandmarks[0].landmarks[i].x,
                                                      _gestureResult.handLandmarks[0].landmarks[i].y,
                                                      _gestureResult.handLandmarks[0].landmarks[i].z);

                    landmarkVectors.Add(screenPoint);
                }

                Vector3[] filteredData = _filter.Filter(landmarkVectors.ToArray());
                proxy = filteredData;
                Vector3[] finalPoints = _depthModifier.Process(filteredData, Screen.width, Screen.height);

                handVisualizer.SendNewLandMarks(finalPoints);

                ProceedGuesters(_gestureResult.gestures);
            }
            else
            {
                UnityEngine.Debug.Log("No hands detected");
            }
        }
        else
        {
            UnityEngine.Debug.Log("No image");
        }
    }


    private Texture2D GetTexture2DFromCpuImage(XRCpuImage cpuImage)
    {
        if (conversionParams is null)
        {
            conversionParams = new ConversionParams
            {
                inputRect = new RectInt(0, 0, cpuImage.width, cpuImage.height),

                outputDimensions = new Vector2Int(cpuImage.width, cpuImage.height),

                outputFormat = TextureFormat.RGBA32,

                transformation = Transformation.MirrorX
            };
        }

        var texture = new Texture2D(cpuImage.width, cpuImage.height, TextureFormat.RGBA32, false);
        try
        {
            unsafe
            {
                cpuImage.Convert(
                  conversionParams.Value,
                  new IntPtr(texture.GetRawTextureData<byte>().GetUnsafePtr()),
                  texture.GetRawTextureData<byte>().Length);
            }
        }
        finally
        {
            cpuImage.Dispose();
        }

        texture.Apply();

        return texture;
    }

    public void Calibrate()
    {
        _depthModifier.Calibrate(proxy);
    }

    private void ProceedGuesters(List<Mediapipe.Tasks.Components.Containers.Classifications> classifications)
    {
        Debug.Log("Gestures");
        int i = 0;
        foreach (var classification in classifications)
        {
            Debug.Log($"i:{i}, h_name:{classification.headName}, h_index:{classification.headIndex}");
            int j = 0;
            foreach (var category in classification.categories) {
                Debug.Log($"j:{j}, c_name:{category.categoryName}, c_index:{category.index}, c_displayName:{category.displayName}, c_score:{category.score}");
            }
        }
    }
}
