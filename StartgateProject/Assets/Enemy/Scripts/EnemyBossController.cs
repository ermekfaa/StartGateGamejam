using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class EnemyBossController : MonoBehaviour
{
    public enum BossState { Idle, Chase, AttackWithSword, FireElectricBall }

    [Header("Health Settings")]
    public float maxHealth = 300f; // Boss'un maksimum sağlığı
    private float currentHealth; // Boss'un mevcut sağlığı
    public Slider healthBar; // Sağlık barı (UI Slider)

    [Header("Movement Settings")]
    public float tileSize = 1.5f;
    public float moveCooldown = 0.1f;
    public float moveSmoothness = 1f;
    public Tilemap wallTilemap;

    [Header("Shooting Settings")]
    public GameObject electricBallPrefab;
    public Transform firePoint;
    public float shootingCooldown = 2f;

    [Header("Sword Attack Settings")]
    public Transform swordTransform;
    public float swordSwingBackAngle = -45f;
    public float swordStabDuration = 0.3f;
    public float swordAttackRange = 1.5f;
    public LayerMask playerLayer;
    public Transform attackPoint;

    private GameObject player;
    private BossState currentState = BossState.Idle;
    private float stateTimer;
    private float moveTimer;

    private HashSet<Vector3Int> wallPositions = new HashSet<Vector3Int>();

    private Vector3 targetPosition;
    private bool isMoving;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player object not found in the scene!");
        }

        CollectWallPositions();
        targetPosition = transform.position;

        // Sağlık yönetimi
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    private void Update()
    {
        if (player == null) return;

        stateTimer -= Time.deltaTime;
        moveTimer -= Time.deltaTime;

        if (isMoving)
        {
            SmoothMoveToTarget();
            return;
        }

        // Test için sağlık düşüşü
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(10f);
            Debug.Log($"Damage Verildi: {currentHealth}");
        }

        switch (currentState)
        {
            case BossState.Idle:
                if (stateTimer <= 0f)
                {
                    currentState = BossState.Chase;
                    stateTimer = moveCooldown;
                }
                break;

            case BossState.Chase:
                if (Vector3.Distance(transform.position, player.transform.position) <= swordAttackRange + 0.5f)
                {
                    currentState = BossState.AttackWithSword;
                }
                else if (moveTimer <= 0f)
                {
                    MoveTowardsPlayer();
                    moveTimer = moveCooldown;
                }
                if (stateTimer <= 0f)
                {
                    currentState = BossState.FireElectricBall;
                    stateTimer = shootingCooldown;
                }
                break;

            case BossState.AttackWithSword:
                StartCoroutine(SwordAttack());
                currentState = BossState.Idle;
                stateTimer = moveCooldown;
                break;

            case BossState.FireElectricBall:
                ShootElectricBall();
                currentState = BossState.Idle;
                stateTimer = shootingCooldown;
                break;
        }
    }

    private void SmoothMoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSmoothness * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            isMoving = false;
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector3Int currentGridPosition = wallTilemap.WorldToCell(transform.position);
        Vector3Int targetGridPosition = wallTilemap.WorldToCell(player.transform.position);

        Vector3Int moveDirection = Vector3Int.zero;

        if (Mathf.Abs(targetGridPosition.x - currentGridPosition.x) > Mathf.Abs(targetGridPosition.y - currentGridPosition.y))
        {
            moveDirection.x = targetGridPosition.x > currentGridPosition.x ? 1 : -1;
        }
        else
        {
            moveDirection.y = targetGridPosition.y > currentGridPosition.y ? 1 : -1;
        }

        Vector3Int newGridPosition = currentGridPosition + moveDirection;

        if (!wallPositions.Contains(newGridPosition) && Vector3.Distance(transform.position, player.transform.position) > swordAttackRange + 0.5f)
        {
            targetPosition = wallTilemap.CellToWorld(newGridPosition) + new Vector3(tileSize / 2f, tileSize / 2f, 0);
            isMoving = true;
        }

        RotateTowardsPlayer();
    }

    private void RotateTowardsPlayer()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void ShootElectricBall()
    {
        if (electricBallPrefab == null || firePoint == null) return;

        GameObject electricBall = Instantiate(electricBallPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = electricBall.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 direction = (player.transform.position - firePoint.position).normalized;
            rb.linearVelocity = direction * 5f;
        }
    }

    private IEnumerator SwordAttack()
    {
        float elapsedTime = 0f;
        Quaternion initialRotation = swordTransform.localRotation;
        Quaternion swingBackRotation = Quaternion.Euler(0, 0, swordSwingBackAngle);

        while (elapsedTime < swordStabDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            swordTransform.localRotation = Quaternion.Lerp(initialRotation, swingBackRotation, elapsedTime / (swordStabDuration / 2));
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < swordStabDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            swordTransform.localRotation = Quaternion.Lerp(swingBackRotation, initialRotation, elapsedTime / (swordStabDuration / 2));
            yield return null;
        }

        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, swordAttackRange, playerLayer);
        foreach (Collider2D hit in hitPlayers)
        {
            if (hit.CompareTag("Player")) Debug.Log("Player hit by sword!");
        }
    }

    private void CollectWallPositions()
    {
        BoundsInt bounds = wallTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                if (wallTilemap.HasTile(tilePosition)) wallPositions.Add(tilePosition);
            }
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            float normalizedHealth = currentHealth / maxHealth;
            healthBar.value = normalizedHealth;
            Debug.Log("Health bar güncellendi. Normalized Health: " + normalizedHealth);
        }
        else
        {
            Debug.LogError("HealthBar referansı eksik!");
        }
    }


    private bool isDead = false; // Ölü olup olmadığını kontrol eder

    public void TakeDamage(float damage)
    {
        if (isDead) return; // Eğer Boss zaten ölmüşse, bir daha çalışmasın

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log("Boss health değeri: " + currentHealth);

        UpdateHealthBar();

        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        if (isDead) return; // Birden fazla kez çalışmasını önle
        isDead = true;

        Debug.Log("Boss öldü!");
        Destroy(gameObject); // Boss'u yok et
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Boss"))
        {
            EnemyBossController boss = collision.GetComponent<EnemyBossController>();
            if (boss != null)
            {
                boss.TakeDamage(50f); // Örnek hasar değeri
            }

            Destroy(gameObject); // Fireball'u yok et
        }
    }

}
