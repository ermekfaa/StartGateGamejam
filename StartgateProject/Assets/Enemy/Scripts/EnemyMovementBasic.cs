using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyMovementBasic : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveDistance = 1f;
    public float moveDuration = 0.1f;

    [Header("Environment Settings")]
    public Tilemap wallTilemap;
    private HashSet<Vector3> wallPositions = new HashSet<Vector3>();

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float moveTimer;
    private bool hasMoved;

    private void Awake()
    {
        CollectWallPositions();
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

            if (!wallPositions.Contains(targetPosition))
                return;
        }

        targetPosition = startPosition;
    }

    private void CollectWallPositions()
    {
        BoundsInt bounds = wallTilemap.cellBounds;
        TileBase[] allTiles = wallTilemap.GetTilesBlock(bounds);

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                if (wallTilemap.HasTile(tilePosition))
                {
                    Vector3 worldPosition = wallTilemap.CellToWorld(tilePosition) + new Vector3(0.5f, 0.5f, 0);
                    wallPositions.Add(worldPosition);
                }
            }
        }
    }

    public void ResetMovement()
    {
        hasMoved = false;
    }
}
