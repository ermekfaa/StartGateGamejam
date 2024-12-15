using UnityEngine;
using TMPro;

public class DialogTrigger2D : MonoBehaviour
{
    [Header("Dialog Settings")]
    public string[] dialogLines; // G�sterilecek c�mleler (her biri bir sat�r)
    public string questionText = "Q: Evet\nE: Hay�r"; // Soru metni
    public string dealMadeText = "Deal is made!"; // Karar verildi�inde g�sterilecek metin
    public Vector3 dialogOffset = new Vector3(0, 1.5f, 0); // Metin pozisyonu i�in offset
    public float fontSize = 5f; // Metin boyutu

    [Header("TMP Settings")]
    public TextMeshPro dialogTextObject; // Sahnedeki bir TextMeshPro objesi

    private bool isPlayerInsideTrigger = false; // Oyuncu trigger'da m�?
    private bool questionAsked = false; // Soru soruldu mu?
    private int currentLineIndex = 0; // �u anki c�mle sat�r�n�n indeksi
    private bool decisionMade = false; // Karar verildi mi?

    void Start()
    {
        // TextMeshPro nesnesini gizle (ba�lang��ta g�r�nmesin)
        if (dialogTextObject != null)
        {
            dialogTextObject.gameObject.SetActive(false);
            dialogTextObject.fontSize = fontSize; // Font boyutunu ba�lang��ta ayarla
        }
        else
        {
            Debug.LogError("DialogTextObject atanmad�! L�tfen bir TextMeshPro nesnesi ba�lay�n.");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // E�er "Player" tag'�na sahip bir obje trigger'a girerse
        {
            isPlayerInsideTrigger = true;

            if (decisionMade) // Karar daha �nce verilmi�se
            {
                ShowDialog(dealMadeText); // "Deal is made" metnini g�ster
            }
            else
            {
                currentLineIndex = 0; // Diyalog ba�tan ba�las�n
                ShowDialog(dialogLines[currentLineIndex]); // �lk sat�r� g�ster
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // E�er "Player" tag'�na sahip bir obje trigger'dan ��karsa
        {
            isPlayerInsideTrigger = false;
            questionAsked = false; // Soru s�f�rlan�r
            HideDialog(); // Diyalo�u gizle
        }
    }

    void Update()
    {
        if (isPlayerInsideTrigger && !decisionMade) // Trigger i�indeyken ve karar verilmemi�se
        {
            if (!questionAsked) // Soru sorulmam��sa
            {
                if (Input.GetKeyDown(KeyCode.Space)) // Space ile bir sonraki c�mleye ge�
                {
                    currentLineIndex++;
                    if (currentLineIndex < dialogLines.Length) // C�mleler bitmediyse
                    {
                        ShowDialog(dialogLines[currentLineIndex]);
                    }
                    else // T�m c�mleler bitti�inde soru sor
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
                    ShowDialog("Evet dedin! Te�ekk�rler.");
                    EndDialog();
                }
                else if (Input.GetKeyDown(KeyCode.E)) // E: Hay�r
                {
                    Debug.Log("Hay�r dedin!");
                    ShowDialog("Hay�r dedin! Belki sonra.");
                    EndDialog();
                }
            }
        }
    }

    void EndDialog()
    {
        decisionMade = true; // Karar verildi
        ShowDialog(dealMadeText); // "Deal is made!" metnini g�ster
        Debug.Log(dealMadeText); // Konsola yaz
    }

    void ShowDialog(string text)
    {
        if (dialogTextObject != null)
        {
            dialogTextObject.text = text; // Metni ayarla
            dialogTextObject.gameObject.SetActive(true); // TextMeshPro'yu g�r�n�r yap
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
