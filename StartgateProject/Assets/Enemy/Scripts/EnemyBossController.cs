using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Collections;

public class EnemyBossController : MonoBehaviour
{
    // Health variables
    public int maxHealth = 100;
    private int currentHealth;

    // Movement variables
    public float stepDuration = 0.3f;
    public float teleportDuration = 0.5f; // Duration for smooth teleport

    // Attack variables
    public GameObject electricShockPrefab;
    public GameObject swordPrefab;
    public Transform attackPoint;
    public float shockRange = 5f;
    public float shockCooldown = 3f;
    private float lastShockTime;
    public float swordAttackRange = 1.5f;
    public float swordAttackCooldown = 2f;
    private float lastSwordAttackTime;

    public Transform player;

    // Animator
    private Animator animator;

    // Wall mechanic variables
    public Tilemap wallTilemap;
    private HashSet<Vector3Int> wallPositions = new HashSet<Vector3Int>();

    private bool isMoving = false;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        CollectWallPositions();
    }

    void Update()
    {
        if (player == null || isMoving)
            return;

        MoveStepTowardsPlayer();
        HandleAttacks();
    }

    private void MoveStepTowardsPlayer()
    {
        Vector3Int currentGridPosition = wallTilemap.WorldToCell(transform.position);
        Vector3Int targetGridPosition = wallTilemap.WorldToCell(player.position);

        Vector3Int nextStep = GetNextStep(currentGridPosition, targetGridPosition);

        if (nextStep != currentGridPosition && !wallPositions.Contains(nextStep))
        {
            Vector3 nextWorldPosition = wallTilemap.CellToWorld(nextStep) + new Vector3(0.5f, 0.5f, 0f);
            StartCoroutine(MoveToPosition(nextWorldPosition));
        }
    }

    private Vector3Int GetNextStep(Vector3Int current, Vector3Int target)
    {
        List<Vector3Int> possibleSteps = new List<Vector3Int>
        {
            current + Vector3Int.up,
            current + Vector3Int.down,
            current + Vector3Int.left,
            current + Vector3Int.right
        };

        Vector3Int bestStep = current;
        float shortestDistance = float.MaxValue;

        foreach (var step in possibleSteps)
        {
            if (!wallPositions.Contains(step))
            {
                float distance = Vector3Int.Distance(step, target);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    bestStep = step;
                }
            }
        }

        return bestStep;
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        isMoving = true;
        Vector3 startPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < stepDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Sin((elapsed / stepDuration) * Mathf.PI * 0.5f); // Smoother movement with sine curve
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;
    }

    private void HandleAttacks()
    {
        if (Time.time > lastShockTime + shockCooldown)
        {
            ElectricShockAttack();
        }

        if (Time.time > lastSwordAttackTime + swordAttackCooldown)
        {
            SwordAttack();
        }
    }

    public void TeleportTo(Vector3Int gridPosition)
    {
        Vector3 targetPosition = wallTilemap.CellToWorld(gridPosition) + new Vector3(0.5f, 0.5f, 0f);
        StartCoroutine(SmoothTeleport(targetPosition));
    }

    private IEnumerator SmoothTeleport(Vector3 targetPosition)
    {
        isMoving = true;
        Vector3 startPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < teleportDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Sin((elapsed / teleportDuration) * Mathf.PI * 0.5f); // Smoother teleport with sine curve
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;
    }

    private void ElectricShockAttack()
    {
        lastShockTime = Time.time;
        animator.SetTrigger("ElectricShock");

        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, shockRange);

        foreach (Collider2D obj in hitObjects)
        {
            if (obj.CompareTag("Player"))
            {
                Debug.Log("Player hit by electric shock!");
                // Apply damage or effects to player here
            }
        }
    }

    private void SwordAttack()
    {
        lastSwordAttackTime = Time.time;
        animator.SetTrigger("SwordAttack");

        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, swordAttackRange);

        foreach (Collider2D obj in hitObjects)
        {
            if (obj.CompareTag("Player"))
            {
                Debug.Log("Player hit by sword attack!");
                // Apply damage or effects to player here
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Boss defeated!");
        // Play death animation or effects
        Destroy(gameObject);
    }

    private void CollectWallPositions()
    {
        BoundsInt bounds = wallTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                if (wallTilemap.HasTile(tilePosition))
                {
                    wallPositions.Add(tilePosition);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the shock range and sword attack range in the editor
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, shockRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, swordAttackRange);
    }
}
