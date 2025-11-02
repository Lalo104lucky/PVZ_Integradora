using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngineInternal;

public class Zombie : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 1f;

    [Header("Combat")]
    public int health = 10;
    public int damage = 1;
    public float range = 1.5f;
    public float eatCooldown = 1f;
    public LayerMask plantMask;

    public Plant targetPlant;
    private bool canEat = true;

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, range, plantMask);

        if (hit.collider != null)
        {
            targetPlant = hit.collider.GetComponent<Plant>();
            if (targetPlant && canEat)
            {
                Eat();
            }
            return;
        }
        else
        {
            targetPlant = null;
        }
        transform.position -= new Vector3(speed * Time.fixedDeltaTime, 0, 0);
    }

    void Eat()
    {
        canEat = false;
        targetPlant.Hit(damage);
        Invoke(nameof(ResetEatCooldown), eatCooldown);
    }

    void ResetEatCooldown()
    {
        canEat = true;
    }

    public void Hit(int damage)
    {
        health -= damage;
        if (health <= 0)
            Destroy(gameObject);
    }
}

