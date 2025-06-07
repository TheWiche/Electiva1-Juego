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
    public Canvas healthCanvas;
    public Slider healthSlider;

    [Header("Animación")]
    public Animator animator;

    void Start()
    {
        // Ajustar la vida según dificultad
        int dificultad = PlayerPrefs.GetInt("GameDifficulty", 1); // 0 = Fácil, 1 = Normal, 2 = Difícil

        string dificultadTexto = dificultad == 0 ? "Fácil" : (dificultad == 2 ? "Difícil" : "Normal");
        Debug.Log("Dificultad seleccionada: " + dificultadTexto);

        switch (dificultad)
        {
            case 0: maxHealth = 50; break;   // Fácil
            case 2: maxHealth = 150; break;  // Difícil
            default: maxHealth = 100; break; // Normal
        }

        currentHealth = maxHealth;

        if (healthCanvas != null)
            healthCanvas.gameObject.SetActive(true);

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(10);
        }
    }

    public void TakeDamage(int damage)
    {
        if (!isAlive) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        Debug.Log(gameObject.name + " received " + damage + " damage. Current Health: " + currentHealth);

        if (healthCanvas != null && !healthCanvas.gameObject.activeSelf)
            healthCanvas.gameObject.SetActive(true);

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

        // Activar animación de muerte aleatoria con triggers
        if (animator != null)
        {
            int randomIndex = Random.Range(1, 4); // 1, 2, 3
            string triggerName = "Die_" + randomIndex;
            animator.SetTrigger(triggerName);

            //StartCoroutine(DisableAnimatorAfterDelay(2f)); // Ajusta según duración de la animación
        }
        else
        {
            Debug.LogWarning("Animator no asignado en EnemyHealth.");
        }

        // Ocultar la barra de vida
        if (healthCanvas != null)
            healthCanvas.gameObject.SetActive(false);

        // Destruir después de un retardo
        Destroy(gameObject, 10f);
    }

    private System.Collections.IEnumerator DisableAnimatorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (animator != null)
            animator.enabled = false;
    }

    public bool IsAlive()
    {
        return isAlive;
    }
}
