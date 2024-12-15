using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Fireball Settings")]
    public GameObject fireballPrefab;  // Fireball prefab referansı
    public BulletData fireballData;   // Fireball için ScriptableObject referansı
    public Transform firePoint;       // FirePoint referansı

    [Header("Attack Settings")]
    public KeyCode attackKey = KeyCode.E; // Ateş topu saldırı tuşu
    public float attackCooldown = 0.5f;   // Atışlar arasındaki bekleme süresi

    private float cooldownTimer = 0f;

    private void Update()
    {
        // FirePoint'i mouse pozisyonuna doğru döndür
        RotateFirePointToMouse();

        // Ateş topu saldırısı
        cooldownTimer -= Time.deltaTime;
        if (Input.GetKeyDown(attackKey) && cooldownTimer <= 0f)
        {
            ShootFireball();
            cooldownTimer = attackCooldown; // Cooldown sıfırlama
        }
    }

    private void RotateFirePointToMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector2 direction = new Vector2(
            mousePosition.x - firePoint.position.x,
            mousePosition.y - firePoint.position.y
        );

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0, 0, angle); // Sadece FirePoint döndürülür
    }

    private void ShootFireball()
    {
        if (fireballPrefab == null || firePoint == null || fireballData == null)
        {
            Debug.LogError("Fireball Prefab, FirePoint, or FireballData is missing!");
            return;
        }

        // Fireball oluştur ve mouse yönüne doğru hareket ettir
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);

        Fireball fireballScript = fireball.GetComponent<Fireball>();
        if (fireballScript != null)
        {
            fireballScript.bulletData = fireballData;
            fireballScript.owner = gameObject;
        }
    }
}
