using UnityEngine;
using UnityEngine.UI;

public class SmoothHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private float smoothSpeed = 5f;
    private float targetValue;

    public void SetHealth(float pct) => targetValue = pct;

    void Update()
    {
        slider.value = Mathf.Lerp(slider.value, targetValue, Time.deltaTime * smoothSpeed);
    }
}