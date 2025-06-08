using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Animator animator;
    public Transform attackPoint;
    public float attackRange = 0.7f;
    public LayerMask enemyLayers;

    public float attackRate = 2f;
    private float nextAttackTime = 0f;

<<<<<<< HEAD
    public bool canAttack = true; // 👈 NUEVO

    private int[] attackDamages = new int[] { 15, 20, 30 };
=======
    public AudioSource audioSource;
    public AudioClip hitSound; // <-- Sonido de golpe

    private int[] attackDamages = new int[] { 15, 20, 30 }; // Daño por animación
>>>>>>> origin/main
    private int currentAttackIndex;

    void Update()
    {
        if (!canAttack) return; // 👈 BLOQUEO DE ATAQUE

        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.Q) || Input.GetMouseButtonDown(0))
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void Attack()
    {
        if (!canAttack) return;  // Bloqueo adicional para que no se dispare la animación si no puede atacar

        currentAttackIndex = Random.Range(0, 2);
        animator.SetInteger("AttackIndex", currentAttackIndex);
        animator.SetTrigger("AttackTrigger");
    }

    void PerformHitDetection()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider enemyCollider in hitEnemies)
        {
            EnemyHealth enemyHealth = enemyCollider.GetComponent<EnemyHealth>();
            if (enemyHealth == null || !enemyHealth.IsAlive()) continue;

            float realDistance = Vector3.Distance(transform.position, enemyCollider.transform.position);
            if (realDistance > attackRange + 0.5f) continue;

            Vector3 dirToEnemy = (enemyCollider.transform.position - transform.position).normalized;
            float dot = Vector3.Dot(transform.forward, dirToEnemy);
<<<<<<< HEAD
            if (dot < 0.3f) continue;

=======

            if (dot < 0.3f) continue;

>>>>>>> origin/main
            Debug.Log("Hit: " + enemyCollider.name);
            enemyHealth.TakeDamage(attackDamages[currentAttackIndex]);

            // 🔊 Reproducir sonido de golpe
            if (audioSource != null && hitSound != null)
            {
                audioSource.PlayOneShot(hitSound, 0.6f); // Volumen opcional
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    // 👇 MÉTODO PARA REACTIVAR ATAQUE DESDE LA ANIMACIÓN
    public void EnableAttack()
    {
        canAttack = true;
        Debug.Log("Ataque habilitado desde animación");
    }

}
