using Assets.Scripts;
using System;
using UnityEngine;

public class SnapPointController : MonoBehaviour
{
    private Transform followPoint;
    private SnapPoint grabbedPoint;
    private Rigidbody rb;


    public event EventHandler OnPointGrabbed;
    public event EventHandler OnPointReleased;

    [SerializeField] private LandmarkInterpreter _landmarkInterpreter;
    private Vector3 _palmNormalWhenGrabbed; 
    private Quaternion _initialRotation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (followPoint != null)
        {
            var positionDelta = Time.deltaTime * 5f * (followPoint.position - grabbedPoint.transform.position);
            rb.MovePosition(transform.position + positionDelta);

            Quaternion palmRotationDelta = Quaternion.FromToRotation(
                _palmNormalWhenGrabbed,
                _landmarkInterpreter.PalmNormal
            );
            Quaternion targetRotation = palmRotationDelta * _initialRotation;

            rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f));
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
        grabbedPoint = sender as SnapPoint;
        followPoint = snapTo;
        OnPointGrabbed?.Invoke(this, EventArgs.Empty);
    }
    public void OnSnapReleased(object sender, EventArgs args)
    {
        followPoint = null;
        grabbedPoint = null;
        OnPointReleased?.Invoke(this, EventArgs.Empty);
    }
}
