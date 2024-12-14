using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public int enemyCount; // Aktif d��man say�s�n� tutar

    void Start()
    {
        // Ba�lang��ta d��man say�s�n� t�m d��manlar� sayarak belirliyoruz
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        Debug.Log("Ba�lang��taki d��man say�s�: " + enemyCount);
    }

    // D��man �ld���nde bu fonksiyon �a�r�lmal�
    public void EnemyDefeated()
    {
        enemyCount--; // D��man say�s�n� azalt
        Debug.Log("Kalan d��man: " + enemyCount);

        // D��man say�s� s�f�ra ula��nca bir i�lem tetikle
        if (enemyCount <= 0)
        {
            Debug.Log("T�m d��manlar �ld�! Portal a��l�yor.");
            OpenPortals(); // Portal a�ma fonksiyonunu �a��r
        }
    }

    // Portal a�ma i�lemi burada yap�l�r
    void OpenPortals()
    {
        Debug.Log("Portal a��ld�!");
        // Portal objelerini aktif et
    }
}
