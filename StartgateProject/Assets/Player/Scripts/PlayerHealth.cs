using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100; // Oyuncunun maksimum can�
    private int currentHealth; // Oyuncunun �u anki can�

    public delegate void OnHealthChanged(int currentHealth, int maxHealth);
    public event OnHealthChanged onHealthChanged; // UI'yi bilgilendirmek i�in event

    private void Start()
    {
        currentHealth = maxHealth; // Oyuncu ba�lang��ta tam canla ba�lar
        Debug.Log($"Player spawned with {currentHealth} health.");
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0) return; // Negatif hasar almay� �nler

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Can 0'dan a�a�� d��mesin
        Debug.Log($"Player took {damage} damage. Current health: {currentHealth}");

        // UI g�ncellemesi i�in event tetiklenir
        onHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        if (healAmount <= 0) return; // Negatif iyile�tirme yok

        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Maksimum can� a�mas�n
        Debug.Log($"Player healed by {healAmount}. Current health: {currentHealth}");

        // UI g�ncellemesi i�in event tetiklenir
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        Debug.Log("Player has died.");
        // EnemyManager'daki EnemyDefeated fonksiyonunu �a��r
        GameObject manager = GameObject.Find("EnemyManager");
        if (manager != null)
        {
            manager.GetComponent<EnemyManager>().EnemyDefeated();
        }
        Destroy(gameObject); // D��man� yok et

    }
}
