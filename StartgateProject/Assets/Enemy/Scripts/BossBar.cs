using UnityEngine;
using UnityEngine.UI;

public class BossBar : MonoBehaviour
{
    public Slider healthSlider; // Saðlýk barýný temsil eden slider
    private BossHealth bossHealth;

    private void Start()
    {
        // Sahnedeki BossHealth bileþenini bul
        bossHealth = FindObjectOfType<BossHealth>();

        if (bossHealth != null)
        {
            // Saðlýk barýný maksimum saðlýkla ayarla
            healthSlider.maxValue = bossHealth.MaxHealth;
            healthSlider.value = bossHealth.CurrentHealth;

            // BossHealth'ten gelen event'i dinle
            bossHealth.onHealthChanged += UpdateHealthBar;
        }
        else
        {
            Debug.LogError("BossHealth scripti sahnede bulunamadý!");
        }
    }

    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    private void OnDestroy()
    {
        // Event'i dinlemekten çýk
        if (bossHealth != null)
        {
            bossHealth.onHealthChanged -= UpdateHealthBar;
        }
    }
}
