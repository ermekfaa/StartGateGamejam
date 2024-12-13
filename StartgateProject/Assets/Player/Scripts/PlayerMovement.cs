using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveDistance = 1f; // Karakterin her hareket ettiðinde ilerleyeceði mesafe

    void Update()
    {
        // Kullanýcý giriþi kontrolü
        if (Input.GetKeyDown(KeyCode.W)) // Yukarý hareket
        {
            TryMove(Vector3.up);
        }
        else if (Input.GetKeyDown(KeyCode.S)) // Aþaðý hareket
        {
            TryMove(Vector3.down);
        }
        else if (Input.GetKeyDown(KeyCode.A)) // Sola hareket
        {
            TryMove(Vector3.left);
        }
        else if (Input.GetKeyDown(KeyCode.D)) // Saða hareket
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
