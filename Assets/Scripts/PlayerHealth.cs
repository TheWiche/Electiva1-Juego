using UnityEngine;

public class PlayerHealth : HealthSystem
{
    [Header("Animator y GameOver")]
    public Animator animator;
    public GameObject gameOverPanel;

    [Header("Scripts a Desactivar al Morir")]
    public MonoBehaviour[] scriptsToDisable;

    [Header("Sonido de Daño")]
    public AudioClip hurtSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            Debug.LogWarning("No se encontró AudioSource en el jugador.");
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);
        Debug.Log("Player received " + amount + " FLOAT DAMAGE. Remaining Health: " + currentHealth);

        PlayRandomHurtAnimation();

        // 🔊 Reproducir sonido de daño
        if (audioSource != null && hurtSound != null)
        {
            audioSource.PlayOneShot(hurtSound, 0.8f); // Puedes ajustar el volumen
        }
    }

    public override void Heal(float amount)
    {
        base.Heal(amount);
        Debug.Log("Player healed " + amount + ". Current Health: " + currentHealth);
    }

    private void PlayRandomHurtAnimation()
    {
        if (animator == null || !IsAlive) return;

        int randomIndex = Random.Range(1, 3); // genera 1 o 2
        string triggerName = "Hurt_" + randomIndex;

        // BLOQUEAMOS el ataque justo antes de activar la animación de daño
        PlayerAttack playerAttack = GetComponent<PlayerAttack>();
        if (playerAttack != null)
            playerAttack.canAttack = false;

        animator.SetTrigger(triggerName);
        Debug.Log("Reproduciendo animación de daño: " + triggerName);
    }

    protected override void Die()
    {
        base.Die();
        Debug.Log("Player has DIED! Game Over.");

        foreach (var script in scriptsToDisable)
        {
            if (script != null)
                script.enabled = false;
        }

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        else
        {
            Debug.LogWarning("Animator no asignado en PlayerHealth.");
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("GameOverPanel no asignado en PlayerHealth.");
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if (!IsAlive) return;

        //if (Input.GetKeyDown(KeyCode.H))
        //{
        //    TakeDamage(10f);
        //}

        //if (Input.GetKeyDown(KeyCode.J))
        //{
        //    Heal(5f);
        //}
    }
}
