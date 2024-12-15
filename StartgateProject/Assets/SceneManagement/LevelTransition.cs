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
            // MaskImage başlangıçta görünmez yap
            maskImage.gameObject.SetActive(false);

            // Tüm RectSide nesnelerini başlangıçta devre dışı bırak
            foreach (Transform child in maskImage.transform.parent)
            {
                if (child.name.StartsWith("RectSide"))
                {
                    child.gameObject.SetActive(false);
                }
            }

            // MaskImage'yi tüm ekranı kapsayacak şekilde ayarla
            RectTransform maskRect = maskImage.rectTransform;
            maskRect.anchorMin = new Vector2(0.5f, 0.5f);  // Ortala
            maskRect.anchorMax = new Vector2(0.5f, 0.5f);  // Ortala
            maskRect.pivot = new Vector2(0.5f, 0.5f);      // Ortala
            maskRect.sizeDelta = new Vector2(10000, 10000); // Başlangıç boyutu
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
                StartCoroutine(TransitionEffect());
            }
        }

        private IEnumerator TransitionEffect()
        {
            isTransitioning = true;

            // MaskImage ve RectSide nesnelerini aktif yap
            maskImage.gameObject.SetActive(true);
            foreach (Transform child in maskImage.transform.parent)
            {
                if (child.name.StartsWith("RectSide"))
                {
                    child.gameObject.SetActive(true);
                }
            }

            // Geçiş animasyonu için boyut ayarları
            Vector2 maxSize = new Vector2(10000, 10000);
            Vector2 minSize = new Vector2(1, 1);

            // Boyutu küçültme
            yield return StartCoroutine(AnimateSize(maskImage.rectTransform, maxSize, minSize));

            // Küçük boyutta bir saniye bekle
            yield return new WaitForSeconds(1f);

            // Sahne geçişi
            LoadNextLevel();
        }

        private IEnumerator AnimateSize(RectTransform rectTransform, Vector2 startSize, Vector2 targetSize)
        {
            float elapsedTime = 0f;

            while (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                rectTransform.sizeDelta = Vector2.Lerp(startSize, targetSize, elapsedTime / transitionDuration);
                yield return null;
            }

            rectTransform.sizeDelta = targetSize; // Nihai boyut
        }

        private void LoadNextLevel()
        {
            switch (gateName)
            {
                case "Gate0":
                    SceneManager.LoadScene("LevelNature");
                    break;
                case "Gate1":
                    SceneManager.LoadScene("LevelWater");
                    break;
                case "Gate2":
                    SceneManager.LoadScene("LevelWind");
                    break;
                case "Gate3":
                    SceneManager.LoadScene("LevelFire");
                    break;
                case "Gate4":
                    SceneManager.LoadScene("LevelBoss");
                    break;
                default:
                    Debug.LogError("Tanımlanmamış bir gate kullanılıyor: " + gateName);
                    break;
            }
        }
    }
}
