using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider; // Saðlýk barýný temsil eden slider
    private PlayerHealth playerHealth;

    private void Start()
    {
        // Sahnedeki PlayerHealth bileþenini bul
        playerHealth = FindObjectOfType<PlayerHealth>();

        if (playerHealth != null)
        {
            // Saðlýk barýný maksimum saðlýkla ayarla
            healthSlider.maxValue = playerHealth.maxHealth;
            healthSlider.value = playerHealth.maxHealth;

            // PlayerHealth'ten gelen event'i dinle
            playerHealth.onHealthChanged += UpdateHealthBar;
        }
        else
        {
            Debug.LogError("PlayerHealth scripti sahnede bulunamadý!");
        }
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
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
        if (playerHealth != null)
        {
            playerHealth.onHealthChanged -= UpdateHealthBar;
        }
    }
}
