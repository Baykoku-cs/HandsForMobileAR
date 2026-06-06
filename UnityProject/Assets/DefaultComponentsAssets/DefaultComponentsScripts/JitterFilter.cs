using UnityEngine;
public class JitterFilter
{
    private Vector3[] _filteredLandmarks;
    private float _smoothingFactor;

    public JitterFilter(float smoothingFactor = 0.2f)
    {
        _smoothingFactor = smoothingFactor;
    }

    public Vector3[] Filter(Vector3[] rawLandmarks)
    {
        if (_filteredLandmarks == null || _filteredLandmarks.Length != rawLandmarks.Length)
        {
            _filteredLandmarks = new Vector3[rawLandmarks.Length];
            System.Array.Copy(rawLandmarks, _filteredLandmarks, rawLandmarks.Length);
            return _filteredLandmarks;
        }

        for (int i = 0; i < rawLandmarks.Length; i++)
        {
            _filteredLandmarks[i] = Vector3.Lerp(_filteredLandmarks[i], rawLandmarks[i], _smoothingFactor);
        }

        return _filteredLandmarks;
    }
}
