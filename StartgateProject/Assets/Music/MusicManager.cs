using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    public AudioClip menuMusic;
    public AudioClip lobbyMusic;
    public AudioClip natureMusic;
    public AudioClip waterMusic;
    public AudioClip windMusic;
    public AudioClip fireMusic;
    public AudioClip bossMusic;

    private AudioSource audioSource;

    void Awake()
    {
        // Singleton kontrolü
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Ayný anda birden fazla MusicManager oluþmasýný engelle
            return;
        }

        instance = this; // Bu MusicManager ana örnek olarak belirlenir
        DontDestroyOnLoad(gameObject); // Sahne geçiþinde yok olmasýn

        // AudioSource baþlatma
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true; // Müzik tekrar etsin
        }

        SceneManager.sceneLoaded += OnSceneLoaded; // Sahne yüklenince tetiklenir
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Yüklenen sahne: " + scene.name);

        // Eðer AudioSource silindiyse veya geçersiz hale geldiyse yeniden oluþtur
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
        }

        // Sahneye göre müzik çal
        switch (scene.name)
        {
            case "Menu":
                PlayMusic(menuMusic);
                break;
            case "Lobby":
                PlayMusic(lobbyMusic);
                break;
            case "Level1":
                PlayMusic(natureMusic);
                break;
            case "LevelWater":
                PlayMusic(waterMusic);
                break;
            case "LevelWind":
                PlayMusic(windMusic);
                break;
            case "LevelFire":
                PlayMusic(fireMusic);
                break;
            case "LevelBoss":
                PlayMusic(bossMusic);
                break;
            default:
                Debug.LogWarning("Bu sahne için müzik tanýmlý deðil!");
                break;
        }
    }

    void PlayMusic(AudioClip musicClip)
    {
        if (audioSource == null) return; // Eðer AudioSource geçersizse dur
        if (audioSource.clip == musicClip) return; // Ayný müzik çalýyorsa deðiþiklik yapma

        audioSource.Stop(); // Eski müziði durdur
        audioSource.clip = musicClip;
        audioSource.Play();
    }
}
