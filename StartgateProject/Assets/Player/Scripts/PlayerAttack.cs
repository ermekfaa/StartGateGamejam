using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Fireball Settings")]
    public GameObject fireballPrefab;  // Fireball prefab referans�
    public BulletData fireballData;   // Fireball i�in ScriptableObject referans�
    public Transform firePoint;       // Merminin ��k�� noktas�

    [Header("Attack Settings")]
    public KeyCode attackKey = KeyCode.E; // Sald�r� tu�u
    public float attackCooldown = 0.5f;   // At��lar aras�ndaki bekleme s�resi

    private float cooldownTimer = 0f;

    private void Update()
    {
        // Mouse pozisyonuna do�ru oyuncuyu d�nd�r
        RotateToMouse();

        // Sald�r� i�in tu�a bas�m� kontrol et
        cooldownTimer -= Time.deltaTime;
        if (Input.GetKeyDown(attackKey) && cooldownTimer <= 0f)
        {
            ShootFireball();
            cooldownTimer = attackCooldown; // Cooldown s�f�rlama
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
}