using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public bool canAttack = true;  
    public Animator animator;
    public Transform attackPoint;
    public float attackRange = 0.7f;
    public LayerMask enemyLayers;

    public float attackRate = 2f;
    private float nextAttackTime = 0f;

    public AudioSource audioSource;
    public AudioClip swingSound; // Sonido de corte al aire
    public AudioClip hitSound;   // Sonido de impacto

    private int[] attackDamages = new int[] { 15, 20, 30 }; // Daño por animación
    private int currentAttackIndex;

    void Update()
    {
        if (!canAttack) return;

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
        if (!canAttack) return;

        currentAttackIndex = Random.Range(0, 2);
        animator.SetInteger("AttackIndex", currentAttackIndex);
        animator.SetTrigger("AttackTrigger");

        // 🔊 Sonido de ataque (swing), incluso si no golpea
        if (audioSource != null && swingSound != null)
        {
            audioSource.PlayOneShot(swingSound, 0.6f);
        }
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
            if (dot < 0.3f) continue;

            Debug.Log("Hit: " + enemyCollider.name);
            enemyHealth.TakeDamage(attackDamages[currentAttackIndex]);

            // 🔊 Sonido de impacto (solo si golpea)
            if (audioSource != null && hitSound != null)
            {
                audioSource.PlayOneShot(hitSound, 0.8f);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void EnableAttack()
    {
        canAttack = true;
        Debug.Log("Ataque habilitado desde animación");
    }
}
