using UnityEngine;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    public float moveDistance = 1f; // Karakterin her hareket ettiðinde ilerleyeceði mesafe
    public float moveDuration = 0.1f; // Hareketin süresi (saniye cinsinden)

    private Vector3 targetPosition;
    private Vector3 startPosition; // Hareketin baþlangýç pozisyonu
    private bool isMoving = false;
    private float moveStartTime;

    private Queue<Vector3> inputQueue = new Queue<Vector3>(); // Tuþ giriþlerini sýraya almak için
    private Vector3 currentDirection = Vector3.zero; // Basýlý tutulduðunda yönü takip etmek için

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        if (isMoving)
        {
            // Hareket süresine göre pozisyonu güncelle
            float elapsedTime = (Time.time - moveStartTime) / moveDuration;

            // Sinusoidal hareket (baþlangýç ve bitiþte yavaþlama, ortada hýzlanma)
            float t = Mathf.Sin(elapsedTime * Mathf.PI * 0.5f);
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            // Hedef pozisyona ulaþýldý mý?
            if (elapsedTime >= 1f)
            {
                transform.position = targetPosition;
                isMoving = false;

                // Kuyruktan bir sonraki hareketi al ve baþlat
                if (inputQueue.Count > 0)
                {
                    TryMove(inputQueue.Dequeue());
                }
                else
                {
                    currentDirection = Vector3.zero; // Kuyruk boþsa yön sýfýrlanýr
                }
            }

            return;
        }

        // Kullanýcý giriþi kontrolü
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
        else if (inputQueue.Count < 1) // Sadece bir hareketi sýraya al
        {
            inputQueue.Enqueue(direction);
        }
    }

    void TryMove(Vector3 direction)
    {
        if (!isMoving)
        {
            // Yeni pozisyonu hesapla
            startPosition = transform.position; // Hareketin baþladýðý pozisyonu kaydet
            targetPosition = transform.position + direction * moveDistance;
            isMoving = true;
            moveStartTime = Time.time;
        }
    }
}
