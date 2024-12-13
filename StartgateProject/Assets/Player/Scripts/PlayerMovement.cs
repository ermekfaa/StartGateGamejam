using UnityEngine;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    public float moveDistance = 1f; // Karakterin her hareket etti�inde ilerleyece�i mesafe
    public float moveDuration = 0.1f; // Hareketin s�resi (saniye cinsinden)

    private Vector3 targetPosition;
    private Vector3 startPosition; // Hareketin ba�lang�� pozisyonu
    private bool isMoving = false;
    private float moveStartTime;

    private Queue<Vector3> inputQueue = new Queue<Vector3>(); // Tu� giri�lerini s�raya almak i�in
    private Vector3 currentDirection = Vector3.zero; // Bas�l� tutuldu�unda y�n� takip etmek i�in

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        if (isMoving)
        {
            // Hareket s�resine g�re pozisyonu g�ncelle
            float elapsedTime = (Time.time - moveStartTime) / moveDuration;

            // Sinusoidal hareket (ba�lang�� ve biti�te yava�lama, ortada h�zlanma)
            float t = Mathf.Sin(elapsedTime * Mathf.PI * 0.5f);
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            // Hedef pozisyona ula��ld� m�?
            if (elapsedTime >= 1f)
            {
                transform.position = targetPosition;
                isMoving = false;

                // Kuyruktan bir sonraki hareketi al ve ba�lat
                if (inputQueue.Count > 0)
                {
                    TryMove(inputQueue.Dequeue());
                }
                else
                {
                    currentDirection = Vector3.zero; // Kuyruk bo�sa y�n s�f�rlan�r
                }
            }

            return;
        }

        // Kullan�c� giri�i kontrol�
        if (Input.GetKey(KeyCode.W))
        {
            HandleInput(Vector3.up);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            HandleInput(Vector3.down);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            HandleInput(Vector3.left);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            HandleInput(Vector3.right);
        }
    }

    void HandleInput(Vector3 direction)
    {
        if (!isMoving)
        {
            TryMove(direction);
        }
        else if (inputQueue.Count < 1) // Sadece bir hareketi s�raya al
        {
            inputQueue.Enqueue(direction);
        }
    }

    void TryMove(Vector3 direction)
    {
        if (!isMoving)
        {
            // Yeni pozisyonu hesapla
            startPosition = transform.position; // Hareketin ba�lad��� pozisyonu kaydet
            targetPosition = transform.position + direction * moveDistance;
            isMoving = true;
            moveStartTime = Time.time;
        }
    }
}
