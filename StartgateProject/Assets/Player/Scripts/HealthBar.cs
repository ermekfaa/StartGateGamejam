using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider; // Sa�l�k bar�n� temsil eden slider
    private PlayerHealth playerHealth;

    private void Start()
    {
        // Sahnedeki PlayerHealth bile�enini bul
        playerHealth = FindObjectOfType<PlayerHealth>();

        if (playerHealth != null)
        {
            // Sa�l�k bar�n� maksimum sa�l�kla ayarla
            healthSlider.maxValue = playerHealth.maxHealth;
            healthSlider.value = playerHealth.maxHealth;

            // PlayerHealth'ten gelen event'i dinle
            playerHealth.onHealthChanged += UpdateHealthBar;
        }
        else
        {
            Debug.LogError("PlayerHealth scripti sahnede bulunamad�!");
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
        // Event'i dinlemekten ��k
        if (playerHealth != null)
        {
            playerHealth.onHealthChanged -= UpdateHealthBar;
        }
    }
}
