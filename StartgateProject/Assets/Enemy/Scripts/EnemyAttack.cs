using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public enum EnemyState { CheckForPlayer, MoveRandomly, AttackPlayer }

    [Header("Enemy Data")]
    public EnemyData enemyData;

    [Header("Fireball Settings")]
    public GameObject fireballPrefab;
    public BulletData fireballData;
    public Transform firePoint;

    private EnemyState currentState = EnemyState.CheckForPlayer;
    private float stateTimer;
    private GameObject player;
    private EnemyMovementBasic movementComponent;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        movementComponent = GetComponent<EnemyMovementBasic>();

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
                movementComponent.MoveRandomly();
                if (stateTimer <= 0f)
                {
                    currentState = EnemyState.CheckForPlayer;
                    stateTimer = enemyData.waitingTime;
                    movementComponent.ResetMovement();
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

    private void CheckForPlayer()
    {
        if (player == null) return;

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
}
