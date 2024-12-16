using UnityEngine;
using UnityEngine.UI;

public class BossBar : MonoBehaviour
{
    public Slider healthSlider; // Saðlýk barýný temsil eden slider
    private EnemyBossController bossHealth;

    private void Start()
    {
        // Sahnedeki EnemyBossController bileþenini bul
        bossHealth = FindObjectOfType<EnemyBossController>();

        if (bossHealth != null)
        {
            // Saðlýk barýný maksimum saðlýkla ayarla
            healthSlider.maxValue = bossHealth.maxHealth;
            healthSlider.value = bossHealth.currentHealth;
        }
        else
        {
            Debug.LogError("EnemyBossController scripti sahnede bulunamadý!");
        }
    }

    private void Update()
    {
        // Eðer bossHealth mevcutsa, saðlýk deðerlerini sürekli güncelle
        if (bossHealth != null)
        {
            healthSlider.maxValue = bossHealth.maxHealth;
            healthSlider.value = bossHealth.currentHealth;
        }
    }
}
