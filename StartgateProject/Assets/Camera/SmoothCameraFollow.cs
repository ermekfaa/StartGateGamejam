using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    // Takip edilecek hedef (Player gibi)
    public Transform target;

    // Kameran�n hedefe olan uzakl���
    public Vector3 offset;

    // Kameran�n hareket h�z�n� kontrol eder
    public float smoothSpeed = 0.125f;

    void FixedUpdate()
    {
        // Hedef pozisyonunu hesapla (offset ile birlikte)
        Vector3 desiredPosition = target.position + offset;

        // Kameran�n yeni pozisyonunu yumu�ak ge�i� ile belirle
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Kameray� yeni pozisyona ta��
        transform.position = smoothedPosition;
    }
}
