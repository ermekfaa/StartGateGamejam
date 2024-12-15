using UnityEngine;
using TMPro;

public class DialogTrigger2D : MonoBehaviour
{
    [Header("Dialog Settings")]
    public string[] dialogLines;
    public string questionText = "Q: Evet\nE: Hayýr";
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
            Debug.LogError("DialogTextObject atanmadý! Lütfen bir TextMeshPro nesnesi baðlayýn.");
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
                    ShowDialog("Evet dedin! Teþekkürler.");
                    EndDialog();
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("Hayýr dedin!");
                    ApplyRandomHealthEffect();
                    ShowDialog("Hayýr dedin! Þansýný denedin.");
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
                float randomValue = Random.value; // 0 ile 1 arasýnda rastgele bir sayý
                if (randomValue < 0.5f)
                {
                    Debug.Log("Þanslýsýn! Canýn iki katýna çýktý.");
                    playerHealth.Heal(playerHealth.maxHealth); // Mevcut caný iki katýna çýkar
                }
                else
                {
                    Debug.Log("Þanssýzsýn! Canýn yarýya düþtü.");
                    int damage = Mathf.CeilToInt(playerHealth.maxHealth / 2f);
                    playerHealth.TakeDamage(damage); // Caný yarýya indir
                }
            }
            else
            {
                Debug.LogError("PlayerHealth script'i atanmadý.");
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
