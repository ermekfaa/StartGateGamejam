using UnityEngine;
using UnityEngine.UI;

public class BossBar : MonoBehaviour
{
    public Slider healthSlider; // Sa�l�k bar�n� temsil eden slider
    private EnemyBossController bossHealth;

    private void Start()
    {
        // Sahnedeki EnemyBossController bile�enini bul
        bossHealth = FindObjectOfType<EnemyBossController>();

        if (bossHealth != null)
        {
            // Sa�l�k bar�n� maksimum sa�l�kla ayarla
            healthSlider.maxValue = bossHealth.maxHealth;
            healthSlider.value = bossHealth.currentHealth;
        }
        else
        {
            Debug.LogError("EnemyBossController scripti sahnede bulunamad�!");
        }
    }

    private void Update()
    {
        // E�er bossHealth mevcutsa, sa�l�k de�erlerini s�rekli g�ncelle
        if (bossHealth != null)
        {
            healthSlider.maxValue = bossHealth.maxHealth;
            healthSlider.value = bossHealth.currentHealth;
        }
    }
}
