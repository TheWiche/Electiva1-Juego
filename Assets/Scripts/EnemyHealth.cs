using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Salud")]
    public int maxHealth = 100;
    public int currentHealth;

    private bool isAlive = true;

    [Header("UI")]
    public Canvas healthCanvas;         // Referencia al Canvas hijo con la barra de vida
    public Slider healthSlider;         // Referencia al Slider de vida

    void Start()
    {
        currentHealth = maxHealth;

        // Activar el canvas si está presente
        if (healthCanvas != null)
            healthCanvas.gameObject.SetActive(true);

        // Configurar el slider si está asignado
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    void Update()
    {
        // Prueba manual: tecla T le hace daño al enemigo
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(10);
        }
    }

    public void TakeDamage(int damage)
    {
        if (!isAlive) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth); // Evita que baje de 0
        Debug.Log(gameObject.name + " received " + damage + " damage. Current Health: " + currentHealth);

        // Mostrar la barra si estaba oculta (opcional)
        if (healthCanvas != null && !healthCanvas.gameObject.activeSelf)
            healthCanvas.gameObject.SetActive(true);

        // Actualizar el slider
        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isAlive = false;
        Debug.Log(gameObject.name + " has DIED!");

        // Notificar al WaveManager si existe
        if (WaveManager.instance != null)
        {
            WaveManager.instance.EnemyDied();
        }

        // Desactivar IA, movimiento y colisión
        NavMeshAgent navAgent = GetComponent<NavMeshAgent>();
        if (navAgent != null) navAgent.enabled = false;

        EnemyAI enemyAI = GetComponent<EnemyAI>();
        if (enemyAI != null) enemyAI.enabled = false;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null) meshRenderer.enabled = false;

        // Ocultar la barra de vida
        if (healthCanvas != null)
            healthCanvas.gameObject.SetActive(false);

        // Destruir el objeto tras un retardo
        Destroy(gameObject, 2f);
    }

    public bool IsAlive()
    {
        return isAlive;
    }
}
