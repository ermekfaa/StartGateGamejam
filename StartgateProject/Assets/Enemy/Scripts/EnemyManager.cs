using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public int enemyCount; // Aktif düþman sayýsýný tutar

    void Start()
    {
        // Baþlangýçta düþman sayýsýný tüm düþmanlarý sayarak belirliyoruz
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        Debug.Log("Baþlangýçtaki düþman sayýsý: " + enemyCount);
    }

    // Düþman öldüðünde bu fonksiyon çaðrýlmalý
    public void EnemyDefeated()
    {
        enemyCount--; // Düþman sayýsýný azalt
        Debug.Log("Kalan düþman: " + enemyCount);

        // Düþman sayýsý sýfýra ulaþýnca bir iþlem tetikle
        if (enemyCount <= 0)
        {
            Debug.Log("Tüm düþmanlar öldü! Portal açýlýyor.");
            OpenPortals(); // Portal açma fonksiyonunu çaðýr
        }
    }

    // Portal açma iþlemi burada yapýlýr
    void OpenPortals()
    {
        Debug.Log("Portal açýldý!");
        // Portal objelerini aktif et
    }
}
