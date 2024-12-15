using UnityEngine;
using System;

public class BossHealth : MonoBehaviour
{
    public float MaxHealth = 100f;
    public float CurrentHealth { get; private set; }

    // Sa�l�k de�i�imi i�in event
    public event Action<float, float> onHealthChanged;

    private void Awake()
    {
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);

        // Sa�l�k de�i�ti�inde event'i tetikle
        onHealthChanged?.Invoke(CurrentHealth, MaxHealth);

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Boss �ld�!");
        Destroy(gameObject);
    }
}
