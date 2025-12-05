using UnityEngine;

public class SunFlower : MonoBehaviour, IStateMachine
{
    [Header("Settings")]
    public GameObject sunObject;
    public float cooldown = 5f;

    [Header("Animation")]
    public Animator animator;
    public SpriteRenderer headSprite;
    public SpriteRenderer headSprite1;

    // State Machine
    private IState _currentState;
    public SunFlowerIdleState StateIdle { get; private set; }
    public SunFlowerProducingState StateProducing { get; private set; }

    // Pooling
    private GameObject _currentSun;
    public int AnimID_SpawningSun { get; private set; }

    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();

        AnimID_SpawningSun = Animator.StringToHash("SpawningSun");

        StateIdle = new SunFlowerIdleState(this);
        StateProducing = new SunFlowerProducingState(this);

        ChangeState(StateIdle);
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

    // Called by Animation Event
    public void SpawnSun()
    {
        Vector3 spawnPosition = new Vector3(transform.position.x + Random.Range(-.5f, .5f), transform.position.y + Random.Range(0f, .5f), 0);
        float targetY = transform.position.y - 0.5f;

        if (_currentSun == null)
        {
            _currentSun = Instantiate(sunObject, spawnPosition, Quaternion.identity);
            if (_currentSun.TryGetComponent(out Sun sunScript))
            {
                sunScript.dropToYPos = targetY;
            }
        }
        else
        {
            // Reuse existing sun
            if (_currentSun.TryGetComponent(out Sun sunScript))
            {
                sunScript.ResetSun(spawnPosition, targetY);
            }
            else
            {
                // Fallback if script missing
                _currentSun.transform.position = spawnPosition;
                _currentSun.SetActive(true);
            }
        }
    }

    // Called by Animation Event (at end of anim)
    public void EndSpawn()
    {
        ChangeState(StateIdle);
    }

    public void StartGlow()
    {
        if (headSprite) headSprite.color = new Color(1.8f, 1.6f, 0.8f);
        if (headSprite1) headSprite1.color = new Color(1.8f, 1.6f, 0.8f);
    }

    public void StopGlow()
    {
        if (headSprite) headSprite.color = Color.white;
        if (headSprite1) headSprite1.color = Color.white;
    }
}

public class SunFlowerIdleState : IState
{
    private readonly SunFlower _sm;
    private float _timer;

    public SunFlowerIdleState(SunFlower sm)
    {
        _sm = sm;
    }

    public void Enter()
    {
        _sm.animator.SetBool(_sm.AnimID_SpawningSun, false);
        _timer = 0f;
    }

    public void Execute()
    {
        _timer += Time.deltaTime;
        if (_timer >= _sm.cooldown)
        {
            _sm.ChangeState(_sm.StateProducing);
        }
    }

    public void Exit() { }
}

public class SunFlowerProducingState : IState
{
    private readonly SunFlower _sm;

    public SunFlowerProducingState(SunFlower sm)
    {
        _sm = sm;
    }

    public void Enter()
    {
        _sm.animator.SetBool(_sm.AnimID_SpawningSun, true);
    }

    public void Execute()
    {
        // Waiting for Animation Events (SpawnSun, EndSpawn)
    }

    public void Exit() { }
}
