using System;
using UnityEngine;

public class SnapPointController : MonoBehaviour
{
    private Transform followPoint;
    private SnapPoint grabbedPoint;
    private Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        if (followPoint != null)
        {
            var positionDelta = Time.deltaTime * 5 * (followPoint.position - grabbedPoint.transform.position);
            rb.MovePosition(transform.position + positionDelta);
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
        grabbedPoint = sender as SnapPoint;
        followPoint = snapTo;
    }
    public void OnSnapReleased(object sender, EventArgs args)
    {
        followPoint = null;
        grabbedPoint = null;
    }
}
