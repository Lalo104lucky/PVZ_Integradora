using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 2;
    public float speed = 12f;

    public bool freeze;
    private void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.right * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Zombie>(out Zombie zombie))
        {
            zombie.Hit(damage);
            Destroy(gameObject);
        }
    }
}