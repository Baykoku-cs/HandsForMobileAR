using NUnit.Framework.Constraints;
using System;
using UnityEngine;
using UnityEngine.UI;

public class VrMode : MonoBehaviour
{
    [SerializeField] private GameObject _uiGameObject;
    // To Do: needs to be removed
    [SerializeField]
    private Slider eyeStepSlider;

    [SerializeField] private Camera cameraToSplit;
    [SerializeField] private RawImage fullScreen;
    [SerializeField] private RawImage leftEye;
    [SerializeField] private RawImage rightEye;
    [SerializeField] private bool triggerSplitModeOn;
    [SerializeField, Range(-0.1f, 0.1f)] private float eyeStep;

    private float _cropPercentage = 0.25f;
    private bool isSplitModeOn;
    private RenderTexture renderTexture;

    private void Start()
    {
        Recount();
        
        fullScreen.texture = renderTexture;
        fullScreen.enabled = true;


        eyeStepSlider.onValueChanged.AddListener(SetEyeStep);
    }

    private void OnDestroy()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
            renderTexture = null;
        }

        eyeStepSlider.onValueChanged.RemoveListener(SetEyeStep);
    }

    private void Update()
    {
        if (!triggerSplitModeOn.Equals(isSplitModeOn))
        {
            isSplitModeOn = triggerSplitModeOn;
            ChangeMode(isSplitModeOn);
        }
    }

    private void ChangeMode(bool currentMode)
    {
        _uiGameObject.SetActive(!currentMode);
        eyeStepSlider.gameObject.SetActive(currentMode);

        var currentScreenRect = GetComponent<RectTransform>();
        var lEyeRect = leftEye.gameObject.GetComponent<RectTransform>();
        var rEyeRect = rightEye.gameObject.GetComponent<RectTransform>();

        if (currentMode)
        {
            Recount();
            var texture = renderTexture;

            float eyeWidth = currentScreenRect.rect.width * 0.5f;

            lEyeRect.sizeDelta = new Vector2(eyeWidth, lEyeRect.sizeDelta.y);
            rEyeRect.sizeDelta = new Vector2(eyeWidth, rEyeRect.sizeDelta.y);

            leftEye.texture = texture;
            rightEye.texture = texture;

            leftEye.enabled = true;
            rightEye.enabled = true;
            fullScreen.enabled = false;
        }
        else
        {
            Recount();
            fullScreen.texture = renderTexture;
            fullScreen.enabled = true;
            leftEye.enabled = false;
            rightEye.enabled = false;
        }
    }


    private void Recount()
    {
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        // should be cpuImage resolution, not screen
        
        cameraToSplit.targetTexture = renderTexture;

        leftEye.uvRect = new Rect(0.25f - eyeStep, leftEye.uvRect.y, leftEye.uvRect.width, leftEye.uvRect.height);
        rightEye.uvRect = new Rect(0.25f + eyeStep, rightEye.uvRect.y, rightEye.uvRect.width, rightEye.uvRect.height);

        fullScreen.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
    }

    public void ChangeMode()
    {
        triggerSplitModeOn = !triggerSplitModeOn;
    }

    public void SetEyeStep(float value)
    {
        eyeStep = (value * 0.1f - 0.05f) * 2;

        leftEye.uvRect = new Rect(0.25f - eyeStep, leftEye.uvRect.y, leftEye.uvRect.width, leftEye.uvRect.height);
        rightEye.uvRect = new Rect(0.25f + eyeStep, rightEye.uvRect.y, rightEye.uvRect.width, rightEye.uvRect.height);
    }
}
