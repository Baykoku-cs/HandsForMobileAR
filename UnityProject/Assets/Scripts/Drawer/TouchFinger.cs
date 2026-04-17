using UnityEngine;
using UnityEngine.Events;

public class TouchFinger : MonoBehaviour
{
    public UnityEvent<Color> OnColorButtonTouched;
    public void OnTriggerEnter(Collider collider)
    {
        OnColorButtonTouched?.Invoke(collider.GetComponent<ColorButton>().Color);
    }
}
