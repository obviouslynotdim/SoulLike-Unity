using UnityEngine;
using UnityEngine.UI;

public class BossUIManager : MonoBehaviour
{
    [SerializeField] private GameObject bossUIPanel; // The parent object containing the slider/name
    [SerializeField] private Slider bossHealthSlider;
    [SerializeField] private HealthSystem bossHealthSystem;

    void Start()
    {
        if (bossUIPanel != null) bossUIPanel.SetActive(false);
    }

    void Update()
    {
        if (bossUIPanel.activeSelf && bossHealthSystem != null)
        {
            bossHealthSlider.value = bossHealthSystem.GetHealthPercentage();

            // Auto-hide if boss dies
            if (bossHealthSystem.IsDead())
                Invoke(nameof(HideUI), 3f);
        }
    }

    public void ShowUI() => bossUIPanel.SetActive(true);
    public void HideUI() => bossUIPanel.SetActive(false);
}