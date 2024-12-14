using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    // Takip edilecek hedef (Player gibi)
    public Transform target;

    // Kameranýn hedefe olan uzaklýðý
    public Vector3 offset;

    // Kameranýn hareket hýzýný kontrol eder
    public float smoothSpeed = 0.125f;

    void FixedUpdate()
    {
        // Hedef pozisyonunu hesapla (offset ile birlikte)
        Vector3 desiredPosition = target.position + offset;

        // Kameranýn yeni pozisyonunu yumuþak geçiþ ile belirle
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Kamerayý yeni pozisyona taþý
        transform.position = smoothedPosition;
    }
}
