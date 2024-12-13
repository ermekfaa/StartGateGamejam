using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public enum EnemyState
    {
        MoveRandomly,
        CheckForPlayer,
        AttackPlayer
    }

    [Header("Enemy Data")]
    public EnemyData enemyData; // EnemyData ScriptableObject referans�

    [Header("Fireball Settings")]
    public GameObject fireballPrefab;  // Fireball prefab referans�
    public BulletData fireballData;   // Fireball i�in ScriptableObject referans�
    public Transform firePoint;       // Merminin ��k�� noktas�

    [Header("Movement Settings")]
    public float moveDistance = 1f; // Karakterin her hareket etti�inde ilerleyece�i mesafe
    public float moveDuration = 0.1f; // Hareketin s�resi (saniye cinsinden)

    private EnemyState currentState = EnemyState.MoveRandomly;
    private float stateTimer;
    private GameObject player;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool hasMoved;
    private float moveTimer;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player object not found in the scene!");
        }

        stateTimer = enemyData.waitingTime; // Ba�lang�� durumu i�in timer
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
                    hasMoved = false; // Sonraki d�ng� i�in hareketi s�f�rla
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

        // Hareketi ba�lat
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

        transform.position = targetPosition; // Hedef pozisyona ula�
    }

    private void SetRandomTargetPosition()
    {
        startPosition = transform.position;

        // Rastgele bir y�n se� (yukar�, a�a��, sa�, sol)
        Vector2 direction = Vector2.zero;
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
    }

    private void CheckForPlayer()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= enemyData.aggroRange)
        {
            currentState = EnemyState.AttackPlayer;
            stateTimer = 0f; // Beklemeden sald�r� durumuna ge�
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
            currentState = EnemyState.MoveRandomly;
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (distanceToPlayer > enemyData.aggroRange)
        {
            currentState = EnemyState.MoveRandomly;
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
}
