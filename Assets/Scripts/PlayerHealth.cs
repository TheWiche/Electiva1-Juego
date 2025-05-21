using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public UnityEvent<float> onHealthChanged; // Para conectar con la UI

    void Start()
    {
        maxHealth = Mathf.Max(1, maxHealth); // Asegura que no sea 0
        currentHealth = maxHealth;

        float normalized = currentHealth / maxHealth;
        Debug.Log("Vida inicial: " + normalized);

        onHealthChanged.Invoke(normalized); // Valor entre 0 y 1
    }

    void Update()
    {
        // Prueba de daño al presionar H
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10f);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        float normalized = currentHealth / maxHealth;
        Debug.Log("Vida después de daño: " + normalized);

        onHealthChanged.Invoke(normalized);
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        float normalized = currentHealth / maxHealth;
        Debug.Log("Vida después de curar: " + normalized);

        onHealthChanged.Invoke(normalized);
    }
}
