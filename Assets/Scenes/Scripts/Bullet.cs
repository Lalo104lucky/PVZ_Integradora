using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

    public float speed = 0.8f;

    private void Update()
    {
        transform.position += new Vector3(speed * Time.deltaTime, 0, 0);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Zombie>(out Zombie zombie)) {
            zombie.Hit(damage);
            Destroy(gameObject);
        }
    }
}
