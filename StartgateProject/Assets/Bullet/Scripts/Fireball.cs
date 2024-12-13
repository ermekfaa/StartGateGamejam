using UnityEngine;

public class Fireball : MonoBehaviour
{
    [Header("Bullet Settings")]
    public BulletData bulletData; // BulletData ScriptableObject referans�
    public GameObject owner;      // Mermiyi f�rlatan nesne

    private Rigidbody2D rb;

    private void Start()
    {
        if (bulletData == null)
        {
            Debug.LogError("BulletData is not assigned!");
            Destroy(gameObject);
            return;
        }

        // Rigidbody al�n�r ve h�z� ayarlan�r
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = transform.right * bulletData.bulletSpeed;
        }

        // Mermi, ya�am s�resi sonunda yok edilir
        Destroy(gameObject, bulletData.bulletLifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == owner)
        {
            // Mermi, kendi sahibine �arpmaz
            return;
        }

        // Player'� hedef alan bir mermi
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(bulletData.bulletDamage);
                Debug.Log($"Player hit by fireball for {bulletData.bulletDamage} damage!");
            }
        }

        // Enemy'yi hedef alan bir mermi
        if (collision.CompareTag("Enemy"))
        {
            EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(bulletData.bulletDamage);
                Debug.Log($"Enemy hit by fireball for {bulletData.bulletDamage} damage!");
            }
        }

        // E�er mermi delici de�ilse veya t�m delici haklar� t�kendiyse yok et
        bulletData.bulletPierce--;
        if (bulletData.bulletPierce <= 0)
        {
            Destroy(gameObject);
        }
    }
}
