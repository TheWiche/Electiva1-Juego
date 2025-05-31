using UnityEngine;

public class PlayerHealth : HealthSystem
{
    [Header("Animator y GameOver")]
    public Animator animator;
    public GameObject gameOverPanel;

    [Header("Scripts a Desactivar al Morir")]
    public MonoBehaviour[] scriptsToDisable;

    public void TakeDamage(int amount)
    {
        base.TakeDamage((float)amount);
        Debug.Log("Player received " + amount + " DAMAGE from Enemy. Remaining Health: " + currentHealth);
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);
        Debug.Log("Player received " + amount + " FLOAT DAMAGE. Remaining Health: " + currentHealth);
    }

    public override void Heal(float amount)
    {
        base.Heal(amount);
        Debug.Log("Player healed " + amount + ". Current Health: " + currentHealth);
    }

    protected override void Die()
    {
        base.Die();
        Debug.Log("Player has DIED! Game Over.");

        // Desactivar control del jugador
        foreach (var script in scriptsToDisable)
        {
            if (script != null)
                script.enabled = false;
        }

        // Activar animación de muerte
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        else
        {
            Debug.LogWarning("Animator no asignado en PlayerHealth.");
        }

        // Mostrar pantalla de Game Over
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("GameOverPanel no asignado en PlayerHealth.");
        }

        // Mostrar cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if (!IsAlive) return;

        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10f);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            Heal(5f);
        }
    }
}
