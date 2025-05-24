using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public UnityEvent<float> onHealthChanged; 
    public UnityEvent onDied; 

    public bool IsAlive { get; protected set; } 

    protected virtual void Awake()
    {
        maxHealth = Mathf.Max(1, maxHealth);
        currentHealth = maxHealth;
        IsAlive = true;

        InvokeHealthChanged();
    }

    public virtual void TakeDamage(float amount)
    {
        if (!IsAlive) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        InvokeHealthChanged();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Heal(float amount)
    {
        if (!IsAlive) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        InvokeHealthChanged();
    }

    protected virtual void Die()
    {
        IsAlive = false;
        onDied?.Invoke();
    }

    protected void InvokeHealthChanged()
    {
        float normalized = currentHealth / maxHealth;
        onHealthChanged?.Invoke(normalized);
    }
}