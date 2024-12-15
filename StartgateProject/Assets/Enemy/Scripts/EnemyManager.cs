using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject gate1; // Gate referansý
    public GameObject gate2; // Gate referansý

    void Start()
    {
        // Kapýlarý baþlangýçta gizle
        if (gate1 != null)
        {
            gate1.SetActive(false);
        }
        if (gate2 != null)
        {
            gate2.SetActive(false);
        }

        // Ýlk düþman kontrolünü yap
        CheckEnemies();
    }

    private void Update()
    {
        // Sürekli düþman sayýsýný kontrol et
        CheckEnemies();
    }

    private void CheckEnemies()
    {
        // Dinamik olarak düþman sayýsýný kontrol et
        int enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

        Debug.Log("Kalan düþman: " + enemyCount);

        if (enemyCount <= 0)
        {
            OpenPortals();
        }
    }
    public void EnemyDefeated()
    {
        //enemyCount--;
        Debug.Log("Kalan düþman: "  );

        //
    }

    private void OpenPortals()
    {
        Debug.Log("Portal açýldý!");

        if (gate1 != null)
        {
            gate1.SetActive(true); // Gate'i aktif et
        }
        if (gate2 != null)
        {
            gate2.SetActive(true); // Gate'i aktif et
        }
    }
}
