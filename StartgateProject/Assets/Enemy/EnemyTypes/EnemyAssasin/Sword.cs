using UnityEngine;

public class Sword : MonoBehaviour
{
    [Header("Sword Settings")]
    public int damage = 10; // Kýlýcýn verdiði hasar
    private bool canDealDamage = true; // Cooldown kontrolü

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canDealDamage && collision.CompareTag("Player")) // Sadece "Player" tag'li nesneler
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>(); // PlayerHealth bileþenini al
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage); // Hasar ver
                Debug.Log($"Sword dealt {damage} damage to player.");
                StartCoroutine(DamageCooldown()); // Cooldown baþlat
            }
        }
    }

    private System.Collections.IEnumerator DamageCooldown()
    {
        canDealDamage = false; // Hasar vermeyi geçici olarak devre dýþý býrak
        yield return new WaitForSeconds(0.5f); // 0.5 saniye bekle
        canDealDamage = true; // Hasar vermeyi yeniden etkinleþtir
    }
}
