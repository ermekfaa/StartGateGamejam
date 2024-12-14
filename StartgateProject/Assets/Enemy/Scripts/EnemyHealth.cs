using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Data")]
    public EnemyData enemyData; // EnemyData ScriptableObject referansý

    private int currentHealth; // Düþmanýn mevcut caný

    private void Start()
    {
        if (enemyData == null)
        {
            Debug.LogError("EnemyData is not assigned!");
            Destroy(gameObject);
            return;
        }

        // ScriptableObject'teki maxHealth'i kullanarak caný ayarla
        currentHealth = enemyData.health;
        Debug.Log($"{enemyData.enemyName} spawned with {currentHealth} health.");
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, enemyData.health); // Can 0'ýn altýna düþmesin
        Debug.Log($"{enemyData.enemyName} took {damage} damage. Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{enemyData.enemyName} has died.");
        // Ölme animasyonu veya efektlerini burada yapabilirsiniz
        Debug.Log(gameObject.name + " öldü!");

        // EnemyManager'daki EnemyDefeated fonksiyonunu çaðýr
        GameObject manager = GameObject.Find("EnemyManager");
        if (manager != null)
        {
            manager.GetComponent<EnemyManager>().EnemyDefeated();
        }

        Destroy(gameObject); // Düþmaný yok et
    }
}
