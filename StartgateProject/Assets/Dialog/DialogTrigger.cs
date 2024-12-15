using UnityEngine;
using TMPro;

public class DialogTrigger2D : MonoBehaviour
{
    [Header("Dialog Settings")]
    public string[] dialogLines; // Gösterilecek cümleler (her biri bir satýr)
    public string questionText = "Q: Evet\nE: Hayýr"; // Soru metni
    public string dealMadeText = "Deal is made!"; // Karar verildiðinde gösterilecek metin
    public Vector3 dialogOffset = new Vector3(0, 1.5f, 0); // Metin pozisyonu için offset
    public float fontSize = 5f; // Metin boyutu

    [Header("TMP Settings")]
    public TextMeshPro dialogTextObject; // Sahnedeki bir TextMeshPro objesi

    private bool isPlayerInsideTrigger = false; // Oyuncu trigger'da mý?
    private bool questionAsked = false; // Soru soruldu mu?
    private int currentLineIndex = 0; // Þu anki cümle satýrýnýn indeksi
    private bool decisionMade = false; // Karar verildi mi?

    void Start()
    {
        // TextMeshPro nesnesini gizle (baþlangýçta görünmesin)
        if (dialogTextObject != null)
        {
            dialogTextObject.gameObject.SetActive(false);
            dialogTextObject.fontSize = fontSize; // Font boyutunu baþlangýçta ayarla
        }
        else
        {
            Debug.LogError("DialogTextObject atanmadý! Lütfen bir TextMeshPro nesnesi baðlayýn.");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Eðer "Player" tag'ýna sahip bir obje trigger'a girerse
        {
            isPlayerInsideTrigger = true;

            if (decisionMade) // Karar daha önce verilmiþse
            {
                ShowDialog(dealMadeText); // "Deal is made" metnini göster
            }
            else
            {
                currentLineIndex = 0; // Diyalog baþtan baþlasýn
                ShowDialog(dialogLines[currentLineIndex]); // Ýlk satýrý göster
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Eðer "Player" tag'ýna sahip bir obje trigger'dan çýkarsa
        {
            isPlayerInsideTrigger = false;
            questionAsked = false; // Soru sýfýrlanýr
            HideDialog(); // Diyaloðu gizle
        }
    }

    void Update()
    {
        if (isPlayerInsideTrigger && !decisionMade) // Trigger içindeyken ve karar verilmemiþse
        {
            if (!questionAsked) // Soru sorulmamýþsa
            {
                if (Input.GetKeyDown(KeyCode.Space)) // Space ile bir sonraki cümleye geç
                {
                    currentLineIndex++;
                    if (currentLineIndex < dialogLines.Length) // Cümleler bitmediyse
                    {
                        ShowDialog(dialogLines[currentLineIndex]);
                    }
                    else // Tüm cümleler bittiðinde soru sor
                    {
                        ShowDialog(questionText);
                        questionAsked = true;
                    }
                }
            }
            else // Soru sorulduysa
            {
                if (Input.GetKeyDown(KeyCode.Q)) // Q: Evet
                {
                    Debug.Log("Evet dedin!");
                    ShowDialog("Evet dedin! Teþekkürler.");
                    EndDialog();
                }
                else if (Input.GetKeyDown(KeyCode.E)) // E: Hayýr
                {
                    Debug.Log("Hayýr dedin!");
                    ShowDialog("Hayýr dedin! Belki sonra.");
                    EndDialog();
                }
            }
        }
    }

    void EndDialog()
    {
        decisionMade = true; // Karar verildi
        ShowDialog(dealMadeText); // "Deal is made!" metnini göster
        Debug.Log(dealMadeText); // Konsola yaz
    }

    void ShowDialog(string text)
    {
        if (dialogTextObject != null)
        {
            dialogTextObject.text = text; // Metni ayarla
            dialogTextObject.gameObject.SetActive(true); // TextMeshPro'yu görünür yap
            dialogTextObject.transform.position = transform.position + dialogOffset; // Pozisyonunu ayarla
            dialogTextObject.fontSize = fontSize; // Metin boyutunu ayarla
        }
    }

    void HideDialog()
    {
        if (dialogTextObject != null)
        {
            dialogTextObject.gameObject.SetActive(false); // TextMeshPro'yu gizle
        }
    }
}
