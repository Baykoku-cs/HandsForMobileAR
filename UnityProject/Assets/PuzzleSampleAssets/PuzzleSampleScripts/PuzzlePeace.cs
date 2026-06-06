using System;
using UnityEngine;

namespace Assets.Scripts
{
    internal class PuzzlePeace: MonoBehaviour
    {
        private SnapPointController _snapPointController;
        private Rigidbody _rb;

        private bool _isSnapped;

        [SerializeField] private Transform _spawnPoint;

        private void Awake()
        {
            _snapPointController = GetComponent<SnapPointController>();
            _rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            ResetPosition();
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
            if (_isSnapped)
            {
                DetachFromSlot();
            }

            _rb.isKinematic = true;
            _rb.useGravity = false; 
        }

        private void OnReleased(object sender, EventArgs args)
        {
            if (!_isSnapped)
            {
                _rb.isKinematic = false;
                _rb.useGravity = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("PuzzleSlot")) return;
            if (_isSnapped) return;

            if (Vector3.Distance(transform.position, other.transform.position) < 0.2f)
            {
                SnapToSlot(other.transform);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("PuzzleSlot")) return;
            if (!_isSnapped) return;

            if (other.transform == transform.parent)
            {
                DetachFromSlot();

                if (!_snapPointController.IsGrabbed)
                {
                    _rb.isKinematic = false;
                    _rb.useGravity = true;
                }
            }
        }


        private void SnapToSlot(Transform slot)
        {
            _isSnapped = true;
            _rb.isKinematic = true;
            _rb.useGravity = false;

            transform.position = slot.position - slot.forward * 0.01f;
            transform.forward = slot.forward;
            transform.parent = slot;
            transform.localRotation = Quaternion.identity;

            _snapPointController.OnSnapReleased(this, EventArgs.Empty);
        }

        private void DetachFromSlot()
        {
            _isSnapped = false;
            transform.parent = null;
        }

        public void ResetPosition()
        {
            if (!_isSnapped)
            {
                _rb.linearVelocity = Vector3.zero;
                transform.position = _spawnPoint.position + Vector3.one * UnityEngine.Random.value * 0.1f;
            }
        }
    }
}
