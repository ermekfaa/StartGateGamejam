using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace IgnoreSolutions
{
    public class LevelTransition : MonoBehaviour
    {
        public string gateName;
        public GameObject player;
        public Image maskImage; // MaskImage referansı
        public float transitionDuration = 1.0f; // Geçiş süresi

        private bool isTransitioning = false;

        private void Start()
        {
            // MaskImage ve yan rect'leri başlangıçta görünmez yap
            maskImage.gameObject.SetActive(false);
            foreach (Transform child in maskImage.transform.parent)
            {
                if (child.name.StartsWith("RectSide"))
                {
                    child.gameObject.SetActive(false);
                }
            }

            // MaskImage'yi ortalamak için anchor değerlerini ayarla
            RectTransform maskRect = maskImage.rectTransform;
            maskRect.anchorMin = new Vector2(0.5f, 0.5f);
            maskRect.anchorMax = new Vector2(0.5f, 0.5f);
            maskRect.pivot = new Vector2(0.5f, 0.5f);
            maskRect.sizeDelta = new Vector2(10000, 10000); // Başlangıç boyutu büyük
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && !isTransitioning)
            {
                // Karakter hareketini durdur
                if (player.TryGetComponent(out Rigidbody2D rb))
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.isKinematic = true;
                }
                if (player.TryGetComponent(out MonoBehaviour movementScript))
                {
                    movementScript.enabled = false;
                }

                // Transition'u başlat
                StartCoroutine(TransitionAndLoadNextLevel());
            }
        }

        private IEnumerator TransitionAndLoadNextLevel()
        {
            isTransitioning = true;
            maskImage.gameObject.SetActive(true);

            // Yan rect'leri aktif yap
            foreach (Transform child in maskImage.transform.parent)
            {
                if (child.name.StartsWith("RectSide"))
                {
                    child.gameObject.SetActive(true);
                }
            }

            // MaskImage boyutunu küçült
            float elapsedTime = 0f;
            Vector2 maxSize = new Vector2(10000, 10000);
            Vector2 minSize = new Vector2(1, 1);

            while (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                maskImage.rectTransform.sizeDelta = Vector2.Lerp(maxSize, minSize, elapsedTime / transitionDuration);
                yield return null;
            }

            // MaskImage boyutunu büyüt
            elapsedTime = 0f;
            while (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                maskImage.rectTransform.sizeDelta = Vector2.Lerp(minSize, maxSize, elapsedTime / transitionDuration);
                yield return null;
            }

            // MaskImage ve yan rect'leri tekrar gizle
            maskImage.gameObject.SetActive(false);
            foreach (Transform child in maskImage.transform.parent)
            {
                if (child.name.StartsWith("RectSide"))
                {
                    child.gameObject.SetActive(false);
                }
            }

            // Bir sonraki sahneye geçiş yap
            switch (gateName)
            {
                case "Gate1":
                    SceneManager.LoadScene("Level2");
                    break;
                case "Gate2":
                    SceneManager.LoadScene("Level3");
                    break;
                case "Gate3":
                    SceneManager.LoadScene("Level4");
                    break;
                default:
                    Debug.LogError("Tanımlanmamış bir gate kullanılıyor: " + gateName);
                    break;
            }
        }
    }
}
