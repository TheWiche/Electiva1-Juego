using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Animator animator;             
    public Transform attackPoint;         
    public float attackRange = 0.7f;      
    public int attackDamage = 20;         
    public LayerMask enemyLayers;         

    public float attackRate = 2f;         
    private float nextAttackTime = 0f;    

    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void Attack()
    {
        animator.SetTrigger("AttackTrigger"); 
        Invoke("PerformHitDetection", 0.25f); 
    }

    void PerformHitDetection()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider enemyCollider in hitEnemies)
        {
            Debug.Log("Hit: " + enemyCollider.name);
            EnemyHealth enemyHealth = enemyCollider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}