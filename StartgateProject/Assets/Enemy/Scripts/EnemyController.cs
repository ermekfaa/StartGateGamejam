using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyController : MonoBehaviour
{
    public enum EnemyState { CheckForPlayer, MoveRandomly, AttackPlayer }

    [Header("Movement Settings")]
    public float moveDistance = 1f;
    public float moveDuration = 0.1f;

    [Header("Environment Settings")]
    public Tilemap wallTilemap;
    private HashSet<Vector3Int> wallPositions = new HashSet<Vector3Int>();

    [Header("Enemy Data")]
    public EnemyData enemyData;

    [Header("Fireball Settings")]
    public GameObject fireballPrefab;
    public BulletData fireballData;
    public Transform firePoint;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float moveTimer;
    private bool hasMoved;

    private EnemyState currentState = EnemyState.CheckForPlayer;
    private float stateTimer;
    private GameObject player;

    private void Awake()
    {
        CollectWallPositions();
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player object not found in the scene!");
        }
        stateTimer = enemyData.waitingTime;
    }

    private void Update()
    {
        stateTimer -= Time.deltaTime;

        switch (currentState)
        {
            case EnemyState.MoveRandomly:
                MoveRandomly();
                if (stateTimer <= 0f)
                {
                    currentState = EnemyState.CheckForPlayer;
                    stateTimer = enemyData.waitingTime;
                    hasMoved = false;
                }
                break;

            case EnemyState.CheckForPlayer:
                CheckForPlayer();
                break;

            case EnemyState.AttackPlayer:
                AttackPlayer();
                break;
        }
    }

    private void MoveRandomly()
    {
        if (hasMoved) return;

        SetRandomTargetPosition();
        StartCoroutine(MoveCoroutine());
        hasMoved = true;
    }

    private IEnumerator MoveCoroutine()
    {
        startPosition = transform.position;
        moveTimer = 0f;

        while (moveTimer < moveDuration)
        {
            moveTimer += Time.deltaTime;
            float t = Mathf.Sin((moveTimer / moveDuration) * Mathf.PI * 0.5f);
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        transform.position = targetPosition;
    }

    private void SetRandomTargetPosition()
    {
        startPosition = transform.position;

        for (int i = 0; i < 10; i++)
        {
            Vector2 direction = Vector2.zero;
            switch (Random.Range(0, 4))
            {
                case 0: direction = Vector2.up; break;
                case 1: direction = Vector2.down; break;
                case 2: direction = Vector2.left; break;
                case 3: direction = Vector2.right; break;
            }

            targetPosition = startPosition + new Vector3(direction.x, direction.y, 0f) * moveDistance;

            if (!wallPositions.Contains(wallTilemap.WorldToCell(targetPosition)))
                return;
        }

        targetPosition = startPosition;
    }

    private void CheckForPlayer()
    {
        if (player == null) return;

        if (PlayerDetected())
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            if (distanceToPlayer <= enemyData.aggroRange)
            {
                currentState = EnemyState.AttackPlayer;
                stateTimer = 0f;
            }
            else
            {
                currentState = EnemyState.MoveRandomly;
                stateTimer = enemyData.waitingTime;
            }
        }
        else
        {
            Debug.Log("Player duvarin arkasi algilanamadi.");
            currentState = EnemyState.MoveRandomly;
            stateTimer = enemyData.waitingTime;
        }
    }

    private void AttackPlayer()
    {
        if (player == null)
        {
            currentState = EnemyState.CheckForPlayer;
            return;
        }

        if (!PlayerDetected())
        {
            Debug.Log("Player artik gorunmuyor, CheckForPlayer'a donuluyor.");
            currentState = EnemyState.CheckForPlayer;
            stateTimer = enemyData.waitingTime;
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (distanceToPlayer > enemyData.aggroRange)
        {
            currentState = EnemyState.CheckForPlayer;
            stateTimer = enemyData.waitingTime;
            return;
        }

        if (stateTimer <= 0f)
        {
            ShootFireball();
            stateTimer = enemyData.waitingTime;
        }
    }

    private void ShootFireball()
    {
        if (fireballPrefab == null || firePoint == null || fireballData == null)
        {
            Debug.LogError("Fireball Prefab, FirePoint, or FireballData is missing!");
            return;
        }

        Vector2 direction = (player.transform.position - firePoint.position).normalized;
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        fireball.transform.right = direction;

        Fireball fireballScript = fireball.GetComponent<Fireball>();
        if (fireballScript != null)
        {
            fireballScript.bulletData = fireballData;
            fireballScript.owner = gameObject;
        }
    }

    private bool PlayerDetected()
    {
        if (player == null) return false;

        Vector3Int enemyGridPos = wallTilemap.WorldToCell(transform.position);
        Vector3Int playerGridPos = wallTilemap.WorldToCell(player.transform.position);

        List<Vector3Int> gridPositionsBetween = GetGridPositionsBetween(enemyGridPos, playerGridPos);

        foreach (Vector3Int gridPosition in gridPositionsBetween)
        {
            if (wallPositions.Contains(gridPosition))
            {
                Debug.Log("Player ile Enemy arasinda duvar var.");
                return false;
            }
        }

        Debug.Log("Player algilandi!");
        return true;
    }

    private List<Vector3Int> GetGridPositionsBetween(Vector3Int start, Vector3Int end)
    {
        List<Vector3Int> positions = new List<Vector3Int>();

        int xDiff = Mathf.Abs(end.x - start.x);
        int yDiff = Mathf.Abs(end.y - start.y);

        int xStep = start.x < end.x ? 1 : -1;
        int yStep = start.y < end.y ? 1 : -1;

        int error = xDiff - yDiff;

        while (true)
        {
            positions.Add(start);

            if (start == end) break;

            int e2 = error * 2;

            if (e2 > -yDiff)
            {
                error -= yDiff;
                start.x += xStep;
            }

            if (e2 < xDiff)
            {
                error += xDiff;
                start.y += yStep;
            }
        }

        return positions;
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

        Debug.Log($"Toplam duvar pozisyonu: {wallPositions.Count}");
    }
}
