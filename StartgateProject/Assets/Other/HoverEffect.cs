using UnityEngine;
using TMPro;

public class HoverEffectWithPlayer : MonoBehaviour
{
    [Header("Text Settings")]
    public string hoverText = "Fire"; // Hoverlandýðýnda gösterilecek metin
    public Vector3 textOffset = new Vector3(-1.5f, 0, 0); // Metnin obje konumuna göre offset'i

    [Header("Prefab Settings")]
    public GameObject textPrefab; // TMP içeren prefab
    private GameObject instantiatedText; // Dinamik olarak oluþturulan metin objesi

    private SpriteRenderer spriteRenderer;
    private Color originalColor; // Objeyi hover öncesi eski haline döndürmek için

    [Header("Hover Settings")]
    public Color brighterWhite = new Color(1.2f, 1.2f, 1.2f, 1f); // Parlak beyaz hover rengi
    public float colorTransitionSpeed = 5f; // Renk geçiþ hýzý

    [Header("Player Proximity Settings")]
    public Transform player; // Player'ýn Transform'u
    public float activationDistance = 3f; // Player ile obje arasýndaki mesafe

    private bool isHovering = false; // Hover etkisinin aktif olup olmadýðýný kontrol eder
    private bool isPlayerNearby = false; // Player yakýn mý kontrolü

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

        // Hover veya player yakýnsa parlaklýk uygula
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
        isHovering = true; // Mouse hover baþladýðýnda
        ShowText();
    }

    void OnMouseExit()
    {
        isHovering = false; // Mouse hover bittiðinde
        HideText();
    }

    void ShowText()
    {
        // TMP Text oluþturma
        if (textPrefab != null && instantiatedText == null)
        {
            Vector3 textPosition = transform.position + textOffset; // Offset ile pozisyon hesapla
            instantiatedText = Instantiate(textPrefab, textPosition, Quaternion.identity);

            // TMP bileþenini bul ve metin ayarlarýný uygula
            TextMeshPro textMeshPro = instantiatedText.GetComponent<TextMeshPro>();
            if (textMeshPro != null)
            {
                textMeshPro.text = hoverText; // Metni ayarla
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
