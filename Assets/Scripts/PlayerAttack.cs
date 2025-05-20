using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Animator animator;             // Referencia al Animator del personaje
    public Transform attackPoint;         // Punto desde donde se origina el ataque (punta de la espada)
    public float attackRange = 0.7f;      // Rango del ataque
    public int attackDamage = 20;         // Daño del ataque
    public LayerMask enemyLayers;         // Qué layers se consideran enemigos

    public float attackRate = 2f;         // Ataques por segundo
    private float nextAttackTime = 0f;    // Para controlar el cooldown

    // Puedes agregar una referencia al objeto de la espada si quieres activarlo/desactivarlo
    // public GameObject swordObject;

    void Update()
    {
        // Control de cooldown
        if (Time.time >= nextAttackTime)
        {
            // Detectar input de ataque (ej. clic izquierdo del mouse)
            if (Input.GetButtonDown("Fire1")) // "Fire1" usualmente es clic izq o Ctrl izq
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate; // Calcular el próximo tiempo de ataque
            }
        }
    }

    void Attack()
    {
        // 1. Disparar animación de ataque
        animator.SetTrigger("AttackTrigger"); // Asegúrate de tener un Trigger llamado "AttackTrigger" en tu Animator

        // 2. Detectar enemigos en rango (esto se hará en un momento específico de la animación)
        // Lo llamaremos desde un Animation Event (ver Paso 4) o con un pequeño delay
        // Por simplicidad inicial, lo llamaremos con un delay.
        // Un mejor enfoque es con Animation Events (explicado más abajo)
        Invoke("PerformHitDetection", 0.25f); // Ajusta este delay según tu animación
    }

    void PerformHitDetection()
    {
        // Dibuja una esfera para visualizar el rango en el editor (opcional)
        // Gizmos.DrawWireSphere(attackPoint.position, attackRange); // Solo funciona en OnDrawGizmos

        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        // 3. Aplicar daño
        foreach (Collider enemyCollider in hitEnemies)
        {
            Debug.Log("Golpeamos a: " + enemyCollider.name);
            EnemyHealth enemyHealth = enemyCollider.GetComponent<EnemyHealth>(); // Asumimos que el enemigo tiene un script EnemyHealth
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
            }
        }
    }

    // Para visualizar el rango del ataque en el editor
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}