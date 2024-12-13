using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Fireball Settings")]
    public GameObject fireballPrefab;  // Fireball prefab referansý
    public BulletData fireballData;   // Fireball için ScriptableObject referansý
    public Transform firePoint;       // Merminin çýkýþ noktasý

    [Header("Attack Settings")]
    public KeyCode attackKey = KeyCode.E; // Saldýrý tuþu
    public float attackCooldown = 0.5f;   // Atýþlar arasýndaki bekleme süresi

    private float cooldownTimer = 0f;

    private void Update()
    {
        // Mouse pozisyonuna doðru oyuncuyu döndür
        RotateToMouse();

        // Saldýrý için tuþa basýmý kontrol et
        cooldownTimer -= Time.deltaTime;
        if (Input.GetKeyDown(attackKey) && cooldownTimer <= 0f)
        {
            ShootFireball();
            cooldownTimer = attackCooldown; // Cooldown sýfýrlama
        }
    }

    private void RotateToMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector2 direction = new Vector2(
            mousePosition.x - transform.position.x,
            mousePosition.y - transform.position.y
        );

        transform.right = direction; // Oyuncuyu mouse yönüne döndür
    }

    private void ShootFireball()
    {
        if (fireballPrefab == null || firePoint == null || fireballData == null)
        {
            Debug.LogError("Fireball Prefab, FirePoint, or FireballData is missing!");
            return;
        }

        // Fireball oluþtur ve mouse yönüne doðru hareket ettir
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);

        Fireball fireballScript = fireball.GetComponent<Fireball>();
        if (fireballScript != null)
        {
            fireballScript.bulletData = fireballData;
            fireballScript.owner = gameObject;
        }
    }
}