using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyMovementMiddle : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveDistance = 1f; // D��man bir ad�mda hareket edece�i mesafe
    public float moveDuration = 0.1f; // Hareket s�resi

    [Header("Environment Settings")]
    public Tilemap wallTilemap; // Duvar Tilemap
    private HashSet<Vector3Int> wallPositions = new HashSet<Vector3Int>(); // Duvar pozisyonlar� (grid tabanl�)

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float moveTimer;
    private bool hasMoved;

    private Vector3? lastSeenPlayerPosition = null; // Oyuncunun en son g�r�ld��� pozisyon
    private bool isMovingToLastSeenPosition = false; // Son g�r�len pozisyona hareket ediyor mu?

    private void Awake()
    {
        CollectWallPositions();
    }

    public void MoveRandomly()
    {
        if (hasMoved || isMovingToLastSeenPosition) return;

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
            float t = Mathf.Sin((moveTimer / moveDuration) * Mathf.PI * 0.5f); // Ease-in-out
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        transform.position = targetPosition;

        // E�er oyuncunun son g�r�ld��� pozisyona hareket ediliyorsa, pozisyona ula�t�k m� kontrol et
        if (isMovingToLastSeenPosition && Vector3.Distance(transform.position, lastSeenPlayerPosition.Value) < 0.1f)
        {
            isMovingToLastSeenPosition = false;
            lastSeenPlayerPosition = null; // Hedefe ula��ld���nda s�f�rla
        }
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

            // Hedef pozisyonda duvar yoksa pozisyonu kabul et
            if (!wallPositions.Contains(wallTilemap.WorldToCell(targetPosition)))
                return;
        }

        // Hi�bir ge�erli hedef bulunamazsa, yerinde kal
        targetPosition = startPosition;
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

    public void ResetMovement()
    {
        hasMoved = false;
    }

    public bool PlayerDetected()
    {
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerTransform == null) return false;

        Vector3Int enemyGridPos = wallTilemap.WorldToCell(transform.position);
        Vector3Int playerGridPos = wallTilemap.WorldToCell(playerTransform.position);

        List<Vector3Int> gridPositionsBetween = GetGridPositionsBetween(enemyGridPos, playerGridPos);

        foreach (Vector3Int gridPosition in gridPositionsBetween)
        {
            if (wallPositions.Contains(gridPosition))
            {
                Debug.Log("Player ile Enemy aras�nda duvar var.");
                return false;
            }
        }

        Debug.Log("Player alg�land�!");
        lastSeenPlayerPosition = playerTransform.position; // Oyuncunun pozisyonunu kaydet
        return true;
    }

    public void MoveToLastSeenPosition()
    {
        if (lastSeenPlayerPosition == null || isMovingToLastSeenPosition) return;

        Vector3Int enemyGridPos = wallTilemap.WorldToCell(transform.position);
        Vector3Int lastSeenGridPos = wallTilemap.WorldToCell(lastSeenPlayerPosition.Value);

        if (enemyGridPos == lastSeenGridPos)
        {
            isMovingToLastSeenPosition = false;
            lastSeenPlayerPosition = null;
            return;
        }

        // Ad�m ad�m hareket
        Vector3 direction = (lastSeenPlayerPosition.Value - transform.position).normalized;
        targetPosition = transform.position + new Vector3(direction.x, direction.y, 0f) * moveDistance;

        if (!wallPositions.Contains(wallTilemap.WorldToCell(targetPosition)))
        {
            StartCoroutine(MoveCoroutine());
            isMovingToLastSeenPosition = true;
        }
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
}