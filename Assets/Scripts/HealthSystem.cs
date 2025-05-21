using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public UnityEvent<float> onHealthChanged; // Se puede conectar a la UI (0-1)

    protected virtual void Start()
    {
        maxHealth = Mathf.Max(1, maxHealth);
        currentHealth = maxHealth;

        InvokeHealthChanged();
    }

    public virtual void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        InvokeHealthChanged();
    }

    public virtual void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        InvokeHealthChanged();
    }

    protected void InvokeHealthChanged()
    {
        float normalized = currentHealth / maxHealth;
        Debug.Log($"{gameObject.name} salud actual: {normalized}");
        onHealthChanged?.Invoke(normalized);
    }
}
