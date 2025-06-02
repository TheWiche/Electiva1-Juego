using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyHealth))]
public class EnemyAI : MonoBehaviour
{
    [Header("Detection and Movement Settings")]
    public float lookRadius = 15f;
    public float stoppingDistance = 1.5f;

    [Header("Player Reference (Lo encuentra automaticamente)")]
    public Transform playerTarget;  // 🔴 Se asigna manualmente en el Inspector

    [Header("Attack Settings")]
    public float attackRange = 2.0f;
    public float attackCooldown = 2.0f;
    public int damageAmount = 10;

    private PlayerHealth playerHealth;
    private NavMeshAgent agent;
    private EnemyHealth enemyHealth;
    private Animator animator;

    private int animIDSpeed;
    private bool hasAnimator;
    private float lastAttackTime;
    private bool isAttacking = false;

    void Awake()
    {
        // 🔁 Asignar automáticamente el jugador si no fue asignado manualmente
        if (playerTarget == null)
        {
            GameObject playerObj = GameObject.FindObjectOfType<PlayerHealth>()?.gameObject;
            if (playerObj != null)
            {
                playerTarget = playerObj.transform;
                playerHealth = playerObj.GetComponent<PlayerHealth>();
            }
            else
            {
                Debug.LogError("EnemyAI: No se encontró ningún objeto con PlayerHealth en la escena.");
                enabled = false;
                return;
            }
        }
        else
        {
            playerHealth = playerTarget.GetComponent<PlayerHealth>();
            if (playerHealth == null)
            {
                Debug.LogError("EnemyAI: El jugador asignado no tiene componente PlayerHealth.");
                enabled = false;
                return;
            }
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
        if (!CanAct()) 
        {
            StopMovement();
            return;
        }

        if (isAttacking) 
        {
            agent.isStopped = true;
            return;
        }

        float distanceToPlayer = Vector3.Distance(playerTarget.position, transform.position);

        if (distanceToPlayer <= lookRadius)
        {
            MoveTowardsPlayer(distanceToPlayer);
        }
        else
        {
            StopMovement();
        }

        if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            StartAttack();
        }

        UpdateAnimatorSpeed();
    }

    private bool CanAct()
    {
        return playerTarget != null 
            && playerHealth != null 
            && playerHealth.IsAlive 
            && enemyHealth != null 
            && enemyHealth.IsAlive();
    }

    private void MoveTowardsPlayer(float distanceToPlayer)
    {
        if (!agent.isOnNavMesh) return;

        agent.SetDestination(playerTarget.position);
        agent.isStopped = false;

        if (distanceToPlayer <= agent.stoppingDistance + 0.5f)
        {
            FaceTarget();
        }
    }

    private void StopMovement()
    {
        if (agent.isOnNavMesh)
        {
            if (!agent.isStopped) agent.isStopped = true;
            if (agent.hasPath) agent.ResetPath();
        }

        if (hasAnimator)
        {
            animator.SetFloat(animIDSpeed, 0, 0.1f, Time.deltaTime);
        }
    }

    private void FaceTarget()
    {
        Vector3 direction = (playerTarget.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void StartAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        agent.isStopped = true;

        if (hasAnimator)
        {
            animator.SetTrigger("Attack");
        }
        else
        {
            ApplyDamageToPlayer();
        }
    }

    public void ApplyDamageToPlayer()
    {
        if (playerTarget == null || playerHealth == null) 
        {
            EndAttack();
            return;
        }

        float distanceToPlayer = Vector3.Distance(playerTarget.position, transform.position);

        if (distanceToPlayer > attackRange)
        {
            Debug.Log("Enemy attack missed: player out of range.");
            EndAttack();
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

        EndAttack();
    }

    private void EndAttack()
    {
        isAttacking = false;
        if (agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }
    }

    private void UpdateAnimatorSpeed()
    {
        if (hasAnimator && agent.isOnNavMesh)
        {
            float speedPercent = agent.velocity.magnitude / agent.speed;
            animator.SetFloat(animIDSpeed, speedPercent, 0.1f, Time.deltaTime);
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