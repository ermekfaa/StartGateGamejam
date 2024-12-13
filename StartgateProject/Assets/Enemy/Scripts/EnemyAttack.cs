using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyAttack : MonoBehaviour
{
    public enum EnemyState
    {
        CheckForPlayer,
        MoveRandomly,
        AttackPlayer
    }

    [Header("Enemy Data")]
    public EnemyData enemyData; // EnemyData ScriptableObject referansý

    [Header("Fireball Settings")]
    public GameObject fireballPrefab;  // Fireball prefab referansý
    public BulletData fireballData;   // Fireball için ScriptableObject referansý
    public Transform firePoint;       // Merminin çýkýþ noktasý

    [Header("Movement Settings")]
    public float moveDistance = 1f;   // Karakterin her hareket ettiðinde ilerleyeceði mesafe
    public float moveDuration = 0.1f; // Hareketin süresi (saniye cinsinden)

    [Header("Environment Settings")]
    public Tilemap wallTilemap; // Duvarlarýn bulunduðu Tilemap
    private HashSet<Vector3> wallPositions = new HashSet<Vector3>(); // Duvar pozisyonlarýný saklar

    private EnemyState currentState = EnemyState.CheckForPlayer;
    private float stateTimer;
    private GameObject player;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool hasMoved;
    private float moveTimer;

    private void Awake()
    {
        // Duvar pozisyonlarýný oyun baþýnda topla
        CollectWallPositions();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player object not found in the scene!");
        }

        stateTimer = enemyData.waitingTime; // Baþlangýç durumu için timer
    }

    private void Update()
    {
        stateTimer -= Time.deltaTime;

        switch (currentState)
        {
            case EnemyState.MoveRandomly:
                if (!hasMoved)
                {
                    MoveRandomly();
                    hasMoved = true;
                }
                if (stateTimer <= 0f)
                {
                    currentState = EnemyState.CheckForPlayer;
                    stateTimer = enemyData.waitingTime;
                    hasMoved = false; // Sonraki döngü için hareketi sýfýrla
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
        SetRandomTargetPosition();
        moveTimer = 0f;

        // Hareketi baþlat
        StartCoroutine(MoveCoroutine());
    }

    private IEnumerator MoveCoroutine()
    {
        startPosition = transform.position;
        moveTimer = 0f;

        while (moveTimer < moveDuration)
        {
            moveTimer += Time.deltaTime;
            float t = Mathf.Sin((moveTimer / moveDuration) * Mathf.PI * 0.5f); // Sinusoidal hareket
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        transform.position = targetPosition; // Hedef pozisyona ulaþ
    }

    private void SetRandomTargetPosition()
    {
        startPosition = transform.position;

        // Rastgele bir yön seç (yukarý, aþaðý, sað, sol)
        Vector2 direction = Vector2.zero;
        for (int i = 0; i < 10; i++) // Maksimum 10 deneme yap
        {
            switch (Random.Range(0, 4))
            {
                case 0:
                    direction = Vector2.up;
                    break;
                case 1:
                    direction = Vector2.down;
                    break;
                case 2:
                    direction = Vector2.left;
                    break;
                case 3:
                    direction = Vector2.right;
                    break;
            }

            targetPosition = startPosition + new Vector3(direction.x, direction.y, 0f) * moveDistance;

            // Eðer hedef pozisyon bir duvar deðilse, hareket et
            if (!wallPositions.Contains(targetPosition))
            {
                return; // Geçerli pozisyon bulundu
            }
        }

        // Eðer geçerli bir pozisyon bulunamazsa hedef pozisyonu deðiþtirme
        targetPosition = startPosition;
    }

    private void CheckForPlayer()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= enemyData.aggroRange)
        {
            currentState = EnemyState.AttackPlayer;
            stateTimer = 0f; // Beklemeden saldýrý durumuna geç
        }
        else
        {
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

    private void CollectWallPositions()
    {
        // Tilemap'teki tüm aktif hücreleri kontrol et ve koordinatlarý kaydet
        BoundsInt bounds = wallTilemap.cellBounds;
        TileBase[] allTiles = wallTilemap.GetTilesBlock(bounds);

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                if (wallTilemap.HasTile(tilePosition))
                {
                    Vector3 worldPosition = wallTilemap.CellToWorld(tilePosition) + new Vector3(0.5f, 0.5f, 0); // Tile'larýn merkezine hizala
                    wallPositions.Add(worldPosition);

                    // Duvar pozisyonlarýný yazdýr
                    Debug.Log($"Enemy için Duvar Pozisyonu: {worldPosition}");
                }
            }
        }

        Debug.Log($"Enemy için toplam {wallPositions.Count} duvar pozisyonu kaydedildi.");
    }
}
