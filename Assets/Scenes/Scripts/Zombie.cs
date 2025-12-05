using UnityEngine;
using System.Collections;

public class Zombie : MonoBehaviour, IStateMachine
{
    [Header("Stats")]
    public int health = 10;
    public int damage = 1;
    public float speed = 1f;
    public float attackCooldown = 1f;

    [Header("Settings")]
    public ZombieTypes type;
    public LayerMask plantMask; // Kept for reference, though using Triggers now

    [Header("Audio Clips")]
    public AudioClip[] chompClips;
    public AudioClip gulpClip;
    public AudioClip freezeClip;
    public AudioClip[] groans;

    [Header("Dependencies")]
    private AudioSource source;
    public Animator animator; // Made public for States or accessed via property
    private MaterialPropertyBlock propBlock;
    private Renderer[] allRenderers;

    // State Machine
    private IState _currentState;
    public ZombieWalkState StateWalk { get; private set; }
    public ZombieEatState StateEat { get; private set; }

    // Internal State
    public bool dead;
    private bool isFrozen = false;
    private int currentPhase = 0;
    private Color frozenColor = new Color(0.2f, 0.5f, 0.9f);
    private Color normalColor = Color.white;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        allRenderers = GetComponentsInChildren<Renderer>();
        propBlock = new MaterialPropertyBlock();

        StateWalk = new ZombieWalkState(this);
        StateEat = new ZombieEatState(this);
    }

    void Start()
    {
        if (type != null)
        {
            health = type.health;
            speed = type.speed;
            damage = type.damage;
            attackCooldown = type.eatCooldown;
        }

        UpdatePhase();
        animator.SetInteger("Phase", currentPhase);

        Invoke("Groan", Random.Range(1f, 20f));

        ChangeState(StateWalk);
    }

    void Update()
    {
        _currentState?.Execute();
    }

    public void ChangeState(IState newState)
    {
        if (_currentState == newState) return;

        _currentState?.Exit();
        _currentState = newState;
        _currentState?.Enter();
    }

    // --- Trigger Logic (Replaces Raycast) ---
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (dead) return;

        if (collider.CompareTag("Plant"))
        {
            if (collider.TryGetComponent(out Plant plant))
            {
                StateEat.SetTarget(plant);
                ChangeState(StateEat);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (dead) return;

        if (collider.CompareTag("Plant"))
        {
            if (_currentState == StateEat && StateEat.TargetPlant == collider.GetComponent<Plant>())
            {
                ChangeState(StateWalk);
            }
            // Note: If plant dies (destroyed), OnTriggerExit might not fire or target becomes null.
            // StateEat.Execute handles null target.
        }
    }

    // --- Combat & Damage ---
    public void Hit(int damageAmount, bool freeze)
    {
        if (dead) return;

        if (type != null && type.hitClips.Length > 0)
            source.PlayOneShot(type.hitClips[Random.Range(0, type.hitClips.Length)]);
        
        health -= damageAmount;

        UpdatePhase();
        animator.SetInteger("Phase", currentPhase);

        if (freeze)
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

    private void UpdatePhase()
    {
        if (type == null || type.health <= 0) return;

        float healthPercent = (float)health / type.health;

        if (healthPercent > 0.66f)
            currentPhase = 0;
        else if (healthPercent > 0.33f)
            currentPhase = 1;
        else
            currentPhase = 2;
    }

    // --- Audio ---
    void Groan()
    {
        if (dead) return;
        if (groans != null && groans.Length > 0)
            source.PlayOneShot(groans[Random.Range(0, groans.Length)]);
        Invoke("Groan", Random.Range(10f, 25f));
    }

    public void PlayChomp()
    {
        if (chompClips != null && chompClips.Length > 0)
            source.PlayOneShot(chompClips[Random.Range(0, chompClips.Length)]);
    }

    public void PlayGulp()
    {
        if (gulpClip != null)
            source.PlayOneShot(gulpClip);
    }

    // --- Freeze Logic ---
    void Freeze()
    {
        if (isFrozen) return;
        isFrozen = true;

        CancelInvoke("UnFreeze");

        if (!isFrozen && freezeClip != null)
        {
            source.PlayOneShot(freezeClip);
        }

        allRenderers = GetComponentsInChildren<Renderer>(true);

        foreach (Renderer rend in allRenderers)
        {
            if (rend != null)
            {
                rend.GetPropertyBlock(propBlock);
                propBlock.SetColor("_Color", frozenColor);
                propBlock.SetColor("_BaseColor", frozenColor);
                rend.SetPropertyBlock(propBlock);
            }
        }
        
        // Reduce speed
        speed = (type != null ? type.speed : speed) / 2;
        
        Invoke("UnFreeze", 5);
    }

    void UnFreeze()
    {
        isFrozen = false;
        allRenderers = GetComponentsInChildren<Renderer>(true);

        foreach (Renderer rend in allRenderers)
        {
            if (rend != null)
            {
                rend.GetPropertyBlock(propBlock);
                propBlock.SetColor("_Color", normalColor);
                propBlock.SetColor("_BaseColor", normalColor);
                rend.SetPropertyBlock(propBlock);
            }
        }
        
        // Restore speed
        speed = (type != null ? type.speed : speed);
    }
}

// --- States ---

public class ZombieWalkState : IState
{
    private readonly Zombie _sm;

    public ZombieWalkState(Zombie stateMachine)
    {
        _sm = stateMachine;
    }

    public void Enter() 
    {
        if (_sm.animator != null)
            _sm.animator.SetBool("IsEating", false);
    }

    public void Execute()
    {
        if (_sm.dead) return;
        // Move left
        _sm.transform.Translate(Vector3.left * _sm.speed * Time.deltaTime);
    }

    public void Exit() { }
}

public class ZombieEatState : IState
{
    private readonly Zombie _sm;
    public Plant TargetPlant { get; private set; }
    private float _currentTimer;

    public ZombieEatState(Zombie stateMachine)
    {
        _sm = stateMachine;
    }

    public void SetTarget(Plant plant)
    {
        TargetPlant = plant;
    }

    public void Enter()
    {
        if (_sm.animator != null)
            _sm.animator.SetBool("IsEating", true);
        
        _currentTimer = 0f; 
    }

    public void Execute()
    {
        if (_sm.dead) return;

        // If plant died (null or destroyed)
        if (TargetPlant == null || TargetPlant.health <= 0) // Assuming Plant has health check
        {
            _sm.ChangeState(_sm.StateWalk);
            return;
        }

        _currentTimer += Time.deltaTime;

        if (_currentTimer >= _sm.attackCooldown)
        {
            _sm.PlayChomp();
            TargetPlant.Hit(_sm.damage);
            
            if (TargetPlant.health <= 0)
            {
                 _sm.PlayGulp();
            }

            _currentTimer = 0f;
        }
    }

    public void Exit()
    {
        TargetPlant = null;
    }
}
