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

    [Header("Audio Clips")]
    public AudioClip[] chompClips;
    public AudioClip gulpClip;
    public AudioClip freezeClip;

    private bool isFrozen = false;

    private AudioSource source;
    public AudioClip[] groans;

    public bool lastZombie;
    public bool dead;

    private Animator animator;
    private Renderer[] allRenderers;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        allRenderers = GetComponentsInChildren<Renderer>();
        health = type.health;
        speed = type.speed;
        damage = type.damage;
        range = type.range;
        eatCooldown = type.eatCooldown;
        
        Invoke("Groan", Random.Range(1f, 20f));
    }

    void Groan()
    {
        if (dead) return;
        source.PlayOneShot(groans[Random.Range(0, groans.Length)]);
        Invoke("Groan", Random.Range(10f, 25f));
    }

    private void FixedUpdate()
    {
        if (dead) return;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, range, plantMask);

        if (hit.collider != null)
        {
            targetPlant = hit.collider.GetComponent<Plant>();
            if (targetPlant && canEat)
            {
                Eat();
            }
            animator.SetBool("IsEating", true);
            return;
        }
        else
        {
            targetPlant = null;
            animator.SetBool("IsEating", false);
        }
        transform.position -= new Vector3(speed * Time.fixedDeltaTime, 0, 0);
    }

    void Eat()
    {
        canEat = false;
        if (chompClips.Length > 0)
        {
            AudioClip chomp = chompClips[Random.Range(0, chompClips.Length)];
            source.PlayOneShot(chomp);
        }
        targetPlant.Hit(damage);

        if (targetPlant != null && targetPlant.health <= 0)
        {
            Invoke("PlayGulp", 0.1f);
        }
        Invoke(nameof(ResetEatCooldown), eatCooldown);
    }

    private void PlayGulp()
    {
        if (gulpClip != null)
        {
            source.PlayOneShot(gulpClip);
        }
    }

    void ResetEatCooldown()
    {
        canEat = true;
    }

    public void Hit(int damage, bool freeze)
    {
        if (dead) return;

        source.PlayOneShot(type.hitClips[Random.Range(0, type.hitClips.Length)]);
        health -= damage;
        if(freeze)
        {
            Freeze();
        }
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        dead = true;
        animator.SetTrigger("Die");

        ZombieSpawner spawner = FindFirstObjectByType<ZombieSpawner>();
        if (spawner != null)
        {
            spawner.OnZombieKilled();
        }

        Destroy(gameObject, 1f);
    }

    void Freeze()
    {
        CancelInvoke("UnFreeze");

        if (!isFrozen && freezeClip != null)
        {
            source.PlayOneShot(freezeClip);
            isFrozen = true;
        }

        foreach (Renderer rend in allRenderers)
        {
            if (rend != null)
            {
                rend.material.color = new Color(0.2f, 0.5f, 0.9f);
            }
        }
        speed = type.speed / 2;
        Invoke("UnFreeze", 5);
    }

    void UnFreeze()
    {
        isFrozen = false;

        foreach (Renderer rend in allRenderers)
        {
            if (rend != null)
            {
                rend.material.color = Color.white;
            }
        }
        speed = type.speed;
    }
}

