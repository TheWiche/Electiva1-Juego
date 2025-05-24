using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyHealth))] 
public class EnemyAI : MonoBehaviour
{
    [Header("Detection and Movement Settings")]
    public float lookRadius = 15f;
    public float stoppingDistance = 1.5f;

    [Header("Attack Settings")]
    public float attackRange = 2.0f;
    public float attackCooldown = 2.0f;
    public int damageAmount = 10;

    private Transform playerTarget;
    private NavMeshAgent agent;
    private EnemyHealth enemyHealth; // Ahora es EnemyHealth, no HealthSystem
    private Animator animator;

    private int animIDSpeed;
    private bool hasAnimator;

    private float lastAttackTime;

    void Awake()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerTarget = playerObject.transform;
        }
        else
        {
            Debug.LogError("EnemyAI: Player object with 'Player' tag not found. Please ensure the player has the 'Player' tag.");
            enabled = false;
            return;
        }

        agent = GetComponent<NavMeshAgent>();
        enemyHealth = GetComponent<EnemyHealth>(); // Obtenemos EnemyHealth
        hasAnimator = TryGetComponent(out animator);

        if (hasAnimator && animator != null)
        {
            animIDSpeed = Animator.StringToHash("Speed");
        }

        if (agent != null)
        {
            agent.stoppingDistance = this.stoppingDistance; 
        }

        lastAttackTime = -attackCooldown; 
    }

    void Update()
    {
        if (playerTarget == null || (enemyHealth != null && !enemyHealth.IsAlive())) // Usa IsAlive() de EnemyHealth
        {
            if (agent != null && agent.isOnNavMesh && agent.isActiveAndEnabled)
            {
                if (!agent.isStopped) agent.isStopped = true;
                if (agent.hasPath) agent.ResetPath();
            }

            if (hasAnimator && animator != null)
            {
                animator.SetFloat(animIDSpeed, 0);
            }
            return;
        }
        
        float distanceToPlayer = Vector3.Distance(playerTarget.position, transform.position);

        if (distanceToPlayer <= lookRadius)
        {
            if (agent.isOnNavMesh && agent.isActiveAndEnabled)
            {
                agent.SetDestination(playerTarget.position);
                if (agent.isStopped) agent.isStopped = false; 
            }

            if (distanceToPlayer <= agent.stoppingDistance + 0.5f) 
            {
                FaceTarget();
            }
        }
        else 
        {
            if (agent.isOnNavMesh && agent.isActiveAndEnabled)
            {
                if (!agent.isStopped) agent.isStopped = true;
                if (agent.hasPath) agent.ResetPath();
            }
        }

        if (distanceToPlayer <= attackRange)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                AttackPlayer();
                lastAttackTime = Time.time;
            }
        }

        if (hasAnimator && animator != null && agent != null && agent.isOnNavMesh)
        {
            float speedPercent = agent.velocity.magnitude / agent.speed;
            animator.SetFloat(animIDSpeed, speedPercent, 0.1f, Time.deltaTime);
        }
    }

    void FaceTarget()
    {
        Vector3 direction = (playerTarget.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void AttackPlayer()
    {
        Debug.Log("Enemy attacked the player!");

        PlayerHealth playerHealth = playerTarget.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageAmount);
        }
        else
        {
            Debug.LogWarning("EnemyAI: Player does not have a PlayerHealth component. Cannot apply damage.");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, lookRadius); 

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); 
    }
}