using UnityEngine;

public class GolemBomb : MonoBehaviour
{
    public GameObject golemPrefab; // Golem prefab referansý
    public GameObject explosionEffectPrefab; // Patlama efekti prefab referansý
    public float maxDistance = 5f; // Maksimum menzil

    private Vector3 startPosition;

    private void Start()
    {
        // Baþlangýç pozisyonunu kaydet
        startPosition = transform.position;
    }

    private void Update()
    {
        // Bombanýn menzili kontrol ediliyor
        float distanceTraveled = Vector3.Distance(startPosition, transform.position);
        if (distanceTraveled >= maxDistance)
        {
            TransformToGolem();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Bombanýn yere çarpýnca patlama efekti oluþtur ve goleme dönüþ
        TransformToGolem();
    }

    private void TransformToGolem()
    {
        // Patlama efekti oluþtur
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, new Vector3(transform.position.x, transform.position.y, -1), Quaternion.identity);
        }

        // Golem oluþtur
        if (golemPrefab != null)
        {
            Instantiate(golemPrefab, transform.position, Quaternion.identity);
        }

        // Bombayý yok et
        Destroy(gameObject);
    }
}
