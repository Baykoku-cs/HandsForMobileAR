using System.Collections;
using UnityEngine;

namespace HandsForMobileAR
{
    namespace PuzzleSampleComponents
    {
        public class ButtonTriger : MonoBehaviour
        {
            [SerializeField]
            private PuzzlePeace[] _puzzles;

            private float _buttonReloadTime = 0.5f;
            private bool _isReloading;

            private void OnTriggerEnter(Collider other)
            {
                if (!_isReloading && other.CompareTag("Button"))
                {
                    Debug.Log("ResetButtonTriggered");
                    other.GetComponent<Animator>().SetTrigger("Click");
                    foreach (var puzzle in _puzzles)
                    {
                        puzzle.ResetPosition();
                    }

                    StartCoroutine(ReloadCorourine());
                }
            }

            private IEnumerator ReloadCorourine()
            {
                _isReloading = true;
                yield return new WaitForSeconds(_buttonReloadTime);
                _isReloading = false;
            }
        }
    }
}