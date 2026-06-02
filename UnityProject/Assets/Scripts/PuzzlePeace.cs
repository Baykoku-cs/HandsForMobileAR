using System;
using UnityEngine;

namespace Assets.Scripts
{
    internal class PuzzlePeace: MonoBehaviour
    {
        private SnapPointController _snapPointController;
        private Rigidbody _rb;

        private void Awake()
        {
            _snapPointController = GetComponent<SnapPointController>();
            _rb = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            _snapPointController.OnPointGrabbed += OnGrabbed;
            _snapPointController.OnPointReleased += OnReleased;
        }

        private void OnDisable()
        {
            _snapPointController.OnPointGrabbed -= OnGrabbed;
            _snapPointController.OnPointReleased -= OnReleased;
        }

        private void OnGrabbed(object sender, EventArgs args)
        {
            _rb.isKinematic = true;
        }

        private void OnReleased(object sender, EventArgs args)
        {
            if (transform.parent is null)
            {
                _rb.isKinematic = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("PuzzleSlot") && other.transform != transform.parent)
            {
                _snapPointController.OnSnapReleased(this, EventArgs.Empty);
                _rb.isKinematic = true;
                transform.position = other.transform.position - other.transform.forward * 0.01f;
                transform.parent = other.transform;
                transform.forward = other.transform.forward;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.transform == transform.parent)
            {
                Debug.Log("Puzzle out of slot");
                transform.parent = null;
            }
        }
    }
}
