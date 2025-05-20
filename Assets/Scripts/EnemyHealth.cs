using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    // Opcional: Efecto de muerte, loot, etc.
    // public GameObject deathEffect;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " recibió " + damage + " de daño. Salud restante: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " murió.");
        // Aquí puedes instanciar un efecto de muerte, soltar loot, etc.
        // if (deathEffect != null) Instantiate(deathEffect, transform.position, Quaternion.identity);

        // Destruir el objeto del enemigo
        Destroy(gameObject);
    }
}