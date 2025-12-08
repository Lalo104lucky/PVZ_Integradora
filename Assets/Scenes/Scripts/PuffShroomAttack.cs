using UnityEngine;

public class PuffShroomAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public GameObject sporePrefab;
    public Transform shootPoint;
    public float attackRate = 1.5f; // Tiempo entre disparos
    public float detectionRange = 5f; // Rango de detección de zombies
    
    private Animator animator;
    private float nextAttackTime = 0f;
    private bool zombieInRange = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Detectar zombies en rango
        DetectZombies();
        
        // Disparar si hay zombie y pasó el cooldown
        if (zombieInRange && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackRate;
        }
    }

    void DetectZombies()
    {
        // Detectar zombies usando Raycast o OverlapCircle
        Collider2D[] zombies = Physics2D.OverlapCircleAll(transform.position, detectionRange);
        
        zombieInRange = false;
        foreach (Collider2D col in zombies)
        {
            if (col.CompareTag("Zombie"))
            {
                zombieInRange = true;
                break;
            }
        }
    }

    void Attack()
    {
        // Reproducir animación de ataque
        animator.SetTrigger("Attack");
    }

    // Este método se llama desde el Animation Event en el frame 8
    public void ShootSpore()
    {
        if (sporePrefab != null && shootPoint != null)
        {
            Instantiate(sporePrefab, shootPoint.position, Quaternion.identity);
        }
    }

    // Visualizar el rango de detección en el editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}