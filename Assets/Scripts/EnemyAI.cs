using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyHealth))]
public class EnemyAI : MonoBehaviour
{
    public float lookRadius = 15f;
    public float stoppingDistance = 1.5f;

    private Transform playerTarget;
    private NavMeshAgent agent;
    private EnemyHealth health;
    private Animator animator;

    private int animIDSpeed;
    private bool hasAnimator;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTarget = playerObject.transform;
        }
        else
        {
            Debug.LogError("EnemyAI: No se pudo encontrar al jugador. Asegúrate que el jugador tenga el Tag 'Player'.");
            enabled = false;
            return;
        }

        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<EnemyHealth>();
        hasAnimator = TryGetComponent(out animator);

        if (hasAnimator)
        {
            animIDSpeed = Animator.StringToHash("Speed");
        }

        if (agent != null)
        {
            agent.stoppingDistance = stoppingDistance;
        }
    }

    void Update()
    {
        if (playerTarget == null || (health != null && !health.IsAlive()))
        {
            if (agent != null && agent.isOnNavMesh && agent.isActiveAndEnabled)
            {
                if (!agent.isStopped)
                {
                    agent.isStopped = true;
                }
                 if (agent.hasPath) // Solo resetear si tiene un camino activo
                {
                    agent.ResetPath();
                }
            }

            if (hasAnimator && animator != null)
            {
                animator.SetFloat(animIDSpeed, 0);
            }
            return;
        }
        
        if (health == null || !health.IsAlive()) return; // Doble chequeo por si acaso health se vuelve null


        float distanceToPlayer = Vector3.Distance(playerTarget.position, transform.position);

        if (distanceToPlayer <= lookRadius)
        {
            if (agent.isOnNavMesh && agent.isActiveAndEnabled)
            {
                agent.SetDestination(playerTarget.position);
                if (agent.isStopped) // Reanudar si estaba detenido
                {
                    agent.isStopped = false;
                }
            }

            if (distanceToPlayer <= agent.stoppingDistance + 0.5f)
            {
                FaceTarget();
            }
        }
        else
        {
            if (agent.isOnNavMesh && agent.isActiveAndEnabled && !agent.isStopped)
            {
                agent.isStopped = true;
            }
        }

        if (hasAnimator && animator != null && agent != null)
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
    }
}