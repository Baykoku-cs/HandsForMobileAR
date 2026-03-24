using System.Collections;
using UnityEngine;

public class HandVisualizer : MonoBehaviour
{
    [SerializeField]
    private Transform worldParent;
    private Rigidbody[] spheres = new Rigidbody[21];
    private Vector3[] lastTrakedLandmarkVectors = new Vector3[21];

    [SerializeField]
    private Rigidbody handPointPrefab;

    private bool isRecentlyUpdated = false;
    private Coroutine hideHandsDelayedCoroutine;
    
    private void Start()
    {
        for (int i = 0; i < 21; i++)
        {
            spheres[i] = Instantiate(handPointPrefab);
            spheres[i].gameObject.SetActive(false);
            spheres[i].transform.SetParent(worldParent);
            spheres[i].transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        }
    }

    private void FixedUpdate()
    {
        if (isRecentlyUpdated)
        {
            for (int i = 0; i < 21; i++)
            {
                if (i == 0)
                {
                    worldParent.position = lastTrakedLandmarkVectors[i];
                }

                if (isRecentlyUpdated)
                {
                    spheres[i].MovePosition(lastTrakedLandmarkVectors[i]);

                }
                else
                {
                    spheres[i].transform.position = lastTrakedLandmarkVectors[i];
                }
            }
        }
    }

    public void SendNewLandMarks(Vector3[] landmarkVectors)
    {
        for (int i = 0; i < landmarkVectors.Length; i++)
        {
            lastTrakedLandmarkVectors[i] = landmarkVectors[i];

            if (!isRecentlyUpdated)
            {
                spheres[i].gameObject.SetActive(true);
            }
        }
        isRecentlyUpdated = true;

        if (hideHandsDelayedCoroutine != null)
            StopCoroutine(hideHandsDelayedCoroutine);

        hideHandsDelayedCoroutine = StartCoroutine(DelayedHideHands());
    }

    private IEnumerator DelayedHideHands()
    {
        yield return new WaitForSeconds(3);
        isRecentlyUpdated = false;
        HideHands();
    }
    private void HideHands()
    {
        foreach (var sphere in spheres)
        {
            sphere.gameObject.SetActive(false);
        }
    }
}

// ToDo:
// 3. Фильтр 1 Евро (1€ Filter)Это адаптивный фильтр низких частот, который был создан специально для интерактивных систем (мышки, трекинг рук, AR/VR), чтобы решить проблему компромисса между дрожанием в покое и задержкой при движении.Как работает: Это тот же EMA (пункт 2), но коэффициент $\alpha$ меняется динамически в зависимости от скорости движения точки.Когда скорость близка к нулю (состояние покоя), $\alpha$ становится очень маленьким $\rightarrow$ сильное сглаживание, точка замирает.Когда скорость высокая (резкое движение), $\alpha$ стремится к 1 $\rightarrow$ сглаживание отключается, задержка пропадает, точка мгновенно следует за курсором.Плюсы: Идеальный баланс. Практически индустриальный стандарт для UI и трекинга движений.Минусы: Чуть сложнее в реализации (требует вычисления производной/скорости и настройки двух параметров: минимальной частоты среза и коэффициента скорости).