using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyMovementMiddle : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveDistance = 1f;
    public float moveDuration = 0.5f; // Hareket s�resi

    [Header("Detection Settings")]
    public float detectionRange = 5f; // Oyuncuyu alg�layabilece�i mesafe
    public Transform player; // Oyuncunun referans�

    [Header("Environment Settings")]
    public Tilemap wallTilemap;
    private HashSet<Vector3> wallPositions = new HashSet<Vector3>();

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Vector3 lastSeenPlayerPosition; // Oyuncunun son g�r�ld��� pozisyon
    private float moveTimer; // Hareket zamanlay�c�s�
    private bool hasMoved;
    private bool playerDetected;
    private bool movingToLastSeen; // Son g�r�len pozisyona m� gidiyor

    private EnemyAttack enemyAttack;

    private void Awake()
    {
        CollectWallPositions();
        enemyAttack = GetComponent<EnemyAttack>();
    }

    private void Update()
    {
        DetectPlayer();

        if (playerDetected)
        {
            // Oyuncuyu g�r�yorsa sald�r
            enemyAttack.CurrentState = EnemyAttack.EnemyState.AttackPlayer;
            hasMoved = false; // Hareketi durdur
        }
        else if (movingToLastSeen)
        {
            // Son g�r�len pozisyona ilerle
            MoveToLastSeenPosition();
        }
        else
        {
            // Rastgele hareket
            enemyAttack.CurrentState = EnemyAttack.EnemyState.MoveRandomly;
            MoveRandomly();
        }
    }

    private void DetectPlayer()
    {
        if (player == null) return;

        playerDetected = Vector3.Distance(transform.position, player.position) <= detectionRange;
        if (playerDetected)
        {
            lastSeenPlayerPosition = player.position; // Son g�r�len pozisyonu g�ncelle
            movingToLastSeen = false; // Son g�r�len pozisyona gitmeye gerek yok
        }
        else if (!movingToLastSeen)
        {
            movingToLastSeen = true; // Oyuncuyu kaybetti�inde son g�r�len pozisyona git
        }
    }

    private void MoveToLastSeenPosition()
    {
        if (hasMoved) return;

        // E�er son g�r�len pozisyona ula��ld�ysa
        if (transform.position == lastSeenPlayerPosition)
        {
            movingToLastSeen = false;
            ResetMovement();
            return;
        }

        // Son g�r�len pozisyona do�ru hareket
        SetDirectionalTargetPosition(lastSeenPlayerPosition);
        StartCoroutine(MoveCoroutine());
        hasMoved = true;
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
        moveTimer = 0f; // Timer s�f�rland�

        while (moveTimer < moveDuration)
        {
            moveTimer += Time.deltaTime;
            float t = Mathf.Sin((moveTimer / moveDuration) * Mathf.PI * 0.5f);
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        transform.position = targetPosition;

        // Duvar kontrol�
        if (wallPositions.Contains(transform.position))
        {
            transform.position = startPosition; // Ge�ersiz pozisyonda ise geri d�ner
        }

        hasMoved = false;
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

        targetPosition = startPosition; // Ge�erli bir pozisyon bulunamazsa hareket etmez
    }

    private void SetDirectionalTargetPosition(Vector3 destination)
    {
        startPosition = transform.position;

        Vector3 direction = Vector3.zero;

        // Yatay ve dikey hareketi kontrol eder
        if (Mathf.Abs(destination.x - transform.position.x) > Mathf.Abs(destination.y - transform.position.y))
        {
            direction = new Vector3(Mathf.Sign(destination.x - transform.position.x), 0, 0); // Sa�a veya sola hareket
        }
        else
        {
            direction = new Vector3(0, Mathf.Sign(destination.y - transform.position.y), 0); // Yukar� veya a�a�� hareket
        }

        targetPosition = startPosition + direction * moveDistance;

        // Duvar kontrol�
        if (wallPositions.Contains(targetPosition))
        {
            targetPosition = startPosition; // Duvar varsa hareket etme
        }
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
