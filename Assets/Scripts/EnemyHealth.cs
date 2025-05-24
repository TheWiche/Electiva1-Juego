using UnityEngine;
using UnityEngine.AI; // Necesario para deshabilitar NavMeshAgent al morir

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    private bool isAlive = true; // Control de vida del enemigo

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (!isAlive) return; 

        currentHealth -= damage;
        Debug.Log(gameObject.name + " received " + damage + " damage. Current Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0; 
            Die();
        }
    }

    void Die()
    {
        isAlive = false; 
        Debug.Log(gameObject.name + " has DIED!");

        // Deshabilitar componentes que controlan el movimiento y la lógica del enemigo
        NavMeshAgent navAgent = GetComponent<NavMeshAgent>();
        if (navAgent != null) navAgent.enabled = false;

        EnemyAI enemyAI = GetComponent<EnemyAI>();
        if (enemyAI != null) enemyAI.enabled = false;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false; // Desactiva el collider para que no bloquee

        // Opcional: Desactivar el renderizado
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null) meshRenderer.enabled = false;
        
        // Destruir el GameObject después de un breve retardo para permitir cualquier efecto
        Destroy(gameObject, 2f); 
    }

    public bool IsAlive()
    {
        return isAlive; // Devuelve el estado de vida del enemigo
    }
}