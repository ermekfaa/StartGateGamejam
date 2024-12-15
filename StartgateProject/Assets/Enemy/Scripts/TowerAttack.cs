using System.Collections;
using UnityEngine;

public class TowerAttack : MonoBehaviour
{
    [Header("Enemy Data")]
    public EnemyData enemyData;

    [Header("Fireball Settings")]
    public GameObject fireballPrefab;
    public BulletData fireballData;
    public Transform firePoint;

    private GameObject player;         // Oyuncunun referansý
    private float attackCooldown;      // Saldýrý bekleme süresi

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player object not found in the scene!");
        }

        attackCooldown = enemyData.waitingTime; // Bekleme süresini baþlat
    }

    private void Update()
    {
        // Oyuncuyu algýla ve saldýr
        DetectAndAttackPlayer();
    }

    private void DetectAndAttackPlayer()
    {
        if (player == null) return;

        // Oyuncunun tarete olan mesafesini kontrol et
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= enemyData.aggroRange)
        {
            // Oyuncunun duvar arkasýnda olup olmadýðýný kontrol et
            if (IsPlayerVisible())
            {
                AttackPlayer();
            }
        }
    }

    private bool IsPlayerVisible()
    {
        // Raycast ile duvar kontrolü yap
        Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, directionToPlayer, enemyData.aggroRange);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player"))
            {
                // Oyuncu taretin görüþ alanýnda
                Debug.Log("Player görüldü, saldýrýya geçiliyor.");
                return true;
            }
            else
            {
                Debug.Log("Oyuncu duvar arkasýnda veya baþka bir engel var.");
            }
        }
        return false;
    }

    private void AttackPlayer()
    {
        // Bekleme süresi kontrolü
        attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0f)
        {
            ShootFireball();
            attackCooldown = enemyData.waitingTime; // Saldýrý sonrasý bekleme süresi
        }
    }

    private void ShootFireball()
    {
        if (fireballPrefab == null || firePoint == null || fireballData == null)
        {
            Debug.LogError("Fireball Prefab, FirePoint, or FireballData is missing!");
            return;
        }

        // Ateþ topu oluþtur ve oyuncuya doðru yönlendir
        Vector2 direction = (player.transform.position - firePoint.position).normalized;
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        fireball.transform.right = direction;

        Fireball fireballScript = fireball.GetComponent<Fireball>();
        if (fireballScript != null)
        {
            fireballScript.bulletData = fireballData;
            fireballScript.owner = gameObject;
        }

        Debug.Log("Ateþ topu fýrlatýldý.");
    }
}
