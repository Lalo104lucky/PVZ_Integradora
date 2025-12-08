using UnityEngine;

public class SporeProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 5f;
    public int damage = 20;
    public float lifetime = 3f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Mover hacia la derecha
        rb.linearVelocity = Vector2.right * speed;


        // Destruir después del tiempo de vida
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Verificar si golpeó a un zombie
        if (other.CompareTag("Zombie"))
        {
            // Hacer daño al zombie
            Zombie zombie = other.GetComponent<Zombie>();
            if (zombie != null)
            {
                zombie.Hit(damage, false); // false = no freeze
            }

            // Destruir el proyectil
            Destroy(gameObject);
        }
    }
}