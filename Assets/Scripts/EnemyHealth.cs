using UnityEngine;
using UnityEngine.AI; // Necesario para deshabilitar NavMeshAgent al morir
using UnityEngine.UI; // Necesario para acceder a UI (opcional si usas Canvas)

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    private bool isAlive = true; // Control de vida del enemigo

    private Canvas healthCanvas; // Referencia a la barra de vida

    void Start()
    {
        currentHealth = maxHealth;

        // Buscar el canvas hijo (barra de vida)
        healthCanvas = GetComponentInChildren<Canvas>();

        if (healthCanvas != null)
            healthCanvas.gameObject.SetActive(true); // Mostrar desde el inicio o cuando recibe daño
    }

    public void TakeDamage(int damage)
    {
        if (!isAlive) return;

        currentHealth -= damage;
        Debug.Log(gameObject.name + " received " + damage + " damage. Current Health: " + currentHealth);

        // Mostrar la barra de vida si estaba oculta (opcional)
        if (healthCanvas != null && !healthCanvas.gameObject.activeSelf)
            healthCanvas.gameObject.SetActive(true);

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
        if (col != null) col.enabled = false;

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null) meshRenderer.enabled = false;

        // Ocultar la barra de vida al morir
        if (healthCanvas != null)
            healthCanvas.gameObject.SetActive(false);

        // Destruir el GameObject después de un breve retardo
        Destroy(gameObject, 2f);
    }

    public bool IsAlive()
    {
        return isAlive;
    }
}
