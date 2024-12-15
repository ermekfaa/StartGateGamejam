using UnityEngine;
using TMPro;

public class HoverEffectWithVisualText : MonoBehaviour
{
    [Header("Text Settings")]
    public string hoverText = "Custom Hover Text"; // Hover sırasında gösterilecek metin
    public Vector3 textOffset = new Vector3(-1.5f, 0, 0); // Hover metni için offset

    [Header("Prefab Settings")]
    public GameObject textPrefab; // TMP içeren prefab
    private GameObject instantiatedText; // Dinamik olarak oluşturulan metin objesi

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

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color; // Orijinal rengi kaydet
        }
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
    }

    void OnMouseExit()
    {
        isHovering = false; // Mouse hover bittiğinde
        HideText();
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

    void HideText()
    {
        // TMP Text'i yok et
        if (instantiatedText != null)
        {
            Destroy(instantiatedText);
        }
    }
}
