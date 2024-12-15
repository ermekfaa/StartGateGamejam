using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Lobby"); // Oyunun asýl sahnesinin adý "GameScene" olmalý.
    }

    public void QuitGame()
    {
        Debug.Log("Oyun Kapandý!");
        Application.Quit();
    }
}
