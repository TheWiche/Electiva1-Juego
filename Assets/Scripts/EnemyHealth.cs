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
        if (!IsAlive()) // No tomar daño si ya está muerto
            return;

        currentHealth -= damage;
        Debug.Log(gameObject.name + " recibió " + damage + " de daño. Salud restante: " + currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0; // Asegurarse que no sea negativo
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " murió.");
        // Aquí puedes instanciar un efecto de muerte, soltar loot, etc.
        // if (deathEffect != null) Instantiate(deathEffect, transform.position, Quaternion.identity);

        // Podrías desactivar componentes en lugar de destruir inmediatamente
        // si quieres que el NavMeshAgent se detenga correctamente o
        // si tienes una animación de muerte que necesita reproducirse.
        // Por ejemplo:
        // GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        // GetComponent<Collider>().enabled = false;
        // this.enabled = false; // Desactiva este script de salud
        // GetComponent<EnemyAI>().enabled = false; // Desactiva la IA

        // Llamar a la destrucción después de un delay si tienes animaciones de muerte
        Destroy(gameObject, 2f); // Destruir después de 2 segundos (ajusta si tienes anim de muerte)
                                 // O Destruir inmediatamente si no hay animación de muerte:
                                 // Destroy(gameObject);
    }

    // --- MÉTODO AÑADIDO ---
    public bool IsAlive()
    {
        return currentHealth > 0;
    }
}