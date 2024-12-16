using UnityEngine;

public class GollemBehaviour : MonoBehaviour
{
    [Header("Golem Settings")]
    public float lifeTime = 10f; // Golem'in sahnede kalma süresi
    public float moveSpeed = 2f; // Hareket hýzý
    public float attackRange = 1.5f; // Saldýrý mesafesi
    public int damage = 10; // Saldýrý hasarý

    private Transform target; // Hedef olarak en yakýn düþman

    private void Start()
    {
        // Golem belirli bir süre sonra yok olacak
        Destroy(gameObject, lifeTime);

        // En yakýn düþmaný hedef al
        FindClosestEnemy();
    }

    private void Update()
    {
        if (target != null)
        {
            // Hedefe doðru hareket
            MoveToTarget();

            // Eðer hedef saldýrý mesafesindeyse saldýr
            if (Vector3.Distance(transform.position, target.position) <= attackRange)
            {
                AttackTarget();
            }
        }
        else
        {
            // Eðer hedef yoksa yeni bir hedef ara
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
        // Hedefte Health script'i var mý kontrol et
        EnemyHealth targetHealth = target.GetComponent<EnemyHealth>();

        if (targetHealth != null)
        {
            int damage = 20; // Golem'in vereceði hasar
            Debug.Log($"Golem {target.name} hedefine {damage} hasar veriyor!");
            targetHealth.TakeDamage(damage); // Hasar uygula
        }
        else
        {
            Debug.LogWarning($"Hedef {target.name} hasar alabilecek bir Health script'ine sahip deðil!");
        }

        // (Opsiyonel) Golem kendini yok eder
        Destroy(gameObject);
    }

    private void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] bosses = GameObject.FindGameObjectsWithTag("Boss");

        if (enemies.Length == 0 && bosses.Length == 0)
        {
            Debug.LogWarning("Hiç düþman bulunamadý!");
            target = null;
            return;
        }

        float shortestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            // Eðer düþman öldüyse (örneðin, "Die" tag'ine sahip olduysa), hedefleme
            if (enemy.CompareTag("Die"))
                continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }
        /*
        if (closestEnemy != null)
        {
            Debug.Log($"Hedef bulundu: {closestEnemy.name}");
            target = closestEnemy;
        }
        else
        {
            Debug.LogWarning("Uygun hedef bulunamadý!");
            target = null;
        }
        */



        //BOSS KISMI
        foreach (GameObject boss in bosses)
        {
            // Eðer düþman öldüyse (örneðin, "Die" tag'ine sahip olduysa), hedefleme
            if (boss.CompareTag("Die"))
                continue;

            float distance = Vector3.Distance(transform.position, boss.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestEnemy = boss.transform;
            }
        }

        if (closestEnemy != null)
        {
            Debug.Log($"Hedef bulundu: {closestEnemy.name}");
            target = closestEnemy;
        }
        else
        {
            Debug.LogWarning("Uygun hedef bulunamadý!");
            target = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Saldýrý mesafesini görselleþtirme
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
