using UnityEngine;
using UnityEngine.UI;

public class VrMode : MonoBehaviour
{
    [SerializeField] private Camera CameraToSplit;
    [SerializeField] private RawImage FullScreen;
    [SerializeField] private RawImage LeftEye;
    [SerializeField] private RawImage RightEye;
    [SerializeField] private bool TriggerSplitModeOn;
    [SerializeField, Range(-0.1f, 0.1f)] private float eyeStep;

    private float _cropPercentage = 0.25f;
    private bool IsSplitModeOn;
    private RenderTexture renderTexture;

    private void Start()
    {
        Recount();
        
        FullScreen.texture = renderTexture;
        FullScreen.enabled = true;
    }

    private void OnDestroy()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
            renderTexture = null;
        }
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
            Recount();
            var texture = renderTexture;

            float eyeWidth = currentScreenRect.rect.width * 0.5f;

            lEyeRect.sizeDelta = new Vector2(eyeWidth, lEyeRect.sizeDelta.y);
            rEyeRect.sizeDelta = new Vector2(eyeWidth, rEyeRect.sizeDelta.y);

            LeftEye.texture = texture;
            RightEye.texture = texture;

            LeftEye.enabled = true;
            RightEye.enabled = true;
            FullScreen.enabled = false;
        }
        else
        {
            Recount();
            FullScreen.texture = renderTexture;
            FullScreen.enabled = true;
            LeftEye.enabled = false;
            RightEye.enabled = false;
        }
    }


    private void Recount()
    {
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        // should be cpuImage resolution, not screen
        
        CameraToSplit.targetTexture = renderTexture;

        LeftEye.uvRect = new Rect(0.25f - eyeStep, LeftEye.uvRect.y, LeftEye.uvRect.width, LeftEye.uvRect.height);
        RightEye.uvRect = new Rect(0.25f + eyeStep, RightEye.uvRect.y, RightEye.uvRect.width, RightEye.uvRect.height);
    }

    public void ChangeMode()
    {
        TriggerSplitModeOn = !TriggerSplitModeOn;
    }
}
