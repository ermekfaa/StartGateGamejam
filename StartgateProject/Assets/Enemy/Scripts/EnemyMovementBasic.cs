using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyMovementBasic : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveDistance = 1f; // Dusman bir adimda hareket edecegi mesafe
    public float moveDuration = 0.1f; // Hareket suresi

    [Header("Environment Settings")]
    public Tilemap wallTilemap; // Duvarla Tilemap
    private HashSet<Vector3Int> wallPositions = new HashSet<Vector3Int>(); // Duvar pozisyonlar (grid taban)

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float moveTimer;
    private bool hasMoved;

    private void Awake()
    {
        CollectWallPositions(); // Duvar pozisyo
    }

    public void MoveRandomly()
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
            float t = Mathf.Sin((moveTimer / moveDuration) * Mathf.PI * 0.5f); // Ease-in-out
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

            // Hedef pozisyonda duvar yoksa pozisyonu kabul et
            if (!wallPositions.Contains(wallTilemap.WorldToCell(targetPosition)))
                return;
        }

        // Hicbir gecerli hedef bulunamazsa, yerinde kal
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
        // Eger oyuncu bulunamiyorsa algilama yapilamaz
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerTransform == null) return false;

        Vector3Int enemyGridPos = wallTilemap.WorldToCell(transform.position); // D grid pozisyonu
        Vector3Int playerGridPos = wallTilemap.WorldToCell(playerTransform.position); // Oyuncunun grid pozisyonu

        // iki nokta arasndaki grid pozisyonlarini al
        List<Vector3Int> gridPositionsBetween = GetGridPositionsBetween(enemyGridPos, playerGridPos);

        // Arada herhangi bir grid pozisyonunda duvar varsa alg iptal et
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
}
