using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100; // Oyuncunun maksimum caný
    private int currentHealth; // Oyuncunun þu anki caný

    public delegate void OnHealthChanged(int currentHealth, int maxHealth);
    public event OnHealthChanged onHealthChanged; // UI'yi bilgilendirmek için event

    private void Start()
    {
        currentHealth = maxHealth; // Oyuncu baþlangýçta tam canla baþlar
        Debug.Log($"Player spawned with {currentHealth} health.");
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0) return; // Negatif hasar almayý önler

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Can 0'dan aþaðý düþmesin
        Debug.Log($"Player took {damage} damage. Current health: {currentHealth}");

        // UI güncellemesi için event tetiklenir
        onHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        if (healAmount <= 0) return; // Negatif iyileþtirme yok

        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Maksimum caný aþmasýn
        Debug.Log($"Player healed by {healAmount}. Current health: {currentHealth}");

        // UI güncellemesi için event tetiklenir
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        Debug.Log("Player has died.");
        // EnemyManager'daki EnemyDefeated fonksiyonunu çaðýr
        GameObject manager = GameObject.Find("EnemyManager");
        if (manager != null)
        {
            manager.GetComponent<EnemyManager>().EnemyDefeated();
        }
        Destroy(gameObject); // Düþmaný yok et

    }
}
