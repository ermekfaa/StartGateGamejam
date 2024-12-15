using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Lobby"); // Oyunun as�l sahnesinin ad� "GameScene" olmal�.
    }

    public void QuitGame()
    {
        Debug.Log("Oyun Kapand�!");
        Application.Quit();
    }
}
