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

    private GameObject player;         // Oyuncunun referans�
    private float attackCooldown;      // Sald�r� bekleme s�resi

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player object not found in the scene!");
        }

        attackCooldown = enemyData.waitingTime; // Bekleme s�resini ba�lat
    }

    private void Update()
    {
        // Oyuncuyu alg�la ve sald�r
        DetectAndAttackPlayer();
    }

    private void DetectAndAttackPlayer()
    {
        if (player == null) return;

        // Oyuncunun tarete olan mesafesini kontrol et
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= enemyData.aggroRange)
        {
            // Oyuncunun duvar arkas�nda olup olmad���n� kontrol et
            if (IsPlayerVisible())
            {
                AttackPlayer();
            }
        }
    }

    private bool IsPlayerVisible()
    {
        // Raycast ile duvar kontrol� yap
        Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, directionToPlayer, enemyData.aggroRange);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player"))
            {
                // Oyuncu taretin g�r�� alan�nda
                Debug.Log("Player g�r�ld�, sald�r�ya ge�iliyor.");
                return true;
            }
            else
            {
                Debug.Log("Oyuncu duvar arkas�nda veya ba�ka bir engel var.");
            }
        }
        return false;
    }

    private void AttackPlayer()
    {
        // Bekleme s�resi kontrol�
        attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0f)
        {
            ShootFireball();
            attackCooldown = enemyData.waitingTime; // Sald�r� sonras� bekleme s�resi
        }
    }

    private void ShootFireball()
    {
        if (fireballPrefab == null || firePoint == null || fireballData == null)
        {
            Debug.LogError("Fireball Prefab, FirePoint, or FireballData is missing!");
            return;
        }

        // Ate� topu olu�tur ve oyuncuya do�ru y�nlendir
        Vector2 direction = (player.transform.position - firePoint.position).normalized;
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        fireball.transform.right = direction;

        Fireball fireballScript = fireball.GetComponent<Fireball>();
        if (fireballScript != null)
        {
            fireballScript.bulletData = fireballData;
            fireballScript.owner = gameObject;
        }

        Debug.Log("Ate� topu f�rlat�ld�.");
    }
}
