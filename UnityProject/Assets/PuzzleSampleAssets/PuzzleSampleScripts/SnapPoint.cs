using System;
using UnityEngine;

namespace HandsForMobileAR
{
    namespace PuzzleSampleComponents
    {
        public class SnapPoint : MonoBehaviour
        {
            public event EventHandler<Transform> OnGrabbed;
            public event EventHandler OnReleased;

            [SerializeField]
            private SnapPointController _controller;
            [SerializeField]
            private MeshRenderer _pointRendererVisual;

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

            public void Show()
            {
                _pointRendererVisual.enabled = true;
            }
            public void Hide()
            {
                _pointRendererVisual.enabled = false;
            }
        }
    }
}