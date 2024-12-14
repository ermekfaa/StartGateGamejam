using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Fireball Settings")]
    public GameObject fireballPrefab;  // Fireball prefab referans�
    public BulletData fireballData;   // Fireball i�in ScriptableObject referans�
    public Transform firePoint;       // Merminin ��k�� noktas�

    [Header("Golem Bomb Settings")]
    public GameObject golemBombPrefab; // Golem bombas� prefab referans�
    public KeyCode golemAttackKey = KeyCode.Q; // Golem bombas� sald�r� tu�u

    [Header("Attack Settings")]
    public KeyCode attackKey = KeyCode.E; // Ate� topu sald�r� tu�u
    public float attackCooldown = 0.5f;   // At��lar aras�ndaki bekleme s�resi

    private float cooldownTimer = 0f;

    private void Update()
    {
        // Mouse pozisyonuna do�ru oyuncuyu d�nd�r
        RotateToMouse();

        // Ate� topu sald�r�s�
        cooldownTimer -= Time.deltaTime;
        if (Input.GetKeyDown(attackKey) && cooldownTimer <= 0f)
        {
            ShootFireball();
            cooldownTimer = attackCooldown; // Cooldown s�f�rlama
        }

        // Golem bombas� sald�r�s�
        if (Input.GetKeyDown(golemAttackKey))
        {
            ThrowGolemBomb();
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

        transform.right = direction; // Oyuncuyu mouse y�n�ne d�nd�r
    }

    private void ShootFireball()
    {
        if (fireballPrefab == null || firePoint == null || fireballData == null)
        {
            Debug.LogError("Fireball Prefab, FirePoint, or FireballData is missing!");
            return;
        }

        // Fireball olu�tur ve mouse y�n�ne do�ru hareket ettir
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

}
