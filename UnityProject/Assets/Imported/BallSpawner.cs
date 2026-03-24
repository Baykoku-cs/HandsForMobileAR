using UnityEngine;
using UnityEngine.UI;

public class BallSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject ballPrefab;
    [SerializeField]
    private Button btnSpawnBall;
    [SerializeField]
    private Transform origin;

    private void Start()
    {
        btnSpawnBall.onClick.AddListener(SpawnBall);
    }
    private void SpawnBall()
    {
        Instantiate(ballPrefab, position: origin.position + origin.forward * 0.5f, Quaternion.identity, transform);
    }
}

