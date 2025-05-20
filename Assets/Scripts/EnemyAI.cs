using UnityEngine;
using UnityEngine.AI; // Necesario para NavMeshAgent

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyHealth))] // Para saber cuándo está vivo
public class EnemyAI : MonoBehaviour
{
    public float lookRadius = 15f;    // Rango de detección para empezar a perseguir
    public float stoppingDistance = 1.5f; // Qué tan cerca llega del jugador antes de detenerse (para atacar luego)

    private Transform playerTarget;
    private NavMeshAgent agent;
    private EnemyHealth health;
    private Animator animator; // Opcional, para animaciones de movimiento

    // Opcional: IDs de animación
    private int animIDSpeed;
    private bool hasAnimator;

    void Start()
    {
        // Intentar encontrar al jugador por Tag. Asegúrate que tu jugador tenga el Tag "Player"
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTarget = playerObject.transform;
        }
        else
        {
            Debug.LogError("EnemyAI: No se pudo encontrar al jugador. Asegúrate que el jugador tenga el Tag 'Player'.");
            enabled = false; // Deshabilitar este script si no hay jugador
            return;
        }

        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<EnemyHealth>();
        hasAnimator = TryGetComponent(out animator);

        if (hasAnimator)
        {
            animIDSpeed = Animator.StringToHash("Speed"); // Asume que tienes un parámetro float "Speed" en tu Animator
        }

        if (agent != null)
        {
            agent.stoppingDistance = stoppingDistance;
        }
    }

    void Update()
    {
        if (playerTarget == null || !health.IsAlive()) // Si no hay jugador o el enemigo está muerto
        {
            if (agent.isOnNavMesh && agent.isActiveAndEnabled) // Verifica si el agente está activo en el NavMesh
            {
                agent.isStopped = true;
                agent.ResetPath(); // Detiene el movimiento si el enemigo muere
            }

            if (hasAnimator)
            {
                animator.SetFloat(animIDSpeed, 0);
            }
            return;
        }

        float distanceToPlayer = Vector3.Distance(playerTarget.position, transform.position);

        if (distanceToPlayer <= lookRadius)
        {
            // Perseguir al jugador
            if (agent.isOnNavMesh && agent.isActiveAndEnabled)
            {
                agent.SetDestination(playerTarget.position);
                agent.isStopped = false;
            }


            // Girar para encarar al jugador si está lo suficientemente cerca
            if (distanceToPlayer <= agent.stoppingDistance + 0.5f) // Un pequeño umbral extra
            {
                FaceTarget();
                // Aquí es donde eventualmente llamarías a la lógica de ataque
            }
        }
        else
        {
            // Si el jugador está fuera de rango, detenerse (o patrullar, si lo implementas)
             if (agent.isOnNavMesh && agent.isActiveAndEnabled)
            {
                agent.isStopped = true;
                // agent.ResetPath(); // Opcional: limpiar el camino actual
            }
        }

        // Actualizar animación de velocidad
        if (hasAnimator)
        {
            // Usar la velocidad deseada del NavMeshAgent, no la velocidad real del transform,
            // para que la animación de caminar/correr se active incluso si está temporalmente bloqueado.
            float speedPercent = agent.velocity.magnitude / agent.speed;
            animator.SetFloat(animIDSpeed, speedPercent, 0.1f, Time.deltaTime); // El 0.1f es un tiempo de suavizado
        }
    }

    void FaceTarget()
    {
        Vector3 direction = (playerTarget.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    // Añade esto a EnemyHealth.cs para que EnemyAI sepa si está vivo:
    // public bool IsAlive()
    // {
    //     return currentHealth > 0;
    // }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
    }
}