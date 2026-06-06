using Assets.Scripts;
using UnityEngine;

public class ColorPicker : MonoBehaviour
{
    [SerializeField]
    private LandmarkInterpreter _landmarkIntepreter;
    [SerializeField]
    private PoseDetectBus _poseDetectBus;
    [SerializeField]
    private TubeDrawer _tubeDrawer;

    [SerializeField]
    private GameObject ColorPallete;
    [SerializeField]
    private TouchFinger _touchFingerPrefab;

    private TouchFinger tfHolder;
    private GameObject palleteHolder;
    private void Start()
    {
        _poseDetectBus.SubscribeOnPoseDetected(EventType.OnPoseDetected, PoseType.Open_Palm, SpawnDialog);
    }
    private void OnDestroy()
    {
        _poseDetectBus.UnSubscribeOnPoseDetected(EventType.OnPoseDetected, PoseType.Open_Palm, SpawnDialog);
    }

    private void Update()
    {
        if (tfHolder is not null)
        {
            tfHolder.transform.position = _landmarkIntepreter.LastProcessedLandmarks[8];
        }
    }

    private void SpawnDialog()
    {
        var worldPos = _landmarkIntepreter.LastProcessedLandmarks;

        if (palleteHolder is not null)
        {
            Destroy(palleteHolder);
            Destroy(tfHolder.gameObject);
            palleteHolder = null;
            tfHolder = null;
        }

        palleteHolder = Instantiate(ColorPallete, worldPos[12] + Vector3.up * 0.1f, Camera.main.transform.rotation);
        tfHolder = Instantiate(_touchFingerPrefab);

        tfHolder.OnColorButtonTouched.AddListener(OnColorPicked);
    }

    private void OnColorPicked(Color color)
    {
        Destroy(palleteHolder);
        Destroy(tfHolder.gameObject);
        palleteHolder = null;
        tfHolder = null;

        _tubeDrawer.SetBrushColor(color);
    }

}
