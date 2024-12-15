using UnityEngine;
using TMPro;

public class DialogTrigger2D : MonoBehaviour
{
    [Header("Dialog Settings")]
    public string[] dialogLines;
    public string questionText = "Q: Evet\nE: Hay�r";
    public string dealMadeText = "Deal is made!";
    public Vector3 dialogOffset = new Vector3(0, 1.5f, 0);
    public float fontSize = 5f;

    [Header("TMP Settings")]
    public TextMeshPro dialogTextObject;

    private bool isPlayerInsideTrigger = false;
    private bool questionAsked = false;
    private int currentLineIndex = 0;
    private bool decisionMade = false;

    void Start()
    {
        if (dialogTextObject != null)
        {
            dialogTextObject.gameObject.SetActive(false);
            dialogTextObject.fontSize = fontSize;
        }
        else
        {
            Debug.LogError("DialogTextObject atanmad�! L�tfen bir TextMeshPro nesnesi ba�lay�n.");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInsideTrigger = true;

            if (decisionMade)
            {
                ShowDialog(dealMadeText);
            }
            else
            {
                currentLineIndex = 0;
                ShowDialog(dialogLines[currentLineIndex]);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInsideTrigger = false;
            questionAsked = false;
            HideDialog();
        }
    }

    void Update()
    {
        if (isPlayerInsideTrigger && !decisionMade)
        {
            if (!questionAsked)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    currentLineIndex++;
                    if (currentLineIndex < dialogLines.Length)
                    {
                        ShowDialog(dialogLines[currentLineIndex]);
                    }
                    else
                    {
                        ShowDialog(questionText);
                        questionAsked = true;
                    }
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    Debug.Log("Evet dedin!");
                    ShowDialog("Evet dedin! Te�ekk�rler.");
                    EndDialog();
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("Hay�r dedin!");
                    ApplyRandomHealthEffect();
                    ShowDialog("Hay�r dedin! �ans�n� denedin.");
                    EndDialog();
                }
            }
        }
    }

    void ApplyRandomHealthEffect()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                float randomValue = Random.value; // 0 ile 1 aras�nda rastgele bir say�
                if (randomValue < 0.5f)
                {
                    Debug.Log("�ansl�s�n! Can�n iki kat�na ��kt�.");
                    playerHealth.Heal(playerHealth.maxHealth); // Mevcut can� iki kat�na ��kar
                }
                else
                {
                    Debug.Log("�anss�zs�n! Can�n yar�ya d��t�.");
                    int damage = Mathf.CeilToInt(playerHealth.maxHealth / 2f);
                    playerHealth.TakeDamage(damage); // Can� yar�ya indir
                }
            }
            else
            {
                Debug.LogError("PlayerHealth script'i atanmad�.");
            }
        }
    }

    void EndDialog()
    {
        decisionMade = true;
        ShowDialog(dealMadeText);
        Debug.Log(dealMadeText);
    }

    void ShowDialog(string text)
    {
        if (dialogTextObject != null)
        {
            dialogTextObject.text = text;
            dialogTextObject.gameObject.SetActive(true);
            dialogTextObject.transform.position = transform.position + dialogOffset;
            dialogTextObject.fontSize = fontSize;
        }
    }

    void HideDialog()
    {
        if (dialogTextObject != null)
        {
            dialogTextObject.gameObject.SetActive(false);
        }
    }
}
