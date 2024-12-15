using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Fireball Settings")]
    public GameObject fireballPrefab;  // Fireball prefab referansı
    public BulletData fireballData;   // Fireball için ScriptableObject referansı
    public Transform firePoint;       // FirePoint referansı

    [Header("Golem Bomb Settings")]
    public GameObject golemBombPrefab; // Golem bombası prefab referansı
    public KeyCode golemAttackKey = KeyCode.Q; // Golem bombası saldırı tuşu
    public int maxGolemBombs = 3; // Maksimum golem bombası sayısı
    public float golemAttackCooldown = 2f; // Golem bombası cooldown süresi

    [Header("Attack Settings")]
    public KeyCode attackKey = KeyCode.E; // Ateş topu saldırı tuşu
    public float attackCooldown = 0.5f;   // Atışlar arasındaki bekleme süresi

    private float cooldownTimer = 0f;
    private float golemCooldownTimer = 0f; // Golem bombası için cooldown sayacı
    private int currentGolemBombs = 0; // Atılmış olan golem bombası sayısı

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

        // Golem bombası saldırısı
        golemCooldownTimer -= Time.deltaTime;
        if (Input.GetKeyDown(golemAttackKey) && golemCooldownTimer <= 0f && currentGolemBombs < maxGolemBombs)
        {
            ThrowGolemBomb();
            currentGolemBombs++; // Kullanılan golem bombası sayısını artır
            golemCooldownTimer = golemAttackCooldown; // Cooldown başlat
        }
    }

    private void RotateFirePointToMouse()
    {
        // Mouse'un dünya koordinatını al
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        // Player ile mouse pozisyonu arasındaki yön
        Vector2 direction = new Vector2(
            mousePosition.x - transform.position.x,
            mousePosition.y - transform.position.y
        );

        // FirePoint'i mouse pozisyonuna doğru döndür
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0, 0, angle);

        // FirePoint'in pozisyonunu Player etrafında bir daire üzerinde güncelle
        float radius = 1.5f; // Attack Point'in Player'dan uzaklığı
        firePoint.position = new Vector3(
            transform.position.x + Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
            transform.position.y + Mathf.Sin(angle * Mathf.Deg2Rad) * radius,
            0 // Z ekseni sabit
        );
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

    private void ThrowGolemBomb()
    {
        if (golemBombPrefab == null || firePoint == null)
        {
            Debug.LogError("Golem Bomb Prefab veya FirePoint eksik!");
            return;
        }

        // Golem bombası oluştur
        GameObject golemBomb = Instantiate(golemBombPrefab, firePoint.position, firePoint.rotation);

        Rigidbody2D rb = golemBomb.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Bombayı ileri doğru fırlat
            rb.linearVelocity = firePoint.right * 10f; // Fırlatma hızı
        }

        // Bombanın partikül efektini başlat
        ParticleSystem particleSystem = golemBomb.GetComponentInChildren<ParticleSystem>();
        if (particleSystem != null)
        {
            particleSystem.Play();
        }
    }

    // Level geçişinde sıfırlama
    public void ResetGolemBombs()
    {
        currentGolemBombs = 0; // Golem bombası sayısını sıfırla
        golemCooldownTimer = 0f; // Cooldown sıfırla
    }
}
