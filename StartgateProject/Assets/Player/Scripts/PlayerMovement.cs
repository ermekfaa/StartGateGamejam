using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveDistance = 1f; // Karakterin her hareket etti�inde ilerleyece�i mesafe

    void Update()
    {
        // Kullan�c� giri�i kontrol�
        if (Input.GetKeyDown(KeyCode.W)) // Yukar� hareket
        {
            TryMove(Vector3.up);
        }
        else if (Input.GetKeyDown(KeyCode.S)) // A�a�� hareket
        {
            TryMove(Vector3.down);
        }
        else if (Input.GetKeyDown(KeyCode.A)) // Sola hareket
        {
            TryMove(Vector3.left);
        }
        else if (Input.GetKeyDown(KeyCode.D)) // Sa�a hareket
        {
            TryMove(Vector3.right);
        }
    }

    void TryMove(Vector3 direction)
    {
        // Yeni pozisyonu hesapla
        Vector3 newPosition = transform.position + direction * moveDistance;
        // Debug.Log(newPosition);

        transform.position = newPosition;

    }
}
