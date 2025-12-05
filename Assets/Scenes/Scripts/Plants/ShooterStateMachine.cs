using UnityEngine;

public class ShooterPlantStateMachine : MonoBehaviour, IStateMachine
{
    [Header("Shooting Stats")]
    public GameObject bulletPrefab;
    public Transform shootOrigin;
    public float range = 10f;
    public LayerMask shootMask;

    [Header("Animation & Audio")]
    public Animator animator;
    public AudioClip[] shootClips;
    private AudioSource _audioSource;

    // Hash para optimizar animación
    public int AnimID_IsAttacking { get; private set; }

    private IState _currentState;

    public ShooterIdleState StateIdle { get; private set; }
    public ShooterAttackState StateAttack { get; private set; }

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        if (animator == null) animator = GetComponent<Animator>();
        if (shootOrigin == null) shootOrigin = transform;

        AnimID_IsAttacking = Animator.StringToHash("isAttacking");

        StateIdle = new ShooterIdleState(this);
        StateAttack = new ShooterAttackState(this);

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

    // --- Lógica de Detección ---
    public bool CheckForEnemy()
    {
        RaycastHit2D hit = Physics2D.Raycast(shootOrigin.position, Vector2.right, range, shootMask);
        return hit.collider != null;
    }

    // ---------------------------------------------------------
    // ¡AQUÍ ESTÁ LA SOLUCIÓN AL ERROR!
    // Esta función es llamada automáticamente por el Evento de la Animación
    // ---------------------------------------------------------
    // Variable para almacenar la única instancia de la bala
    private GameObject _currentBullet;

    // ---------------------------------------------------------
    // ¡AQUÍ ESTÁ LA SOLUCIÓN AL ERROR!
    // Esta función es llamada automáticamente por el Evento de la Animación
    // ---------------------------------------------------------
    public void Shoot()
    {
        // 1. Instanciar bala si no existe
        if (_currentBullet == null && bulletPrefab != null)
        {
            _currentBullet = Instantiate(bulletPrefab, shootOrigin.position, Quaternion.identity);
            if (_currentBullet.TryGetComponent(out Bullet bulletScript))
            {
                bulletScript.plantTransform = shootOrigin;
            }
        }
        else if (_currentBullet != null)
        {
            // 2. Si ya existe, reutilizarla
            _currentBullet.transform.position = shootOrigin.position;
            _currentBullet.SetActive(true);
            
            // Reiniciar estado de la bala si tiene el componente Bullet
            if (_currentBullet.TryGetComponent(out Bullet bulletScript))
            {
                // Forzamos el estado de disparo
                bulletScript.isShooting = true;
                bulletScript.ChangeState(bulletScript.StateShooting);
            }
        }

        // 3. Sonido
        if (_audioSource != null && shootClips != null && shootClips.Length > 0)
        {
            _audioSource.PlayOneShot(shootClips[Random.Range(0, shootClips.Length)]);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (shootOrigin != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(shootOrigin.position, shootOrigin.position + Vector3.right * range);
        }
    }
}// Estado IDLE (Sin cambios)
public class ShooterIdleState : IState
{
    private readonly ShooterPlantStateMachine _sm;

    public ShooterIdleState(ShooterPlantStateMachine sm) => _sm = sm;

    public void Enter()
    {
        if(_sm.animator != null) 
            _sm.animator.SetBool(_sm.AnimID_IsAttacking, false);
    }

    public void Execute()
    {
        if (_sm.CheckForEnemy())
        {
            _sm.ChangeState(_sm.StateAttack);
        }
    }

    public void Exit() { }
}

// Estado ATTACK (Modificado para usar el Evento de Animación)
public class ShooterAttackState : IState
{
    private readonly ShooterPlantStateMachine _sm;

    public ShooterAttackState(ShooterPlantStateMachine sm) => _sm = sm;

    public void Enter()
    {
        // Al poner esto en TRUE, la animación empieza a reproducirse (y a loopear).
        // Cada vez que la animación pase por el frame del evento, llamará a Shoot().
        if (_sm.animator != null) 
            _sm.animator.SetBool(_sm.AnimID_IsAttacking, true);
    }

    public void Execute()
    {
        // Nuestra única responsabilidad ahora es verificar si el enemigo se fue.
        // Si el enemigo sigue ahí, mantenemos el estado y la animación sigue en bucle disparando sola.
        if (!_sm.CheckForEnemy())
        {
            _sm.ChangeState(_sm.StateIdle);
        }
    }

    public void Exit()
    {
        // Al salir, apagamos la animación para que deje de disparar
        if (_sm.animator != null) 
            _sm.animator.SetBool(_sm.AnimID_IsAttacking, false);
    }
}