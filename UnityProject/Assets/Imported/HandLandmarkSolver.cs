using Mediapipe.Tasks.Vision.Core;
using Mediapipe.Tasks.Vision.FaceLandmarker;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.XR.ARSubsystems.XRCpuImage;

public class HandLandmarkSolver : MonoBehaviour
{
    [SerializeField]
    private TextAsset model;
    [SerializeField]
    private ARCameraManager manager;
    [SerializeField]
    private Camera arCamera;
    
    [SerializeField]
    private int targetFps = 30;
    private float restTime;

    [SerializeField]
    private HandVisualizer handVisualizer;

    private HandLandmarker handLandmarker;
    
    private Stopwatch stopwatch = new Stopwatch();
    private HandLandmarkerResult _detectedLandmarks = default;
    private ConversionParams? conversionParams;

    private void Start()
    {
        restTime = 1 / targetFps;

        handLandmarker = HandLandmarker.CreateFromOptions(new HandLandmarkerOptions(
            new Mediapipe.Tasks.Core.BaseOptions(delegateCase: Mediapipe.Tasks.Core.BaseOptions.Delegate.CPU, 
                                                 modelAssetBuffer: model.bytes),
            Mediapipe.Tasks.Vision.Core.RunningMode.VIDEO
        ));
        
        stopwatch.Start();
    }

    private bool IsReady = true;
    private void FixedUpdate()
    {
        if (IsReady)
        {
            GenerateLandmarks();
            StartCoroutine(Rest(1 / targetFps));
        }
    }

    private IEnumerator Rest(float restTime)
    {
        IsReady = false;
        yield return new WaitForSeconds(restTime);
        IsReady = true;
    }

    private void GenerateLandmarks()
    {
        if (manager.TryAcquireLatestCpuImage(out XRCpuImage cpuImage))
        {
            if (handLandmarker.TryDetectForVideo(new Mediapipe.Image(GetTexture2DFromCpuImage(cpuImage)), stopwatch.ElapsedMilliseconds, default(ImageProcessingOptions), result: ref _detectedLandmarks))
            {
                List<Vector3> landmarkVectors = new();

                for (int i = 0; i < _detectedLandmarks.handLandmarks[0].landmarks.Count; i++)
                {
                    Vector3 screenPoint = new Vector3(_detectedLandmarks.handLandmarks[0].landmarks[i].x * Screen.width, 
                                                      _detectedLandmarks.handLandmarks[0].landmarks[i].y * Screen.height,
                                                      _detectedLandmarks.handLandmarks[0].landmarks[i].z + 0.5f); 

                    landmarkVectors.Add(arCamera.ScreenToWorldPoint(screenPoint));
                }

                handVisualizer.SendNewLandMarks(landmarkVectors.ToArray());

                UnityEngine.Debug.Log("Landmarks placed in world space");
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
}
