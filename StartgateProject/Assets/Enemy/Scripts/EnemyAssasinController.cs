using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyAssassinController : MonoBehaviour
{
    public enum EnemyState { CheckForPlayer, MoveTowardsPlayer, AttackPlayer }

    [Header("Movement Settings")]
    public float moveDistance = 1f;
    public float moveDuration = 0.1f;

    [Header("Environment Settings")]
    public Tilemap wallTilemap;
    private HashSet<Vector3Int> wallPositions = new HashSet<Vector3Int>();

    [Header("Enemy Data")]
    public EnemyData enemyData;

    [Header("Weapon Settings")]
    public GameObject swordPrefab;
    public Transform attackPoint;
    public float attackRange = 1.5f;
    public LayerMask playerLayer;
    public Transform swordTransform; // Sword object transform
    public float swordSwingBackAngle = -45f; // Angle to swing back the sword
    public float swordStabDuration = 0.2f; // Time it takes to stab

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

        RotateTowardsPlayer();

        switch (currentState)
        {
            case EnemyState.MoveTowardsPlayer:
                MoveTowardsPlayer();
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

    private void MoveTowardsPlayer()
    {
        if (hasMoved) return;

        Vector3Int startGrid = wallTilemap.WorldToCell(transform.position);
        Vector3Int endGrid = wallTilemap.WorldToCell(player.transform.position);

        List<Vector3Int> path = FindPath(startGrid, endGrid);
        if (path.Count > 1)
        {
            targetPosition = wallTilemap.CellToWorld(path[1]) + new Vector3(0.5f, 0.5f, 0);
            StartCoroutine(MoveCoroutine());
            hasMoved = true;
        }
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

    private void RotateTowardsPlayer()
    {
        if (player == null) return;

        Vector3 direction = (player.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void CheckForPlayer()
    {
        if (player == null) return;

        if (PlayerDetected())
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            if (distanceToPlayer <= attackRange)
            {
                currentState = EnemyState.AttackPlayer;
                stateTimer = 0f;
            }
            else
            {
                currentState = EnemyState.MoveTowardsPlayer;
                stateTimer = enemyData.waitingTime;
            }
        }
        else
        {
            Debug.Log("Player is not detected.");
            currentState = EnemyState.CheckForPlayer;
            stateTimer = enemyData.waitingTime;
        }
    }

    private void AttackPlayer()
    {
        if (player == null || !PlayerDetected())
        {
            currentState = EnemyState.CheckForPlayer;
            stateTimer = enemyData.waitingTime;
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (distanceToPlayer > attackRange)
        {
            currentState = EnemyState.CheckForPlayer;
            stateTimer = enemyData.waitingTime;
            return;
        }

        if (stateTimer <= 0f)
        {
            StartCoroutine(SwordStabAnimation());
            stateTimer = enemyData.waitingTime;
        }
    }

    private IEnumerator SwordStabAnimation()
    {
        Debug.Log("Sword stab initiated.");

        // Swing the sword back
        float elapsedTime = 0f;
        Quaternion initialRotation = swordTransform.localRotation;
        Quaternion swingBackRotation = Quaternion.Euler(0, 0, swordSwingBackAngle);

        while (elapsedTime < swordStabDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            swordTransform.localRotation = Quaternion.Lerp(initialRotation, swingBackRotation, elapsedTime / (swordStabDuration / 2));
            yield return null;
        }

        // Stab the sword forward
        elapsedTime = 0f;
        while (elapsedTime < swordStabDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            swordTransform.localRotation = Quaternion.Lerp(swingBackRotation, initialRotation, elapsedTime / (swordStabDuration / 2));
            yield return null;
        }

        Debug.Log("Sword stab completed.");

        // Check for player damage
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);

        foreach (Collider2D hit in hitPlayers)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("Player hit by sword!");
                // Apply damage to player
            }
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
                Debug.Log("Player is behind a wall.");
                return false;
            }
        }

        Debug.Log("Player detected!");
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

    private List<Vector3Int> FindPath(Vector3Int start, Vector3Int end)
    {
        // A* Pathfinding implementation
        // Use a simple priority queue and heuristic to calculate path

        HashSet<Vector3Int> closedSet = new HashSet<Vector3Int>();
        SimplePriorityQueue<Vector3Int, float> openSet = new SimplePriorityQueue<Vector3Int, float>();
        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        Dictionary<Vector3Int, float> gScore = new Dictionary<Vector3Int, float>();
        Dictionary<Vector3Int, float> fScore = new Dictionary<Vector3Int, float>();

        openSet.Enqueue(start, 0);
        gScore[start] = 0;
        fScore[start] = Heuristic(start, end);

        while (openSet.Count > 0)
        {
            Vector3Int current = openSet.Dequeue();

            if (current == end)
            {
                return ReconstructPath(cameFrom, current);
            }

            closedSet.Add(current);

            foreach (Vector3Int neighbor in GetNeighbors(current))
            {
                if (closedSet.Contains(neighbor) || wallPositions.Contains(neighbor))
                    continue;

                float tentativeGScore = gScore[current] + 1;

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, end);

                    if (!openSet.Contains(neighbor))
                        openSet.Enqueue(neighbor, fScore[neighbor]);
                }
            }
        }

        return new List<Vector3Int>();
    }

    private float Heuristic(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private List<Vector3Int> GetNeighbors(Vector3Int cell)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>
        {
            cell + Vector3Int.up,
            cell + Vector3Int.down,
            cell + Vector3Int.left,
            cell + Vector3Int.right
        };
        return neighbors;
    }

    private List<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int current)
    {
        List<Vector3Int> path = new List<Vector3Int> { current };

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }

        return path;
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

        Debug.Log($"Collected {wallPositions.Count} wall positions.");
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}

public class SimplePriorityQueue<TElement, TPriority>
{
    private List<(TElement Element, TPriority Priority)> elements = new List<(TElement, TPriority)>();

    public int Count => elements.Count;

    public void Enqueue(TElement element, TPriority priority)
    {
        elements.Add((element, priority));
        elements.Sort((x, y) => Comparer<TPriority>.Default.Compare(x.Priority, y.Priority));
    }

    public TElement Dequeue()
    {
        if (elements.Count == 0)
        {
            throw new System.InvalidOperationException("The queue is empty.");
        }

        var element = elements[0];
        elements.RemoveAt(0);
        return element.Element;
    }

    public bool Contains(TElement element)
    {
        return elements.Exists(e => EqualityComparer<TElement>.Default.Equals(e.Element, element));
    }
}