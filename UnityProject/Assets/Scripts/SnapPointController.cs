using Assets.Scripts;
using System;
using UnityEngine;

public class SnapPointController : MonoBehaviour
{
    public event EventHandler OnPointGrabbed;
    public event EventHandler OnPointReleased;

    private Transform _followPoint;
    private SnapPoint _grabbedPoint;
    private Rigidbody _rb;

    [SerializeField] private LandmarkInterpreter _landmarkInterpreter;
    private Vector3 _palmNormalWhenGrabbed; 
    private Quaternion _initialRotation;


    public bool IsGrabbed { get; private set; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (_followPoint != null)
        {
            var positionDelta = Time.deltaTime * 5f * (_followPoint.position - _grabbedPoint.transform.position);
            _rb.MovePosition(transform.position + positionDelta);

            Quaternion palmRotationDelta = Quaternion.FromToRotation(
                _palmNormalWhenGrabbed,
                GetVectorFromLandmarker(_landmarkInterpreter)
            );
            Quaternion targetRotation = palmRotationDelta * _initialRotation;

            _rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f));
        }
    }

    public void SubscribeSnapPoint(SnapPoint point)
    {
        point.OnGrabbed += OnSnapGrabbed;
        point.OnReleased += OnSnapReleased;
    }

    public void UnSubscribeSnapPoint(SnapPoint point)
    {
        point.OnGrabbed -= OnSnapGrabbed;
        point.OnReleased -= OnSnapReleased;
    }

    public void OnSnapGrabbed(object sender, Transform snapTo)
    {
        _palmNormalWhenGrabbed = GetVectorFromLandmarker(_landmarkInterpreter); 

        _initialRotation = transform.rotation;
        _grabbedPoint = sender as SnapPoint;
        _followPoint = snapTo;
        IsGrabbed = true;
        OnPointGrabbed?.Invoke(this, EventArgs.Empty);
    }
    public void OnSnapReleased(object sender, EventArgs args)
    {
        _followPoint = null;
        _grabbedPoint = null;
        IsGrabbed = false;
        OnPointReleased?.Invoke(this, EventArgs.Empty);
    }

    private Vector3 GetVectorFromLandmarker(LandmarkInterpreter landmarkInterpreter)
    {
        return _landmarkInterpreter.PalmNormal;

        // var defVector = _landmarkInterpreter.LastProcessedLandmarks[8] - _landmarkInterpreter.LastProcessedLandmarks[4];
        // return defVector.normalized;
    }
}
