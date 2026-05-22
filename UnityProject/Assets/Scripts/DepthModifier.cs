using UnityEngine;

public class DepthModifier
{
    private float _calibratedHandSize;
    private float _zMultiplier;

    public DepthModifier(float zMultiplier = 10f)
    {
        _zMultiplier = zMultiplier;
    }

    public void Calibrate(Vector3[] landmarks)
    {
        _calibratedHandSize = CalculateHandSize(landmarks);
    }

    public Vector3[] Process(Vector3[] landmarks, Vector2Int resolution)
    {
        Vector3[] worldPoints = new Vector3[landmarks.Length];

        float currentSize = CalculateHandSize(landmarks);
        float baseDistance = (_calibratedHandSize / currentSize) * 0.75f;

        for (int i = 0; i < landmarks.Length; i++)
        {
            Vector3 screenPoint = new Vector3(
                landmarks[i].x * resolution.x,
                landmarks[i].y * resolution.y - (resolution.y - Screen.height) * 0.5f,
                // 
                baseDistance + (landmarks[i].z * _zMultiplier)
            );

            worldPoints[i] = Camera.main.ScreenToWorldPoint(screenPoint);
        }

        return worldPoints;
    }

    private float CalculateHandSize(Vector3[] landmarks)
    {
        float size = 0;
        size += Vector3.Distance(landmarks[0], landmarks[5]);
        size += Vector3.Distance(landmarks[5], landmarks[6]);
        size += Vector3.Distance(landmarks[6], landmarks[7]);
        size += Vector3.Distance(landmarks[7], landmarks[8]);
        return size;
    }
}
