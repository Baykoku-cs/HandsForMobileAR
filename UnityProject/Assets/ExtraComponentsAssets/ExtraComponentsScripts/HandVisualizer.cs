using Assets.Scripts;
using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandVisualizer : MonoBehaviour
{
    [SerializeField] private LandmarkInterpreter _landmarkInterpreter;

    [SerializeField] private LineRenderer _palmNormalRenderer;
    [SerializeField] private Transform _worldParent;

    private LineRenderer[] _spheres = new LineRenderer[21];
    private Vector3[] _lastTrakedLandmarkVectors = new Vector3[21];

    [SerializeField]
    private LineRenderer _handPointPrefab;

    private bool _isRecentlyUpdated = false;
    private Coroutine _hideHandsDelayedCoroutine;
    
    private void Start()
    {
        for (int i = 0; i < 21; i++)
        {
            _spheres[i] = Instantiate(_handPointPrefab);
            _spheres[i].gameObject.SetActive(true);
            _spheres[i].transform.SetParent(_worldParent);
            _spheres[i].transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        }
    }
    private void FixedUpdate()
    {
        if (_landmarkInterpreter.LastProcessedLandmarks is not null)//_isRecentlyUpdated)
        {
            _palmNormalRenderer.SetPosition(0, _landmarkInterpreter.PalmCenter);
            _palmNormalRenderer.SetPosition(1, _landmarkInterpreter.PalmCenter + _landmarkInterpreter.PalmNormal * 0.1f);

            for (int i = 0; i < 21; i++)
            {
                if (i == 8)
                {
                    _worldParent.position = _landmarkInterpreter.LastProcessedLandmarks[4] + (_landmarkInterpreter.LastProcessedLandmarks[8] - _landmarkInterpreter.LastProcessedLandmarks[4]).normalized * Vector3.Distance(_landmarkInterpreter.LastProcessedLandmarks[8], _landmarkInterpreter.LastProcessedLandmarks[4]) * 0.5f;
                }

                if (_isRecentlyUpdated)
                {
                    _spheres[i].transform.position = _landmarkInterpreter.LastProcessedLandmarks[i];
                }
                else
                {
                    List<int> edgaed = new List<int>() { 4, 8, 12, 16, 20 };
                    _spheres[i].transform.position = _landmarkInterpreter.LastProcessedLandmarks[i];
                    
                    if (!edgaed.Contains(i))
                        _spheres[i].transform.LookAt(_spheres[i+1].transform);
                    else
                        _spheres[i].transform.rotation = _spheres[i - 1].transform.rotation;

                    _spheres[i].SetPosition(1, _landmarkInterpreter.LastProcessedLandmarks[i] + _spheres[i].transform.forward * 0.02f);
                    _spheres[i].SetPosition(0, _landmarkInterpreter.LastProcessedLandmarks[i]);
                }
            }
        }
    }

    public void SendNewLandMarks(Vector3[] landmarkVectors)
    {
        for (int i = 0; i < landmarkVectors.Length; i++)
        {
            _lastTrakedLandmarkVectors[i] = landmarkVectors[i];

            if (!_isRecentlyUpdated)
            {
                _spheres[i].gameObject.SetActive(true);
            }
        }
        _isRecentlyUpdated = true;

        if (_hideHandsDelayedCoroutine != null)
            StopCoroutine(_hideHandsDelayedCoroutine);

        _hideHandsDelayedCoroutine = StartCoroutine(DelayedHideHands());
    }

    private IEnumerator DelayedHideHands()
    {
        yield return new WaitForSeconds(3);
        _isRecentlyUpdated = false;
        HideHands();
    }
    private void HideHands()
    {
        foreach (var sphere in _spheres)
        {
            sphere.gameObject.SetActive(false);
        }
    }
}
