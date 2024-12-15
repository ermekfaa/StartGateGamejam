using UnityEngine;
using UnityEngine.UI;

public class BossBar : MonoBehaviour
{
    public Slider healthSlider; // Sa�l�k bar�n� temsil eden slider
    private BossHealth bossHealth;

    private void Start()
    {
        // Sahnedeki BossHealth bile�enini bul
        bossHealth = FindObjectOfType<BossHealth>();

        if (bossHealth != null)
        {
            // Sa�l�k bar�n� maksimum sa�l�kla ayarla
            healthSlider.maxValue = bossHealth.MaxHealth;
            healthSlider.value = bossHealth.CurrentHealth;

            // BossHealth'ten gelen event'i dinle
            bossHealth.onHealthChanged += UpdateHealthBar;
        }
        else
        {
            Debug.LogError("BossHealth scripti sahnede bulunamad�!");
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
        // Event'i dinlemekten ��k
        if (bossHealth != null)
        {
            bossHealth.onHealthChanged -= UpdateHealthBar;
        }
    }
}
