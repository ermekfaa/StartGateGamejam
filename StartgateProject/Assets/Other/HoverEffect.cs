using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class HoverEffectWithVisualStars : MonoBehaviour
{
    [Header("Text Settings")]
    public string hoverText = "Custom Hover Text"; // Hover sırasında gösterilecek metin
    public Vector3 textOffset = new Vector3(-1.5f, 0, 0); // Hover metni için offset
    public Vector3 starsOffset = new Vector3(0, -0.5f, 0); // Yıldızların obje altına konumu için offset

    [Header("Prefab Settings")]
    public GameObject textPrefab; // TMP içeren prefab
    public GameObject starPrefab; // Yıldız sprite prefab
    private GameObject instantiatedText; // Dinamik olarak oluşturulan metin objesi
    private List<GameObject> instantiatedStars = new List<GameObject>(); // Dinamik olarak oluşturulan yıldız objeleri

    private SpriteRenderer spriteRenderer;
    private Color originalColor; // Objeyi hover öncesi eski haline döndürmek için

    [Header("Hover Settings")]
    public Color brighterWhite = new Color(1.2f, 1.2f, 1.2f, 1f); // Parlak beyaz hover rengi
    public float colorTransitionSpeed = 5f; // Renk geçiş hızı

    [Header("Player Proximity Settings")]
    public Transform player; // Player'ın Transform'u
    public float activationDistance = 3f; // Player ile obje arasındaki mesafe

    private bool isHovering = false; // Hover etkisinin aktif olup olmadığını kontrol eder
    private bool isPlayerNearby = false; // Player yakın mı kontrolü
    private int randomNumber; // Rastgele sayı (1 ila 5 arasında)

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color; // Orijinal rengi kaydet
        }

        // Rastgele bir sayı belirle (1 ila 5 arasında)
        randomNumber = Random.Range(1, 6);
    }

    void Update()
    {
        // Player mesafe kontrolü
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(player.position, transform.position);
            isPlayerNearby = distanceToPlayer <= activationDistance;
        }

        // Hover veya player yakınsa parlaklık uygula
        if (spriteRenderer != null)
        {
            if (isHovering || isPlayerNearby)
            {
                spriteRenderer.color = Color.Lerp(spriteRenderer.color, brighterWhite, Time.deltaTime * colorTransitionSpeed);
            }
            else
            {
                spriteRenderer.color = Color.Lerp(spriteRenderer.color, originalColor, Time.deltaTime * colorTransitionSpeed);
            }
        }
    }

    void OnMouseEnter()
    {
        isHovering = true; // Mouse hover başladığında
        ShowText();
        ShowStars(); // Yıldızları göster
    }

    void OnMouseExit()
    {
        isHovering = false; // Mouse hover bittiğinde
        HideText();
        HideStars(); // Yıldızları gizle
    }

    void ShowText()
    {
        // TMP Text oluşturma
        if (textPrefab != null && instantiatedText == null)
        {
            Vector3 textPosition = transform.position + textOffset; // Offset ile pozisyon hesapla
            instantiatedText = Instantiate(textPrefab, textPosition, Quaternion.identity);

            // TMP bileşenini bul ve metni ayarla
            TextMeshPro textMeshPro = instantiatedText.GetComponent<TextMeshPro>();
            if (textMeshPro != null)
            {
                textMeshPro.text = hoverText; // Inspector'dan düzenlenebilir metin
            }
        }
    }

    void ShowStars()
    {
        HideStars(); // Eski yıldızları sil

        // Rastgele sayıya göre yıldızları oluştur
        for (int i = 0; i < randomNumber; i++)
        {
            Vector3 starPosition = transform.position + starsOffset + new Vector3(i * 0.5f, 0, 0); // Yıldızları yan yana diz
            GameObject star = Instantiate(starPrefab, starPosition, Quaternion.identity);
            instantiatedStars.Add(star); // Oluşturulan yıldızları listeye ekle
        }
    }

    void HideText()
    {
        // TMP Text'i yok et
        if (instantiatedText != null)
        {
            Destroy(instantiatedText);
        }
    }

    void HideStars()
    {
        // Eski yıldızları yok et
        foreach (GameObject star in instantiatedStars)
        {
            Destroy(star);
        }
        instantiatedStars.Clear(); // Listeyi temizle
    }
}
