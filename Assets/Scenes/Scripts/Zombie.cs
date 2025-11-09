using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngineInternal;

public class Zombie : MonoBehaviour
{
    [Header("Movement")]
    private float speed = 1f;

    [Header("Combat")]
    private int health = 10;
    private int damage = 1;
    private float range = 1.5f;
    private float eatCooldown = 1f;
    public LayerMask plantMask;

    public ZombieTypes type;

    public Plant targetPlant;
    private bool canEat = true;

    private AudioSource source;

    public AudioClip[] groans;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        health = type.health;
        speed = type.speed;
        damage = type.damage;
        range = type.range;

        GetComponent<SpriteRenderer>().sprite = type.sprite;
        eatCooldown = type.eatCooldown;
        Invoke("Groan", Random.Range(1f, 20f));
    }

    void Groan()
    {
        source.PlayOneShot(groans[Random.Range(0, groans.Length)]);
    }

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

    public void Hit(int damage, bool freeze)
    {
        source.PlayOneShot(type.hitClips[Random.Range(0, type.hitClips.Length)]);
        health -= damage;
        if(freeze)
        {
            Freeze();
        }
        if (health <= 0)
        {
            GetComponent<SpriteRenderer>().sprite = type.deathSprite;
            Destroy(gameObject, 1);
        }
    }

    void Freeze()
    {
        CancelInvoke("UnFreeze");
        GetComponent<SpriteRenderer>().color = Color.blue;
        speed = type.speed / 2;
        Invoke("UnFreeze", 5);
    }

    void UnFreeze()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
        speed = type.speed;
    }
}

