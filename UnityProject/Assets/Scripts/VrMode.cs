using UnityEngine;
using UnityEngine.UI;

public class VrMode : MonoBehaviour
{
    [SerializeField] private Camera CameraToSplit;
    [SerializeField] private RawImage LeftEye;
    [SerializeField] private RawImage RightEye;
    [SerializeField] private bool TriggerSplitModeOn;
    [SerializeField, Range(-0.1f, 0.1f)] private float eyeStep;

    private float _cropPercentage = 0.25f;
    private bool IsSplitModeOn;
    private RenderTexture renderTexture;
    private RenderTexture originalTargetTexture;

    private void Start()
    {
        originalTargetTexture = CameraToSplit.targetTexture;

    }

    private void Update()
    {
        if (!TriggerSplitModeOn.Equals(IsSplitModeOn))
        {
            IsSplitModeOn = TriggerSplitModeOn;
            ChangeMode(IsSplitModeOn);
        }
    }

    private void ChangeMode(bool currentMode)
    {
        var currentScreenRect = GetComponent<RectTransform>();
        var lEyeRect = LeftEye.gameObject.GetComponent<RectTransform>();
        var rEyeRect = RightEye.gameObject.GetComponent<RectTransform>();

        if (currentMode)
        {
            EnableRenderTextureMode();
            var texture = renderTexture;

            float eyeWidth = currentScreenRect.rect.width * 0.5f;

            lEyeRect.sizeDelta = new Vector2(eyeWidth, lEyeRect.sizeDelta.y);
            rEyeRect.sizeDelta = new Vector2(eyeWidth, rEyeRect.sizeDelta.y);

            LeftEye.material.mainTexture = texture;
            LeftEye.texture = texture;
            RightEye.texture = texture;

            LeftEye.enabled = true;
            RightEye.enabled = true;
}
        else
        {
            DisableRenderTextureMode();
            LeftEye.enabled = false;
            RightEye.enabled = false;
        }
    }


    private void EnableRenderTextureMode()
    {
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);

        originalTargetTexture = CameraToSplit.targetTexture;
        CameraToSplit.targetTexture = renderTexture;

        LeftEye.uvRect = new Rect(0.25f - eyeStep, LeftEye.uvRect.y, LeftEye.uvRect.width, LeftEye.uvRect.height);
        RightEye.uvRect = new Rect(0.25f + eyeStep, RightEye.uvRect.y, RightEye.uvRect.width, RightEye.uvRect.height);
    }

    private void DisableRenderTextureMode()
    {
        CameraToSplit.targetTexture = originalTargetTexture;

        if (renderTexture != null)
        {
            renderTexture.Release();
            renderTexture = null;
        }
    }

    public void ChangeMode()
    {
        TriggerSplitModeOn = !TriggerSplitModeOn;
    }
}
