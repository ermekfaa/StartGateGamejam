using UnityEngine;

public class GolemBomb : MonoBehaviour
{
    public GameObject golemPrefab; // Golem prefab referans�
    public GameObject explosionEffectPrefab; // Patlama efekti prefab referans�
    public float maxDistance = 5f; // Maksimum menzil

    private Vector3 startPosition;

    private void Start()
    {
        // Ba�lang�� pozisyonunu kaydet
        startPosition = transform.position;
    }

    private void Update()
    {
        // Bomban�n menzili kontrol ediliyor
        float distanceTraveled = Vector3.Distance(startPosition, transform.position);
        if (distanceTraveled >= maxDistance)
        {
            TransformToGolem();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Bomban�n yere �arp�nca patlama efekti olu�tur ve goleme d�n��
        TransformToGolem();
    }

    private void TransformToGolem()
    {
        // Patlama efekti olu�tur
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, new Vector3(transform.position.x, transform.position.y, -1), Quaternion.identity);
        }

        // Golem olu�tur
        if (golemPrefab != null)
        {
            Instantiate(golemPrefab, transform.position, Quaternion.identity);
        }

        // Bombay� yok et
        Destroy(gameObject);
    }
}
