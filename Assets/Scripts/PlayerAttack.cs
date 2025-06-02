using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Animator animator;
    public Transform attackPoint;
    public float attackRange = 0.7f;
    public LayerMask enemyLayers;

    public float attackRate = 2f;
    private float nextAttackTime = 0f;

    private int[] attackDamages = new int[] { 15, 20, 30 }; // Daño por animación
    private int currentAttackIndex;

    void Update()
    {
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
        currentAttackIndex = Random.Range(0, 2); // 0, 1 
        animator.SetInteger("AttackIndex", currentAttackIndex);
        animator.SetTrigger("AttackTrigger");
    }

    void PerformHitDetection()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider enemyCollider in hitEnemies)
        {
            // 1. Verificar si el enemigo tiene EnemyHealth
            EnemyHealth enemyHealth = enemyCollider.GetComponent<EnemyHealth>();
            if (enemyHealth == null || !enemyHealth.IsAlive()) continue;

            // 2. Verificar que el enemigo esté realmente cerca (usando el centro del jugador)
            float realDistance = Vector3.Distance(transform.position, enemyCollider.transform.position);
            if (realDistance > attackRange + 0.5f) continue;

            // 3. Verificar que esté en la dirección frontal del jugador
            Vector3 dirToEnemy = (enemyCollider.transform.position - transform.position).normalized;
            float dot = Vector3.Dot(transform.forward, dirToEnemy); // 1 = delante, 0 = lado, -1 = detrás

            if (dot < 0.3f) continue; // Solo enemigos al frente (~72° de ángulo)

            // 4. Aplicar daño
            Debug.Log("Hit: " + enemyCollider.name);
            enemyHealth.TakeDamage(attackDamages[currentAttackIndex]);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
