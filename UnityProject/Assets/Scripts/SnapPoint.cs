using System;
using UnityEngine;

public class SnapPoint : MonoBehaviour
{
    [SerializeField]    
    private SnapPointController _controller;

    public event EventHandler<Transform> OnGrabbed;
    public event EventHandler OnReleased;

    private void Start()
    {
        if (_controller is null)
            Debug.LogError("Snap point without controller");
        _controller.SubscribeSnapPoint(this);
    }

    public void Snap(Transform snapTo)
    {
        OnGrabbed?.Invoke(this, snapTo);
    }
    public void UnSnap()
    {
        OnReleased?.Invoke(this, EventArgs.Empty);
    }

}
