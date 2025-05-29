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
    private PlayerHealth playerHealth;  // Referencia directa a PlayerHealth
    private NavMeshAgent agent;
    private EnemyHealth enemyHealth;
    private Animator animator;

    private int animIDSpeed;
    private bool hasAnimator;

    private float lastAttackTime;
    private bool isAttacking = false;

    void Awake()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerTarget = playerObject.transform;
            playerHealth = playerObject.GetComponent<PlayerHealth>();

            if (playerHealth == null)
            {
                Debug.LogError("EnemyAI: Player does not have a PlayerHealth component.");
                enabled = false;
                return;
            }
        }
        else
        {
            Debug.LogError("EnemyAI: Player object with 'Player' tag not found.");
            enabled = false;
            return;
        }

        agent = GetComponent<NavMeshAgent>();
        enemyHealth = GetComponent<EnemyHealth>();
        hasAnimator = TryGetComponent(out animator);

        if (hasAnimator && animator != null)
        {
            animIDSpeed = Animator.StringToHash("Speed");
        }

        if (agent != null)
        {
            agent.stoppingDistance = stoppingDistance;
        }

        lastAttackTime = -attackCooldown;
    }

    void Update()
    {
        // Si el jugador no existe o está muerto, o el enemigo está muerto, detener todo
        if (playerTarget == null || playerHealth == null || !playerHealth.IsAlive || (enemyHealth != null && !enemyHealth.IsAlive()))
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

        // Mientras está atacando, no mover al agente ni buscar al jugador
        if (isAttacking)
        {
            if (agent != null && !agent.isStopped)
            {
                agent.isStopped = true;
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
                isAttacking = true; // Bloquea el movimiento mientras ataca
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
        Debug.Log("Enemy attack animation triggered.");

        if (hasAnimator && animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    // Este método debe ser llamado desde un evento en la animación de ataque justo en el frame del golpe
    public void ApplyDamageToPlayer()
    {
        if (playerTarget == null || playerHealth == null) return;

        // Verificamos distancia justo al momento de aplicar daño
        float distanceToPlayer = Vector3.Distance(playerTarget.position, transform.position);
        if (distanceToPlayer > attackRange)
        {
            Debug.Log("Enemy attack missed: player out of range.");
            // Permitimos que el enemigo termine el ataque sin hacer daño
            isAttacking = false;
            if (agent != null)
            {
                agent.isStopped = false;
            }
            return;
        }

        if (playerHealth.IsAlive)
        {
            playerHealth.TakeDamage(damageAmount);
            Debug.Log("Enemy dealt damage to the player.");
        }
        else
        {
            Debug.Log("Enemy tried to damage player, but player is already dead.");
        }

        isAttacking = false;
        if (agent != null)
        {
            agent.isStopped = false;
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
