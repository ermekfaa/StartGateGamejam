using UnityEngine;
using TMPro;

public class HoverEffectWithPlayer : MonoBehaviour
{
    [Header("Text Settings")]
    public string hoverText = "Fire"; // Hoverland���nda g�sterilecek metin
    public Vector3 textOffset = new Vector3(-1.5f, 0, 0); // Metnin obje konumuna g�re offset'i

    [Header("Prefab Settings")]
    public GameObject textPrefab; // TMP i�eren prefab
    private GameObject instantiatedText; // Dinamik olarak olu�turulan metin objesi

    private SpriteRenderer spriteRenderer;
    private Color originalColor; // Objeyi hover �ncesi eski haline d�nd�rmek i�in

    [Header("Hover Settings")]
    public Color brighterWhite = new Color(1.2f, 1.2f, 1.2f, 1f); // Parlak beyaz hover rengi
    public float colorTransitionSpeed = 5f; // Renk ge�i� h�z�

    [Header("Player Proximity Settings")]
    public Transform player; // Player'�n Transform'u
    public float activationDistance = 3f; // Player ile obje aras�ndaki mesafe

    private bool isHovering = false; // Hover etkisinin aktif olup olmad���n� kontrol eder
    private bool isPlayerNearby = false; // Player yak�n m� kontrol�

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
        // Player mesafe kontrol�
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(player.position, transform.position);
            isPlayerNearby = distanceToPlayer <= activationDistance;
        }

        // Hover veya player yak�nsa parlakl�k uygula
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
        isHovering = true; // Mouse hover ba�lad���nda
        ShowText();
    }

    void OnMouseExit()
    {
        isHovering = false; // Mouse hover bitti�inde
        HideText();
    }

    void ShowText()
    {
        // TMP Text olu�turma
        if (textPrefab != null && instantiatedText == null)
        {
            Vector3 textPosition = transform.position + textOffset; // Offset ile pozisyon hesapla
            instantiatedText = Instantiate(textPrefab, textPosition, Quaternion.identity);

            // TMP bile�enini bul ve metin ayarlar�n� uygula
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
