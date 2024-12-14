using UnityEngine;

public class GollemBehaviour : MonoBehaviour
{
    [Header("Golem Settings")]
    public float lifeTime = 10f; // Golem'in sahnede kalma s�resi
    public float moveSpeed = 2f; // Hareket h�z�
    public float attackRange = 1.5f; // Sald�r� mesafesi
    public int damage = 10; // Sald�r� hasar�

    private Transform target; // Hedef olarak en yak�n d��man

    private void Start()
    {
        // Golem belirli bir s�re sonra yok olacak
        Destroy(gameObject, lifeTime);

        // En yak�n d��man� hedef al
        FindClosestEnemy();
    }

    private void Update()
    {
        if (target != null)
        {
            // Hedefe do�ru hareket
            MoveToTarget();

            // E�er hedef sald�r� mesafesindeyse sald�r
            if (Vector3.Distance(transform.position, target.position) <= attackRange)
            {
                AttackTarget();
            }
        }
        else
        {
            // E�er hedef yoksa yeni bir hedef ara
            FindClosestEnemy();
        }
    }

    private void MoveToTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private void AttackTarget()
    {
        // D��man yok ediliyor
        Debug.Log($"Golem {target.name} hedefine sald�r�yor!");
        Destroy(target.gameObject); // Hedefi yok et
        Destroy(gameObject); // Golem kendini de yok eder
    }

    private void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length == 0)
        {
            Debug.LogWarning("Hi� d��man bulunamad�!");
            target = null;
            return;
        }

        float shortestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            // E�er d��man �ld�yse (�rne�in, "Die" tag'ine sahip olduysa), hedefleme
            if (enemy.CompareTag("Die"))
                continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        if (closestEnemy != null)
        {
            Debug.Log($"Hedef bulundu: {closestEnemy.name}");
            target = closestEnemy;
        }
        else
        {
            Debug.LogWarning("Uygun hedef bulunamad�!");
            target = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Sald�r� mesafesini g�rselle�tirme
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
