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
                _landmarkInterpreter.PalmNormal
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
        _palmNormalWhenGrabbed = _landmarkInterpreter.PalmNormal; 
        _initialRotation = transform.rotation;
        _grabbedPoint = sender as SnapPoint;
        _followPoint = snapTo;
        OnPointGrabbed?.Invoke(this, EventArgs.Empty);
    }
    public void OnSnapReleased(object sender, EventArgs args)
    {
        _followPoint = null;
        _grabbedPoint = null;
        OnPointReleased?.Invoke(this, EventArgs.Empty);
    }
}
