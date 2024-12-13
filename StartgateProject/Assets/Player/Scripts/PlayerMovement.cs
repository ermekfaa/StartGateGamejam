using UnityEngine;
using UnityEngine.Tilemaps; // Tilemap için gerekli
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    public float moveDistance = 1f; // Karakterin her hareket ettiðinde ilerleyeceði mesafe
    public float moveDuration = 0.1f; // Hareketin süresi (saniye cinsinden)
    public Tilemap wallTilemap; // Engelleri içeren Tilemap
    private HashSet<Vector3> wallPositions = new HashSet<Vector3>(); // Duvar pozisyonlarýný saklar

    private Vector3 targetPosition;
    private Vector3 startPosition; // Hareketin baþlangýç pozisyonu
    private bool isMoving = false;
    private float moveStartTime;

    void Start()
    {
        targetPosition = transform.position;
        CollectWallPositions(); // Duvar pozisyonlarýný kaydet
    }

    void Update()
    {
        if (isMoving)
        {
            float elapsedTime = (Time.time - moveStartTime) / moveDuration;
            float t = Mathf.Sin(elapsedTime * Mathf.PI * 0.5f);
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            if (elapsedTime >= 1f)
            {
                transform.position = targetPosition;
                isMoving = false;
            }

            return;
        }

        // Kullanýcý giriþlerini kontrol et
        if (Input.GetKey(KeyCode.W))
        {
            TryMove(Vector3.up);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            TryMove(Vector3.down);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            TryMove(Vector3.left);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            TryMove(Vector3.right);
        }
    }

    void TryMove(Vector3 direction)
    {
        if (isMoving) return;

        Vector3 nextPosition = transform.position + direction * moveDistance;

        // Eðer hedef pozisyon duvarsa hareketi engelle
        if (wallPositions.Contains(nextPosition))
        {
            Debug.Log("Duvara çarpamazsýn! Hareket engellendi.");
            return;
        }

        startPosition = transform.position;
        targetPosition = nextPosition;
        isMoving = true;
        moveStartTime = Time.time;
    }

    void CollectWallPositions()
    {
        // Tilemap'teki tüm aktif hücreleri kontrol et ve koordinatlarý kaydet
        BoundsInt bounds = wallTilemap.cellBounds;
        TileBase[] allTiles = wallTilemap.GetTilesBlock(bounds);

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                if (wallTilemap.HasTile(tilePosition))
                {
                    Vector3 worldPosition = wallTilemap.CellToWorld(tilePosition) + new Vector3(0.5f, 0.5f, 0); // Tile'larýn merkezine hizala
                    wallPositions.Add(worldPosition);

                    // Pozisyonu konsola yazdýr
                    Debug.Log($"Duvar Pozisyonu: {worldPosition}");
                }
            }
        }

        Debug.Log($"Toplam {wallPositions.Count} duvar pozisyonu kaydedildi.");
    }
}
