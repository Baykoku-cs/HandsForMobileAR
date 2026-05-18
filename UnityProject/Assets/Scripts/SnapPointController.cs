using System;
using UnityEngine;

public class SnapPointController : MonoBehaviour
{
    private Transform followPoint;
    private SnapPoint grabbedPoint;

    private void Update()
    {
        if (followPoint != null)
        {
            if (Vector3.Distance(grabbedPoint.transform.position, followPoint.position) > 0.005f)
            {
                transform.Translate(followPoint.position - grabbedPoint.transform.position);
            }
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
