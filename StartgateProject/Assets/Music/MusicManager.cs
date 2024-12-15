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
        // Singleton kontrol�
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Ayn� anda birden fazla MusicManager olu�mas�n� engelle
            return;
        }

        instance = this; // Bu MusicManager ana �rnek olarak belirlenir
        DontDestroyOnLoad(gameObject); // Sahne ge�i�inde yok olmas�n

        // AudioSource ba�latma
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true; // M�zik tekrar etsin
        }

        SceneManager.sceneLoaded += OnSceneLoaded; // Sahne y�klenince tetiklenir
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Y�klenen sahne: " + scene.name);

        // E�er AudioSource silindiyse veya ge�ersiz hale geldiyse yeniden olu�tur
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
        }

        // Sahneye g�re m�zik �al
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
                Debug.LogWarning("Bu sahne i�in m�zik tan�ml� de�il!");
                break;
        }
    }

    void PlayMusic(AudioClip musicClip)
    {
        if (audioSource == null) return; // E�er AudioSource ge�ersizse dur
        if (audioSource.clip == musicClip) return; // Ayn� m�zik �al�yorsa de�i�iklik yapma

        audioSource.Stop(); // Eski m�zi�i durdur
        audioSource.clip = musicClip;
        audioSource.Play();
    }
}
