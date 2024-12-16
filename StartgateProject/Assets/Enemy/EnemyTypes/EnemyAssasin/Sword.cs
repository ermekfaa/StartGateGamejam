using UnityEngine;

public class Sword : MonoBehaviour
{
    [Header("Sword Settings")]
    public int damage = 10; // K�l�c�n verdi�i hasar
    private bool canDealDamage = true; // Cooldown kontrol�

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canDealDamage && collision.CompareTag("Player")) // Sadece "Player" tag'li nesneler
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>(); // PlayerHealth bile�enini al
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage); // Hasar ver
                Debug.Log($"Sword dealt {damage} damage to player.");
                StartCoroutine(DamageCooldown()); // Cooldown ba�lat
            }
        }
    }

    private System.Collections.IEnumerator DamageCooldown()
    {
        canDealDamage = false; // Hasar vermeyi ge�ici olarak devre d��� b�rak
        yield return new WaitForSeconds(0.5f); // 0.5 saniye bekle
        canDealDamage = true; // Hasar vermeyi yeniden etkinle�tir
    }
}
