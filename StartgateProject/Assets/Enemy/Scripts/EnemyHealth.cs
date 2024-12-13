using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Data")]
    public EnemyData enemyData; // EnemyData ScriptableObject referans�

    private int currentHealth; // D��man�n mevcut can�

    private void Start()
    {
        if (enemyData == null)
        {
            Debug.LogError("EnemyData is not assigned!");
            Destroy(gameObject);
            return;
        }

        // ScriptableObject'teki maxHealth'i kullanarak can� ayarla
        currentHealth = enemyData.health;
        Debug.Log($"{enemyData.enemyName} spawned with {currentHealth} health.");
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, enemyData.health); // Can 0'�n alt�na d��mesin
        Debug.Log($"{enemyData.enemyName} took {damage} damage. Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{enemyData.enemyName} has died.");
        // D��man �ld���nde yap�lacak i�lemler burada
        Destroy(gameObject); // �rne�in, sahneden kald�rma
    }
}
