using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject gate1; // Gate referans�
    public GameObject gate2; // Gate referans�

    void Start()
    {
        // Kap�lar� ba�lang��ta gizle
        if (gate1 != null)
        {
            gate1.SetActive(false);
        }
        if (gate2 != null)
        {
            gate2.SetActive(false);
        }

        // �lk d��man kontrol�n� yap
        CheckEnemies();
    }

    private void Update()
    {
        // S�rekli d��man say�s�n� kontrol et
        CheckEnemies();
    }

    private void CheckEnemies()
    {
        // Dinamik olarak d��man say�s�n� kontrol et
        int enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

        Debug.Log("Kalan d��man: " + enemyCount);

        if (enemyCount <= 0)
        {
            OpenPortals();
        }
    }
    public void EnemyDefeated()
    {
        //enemyCount--;
        Debug.Log("Kalan d��man: "  );

        //
    }

    private void OpenPortals()
    {
        Debug.Log("Portal a��ld�!");

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
