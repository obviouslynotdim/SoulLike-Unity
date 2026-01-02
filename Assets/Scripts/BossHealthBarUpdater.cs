using UnityEngine;
using UnityEngine.UI;

public class BossHealthBarUpdater : MonoBehaviour
{
    [SerializeField] private Slider slider; // The Slider UI component
    [SerializeField] private HealthSystem bossHealth; // The HealthSystem on the Boss

    void Update()
    {
        // Every frame, ask the boss for its health percentage and update the slider
        if (bossHealth != null && slider != null)
        {
            slider.value = bossHealth.GetHealthPercentage();
        }
    }
}