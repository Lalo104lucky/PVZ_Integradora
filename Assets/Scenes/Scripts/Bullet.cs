using UnityEngine;

// Corregí el nombre de la clase: Ballet -> Bullet
// Renamed NewBullet -> Bullet to match filename
public class Bullet : MonoBehaviour, IStateMachine
{
    [Header("Settings")]
    // Unity usa float, no double. Usar double causa conversiones innecesarias.
    public float damage = 2f; 
    public float speed = 12f;
    public bool freeze = false;
    
    // Variable de control
    public bool isShooting = false;

    public Transform plantTransform;

    [Header("Dependencies")]
    [SerializeField] private Rigidbody2D _rb;

    private IState _currentState;

    // Cache de estados para evitar basura en memoria (Garbage Collection)
    public BulletIdleState StateIdle { get; private set; }
    public BulletShootingState StateShooting { get; private set; }

    // Propiedad pública para que los estados accedan al Rigidbody sin GetComponent
    public Rigidbody2D Rb => _rb;

    void Awake()
    {
        if (_rb == null) _rb = GetComponent<Rigidbody2D>();

        // Inicializamos estados una sola vez
        StateIdle = new BulletIdleState(this);
        StateShooting = new BulletShootingState(this);

        // Estado inicial
        ChangeState(isShooting ? StateShooting : StateIdle);
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

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Usar CompareTag es eficiente
        if (collision.CompareTag("Zombie"))
        {
            // TryGetComponent es más seguro y rápido si el zombie no tiene el script
            // Adapted: ZombieStateMachine -> Zombie to match existing class
            if (collision.TryGetComponent(out Zombie zombie))
            {
                // Adapted: TakeDamage -> Hit to match existing method signature
                // Casting float damage to int as Zombie.Hit expects int
                zombie.Hit((int)damage, freeze);
                
                // Si tiene efecto de hielo, aquí iría la lógica:
                // if (freeze) zombie.ApplyFreeze(); // Note: Hit already handles freeze
            }

            // Lógica de reciclaje:
            ResetBullet();
        }
        else if (collision.CompareTag("Limit")) // Opcional: Destruir/Resetear si sale de pantalla
        {
            ResetBullet();
        }
    }

    // Método helper para resetear la bala
    private void ResetBullet()
    {
        isShooting = false; // Apagar la señal de disparo
        ChangeState(StateIdle); // Cambiar estado
        if (plantTransform != null)
        {
            transform.position = plantTransform.position; // Regresar a la planta
        }
        else
        {
            // Fallback if plantTransform is missing (e.g. destroyed)
            // If we are pooling, we might just want to hide it, but if plant is gone, maybe destroy?
            // User said "vuelva al firepoint", implying plant exists. 
            // If plant is destroyed, the bullet might be orphaned. 
            // For now, let's just hide it.
        }
        gameObject.SetActive(false);
    }
}

// Usamos CLASS en lugar de STRUCT para evitar Boxing
// Renamed NewBulletIdleState -> BulletIdleState
public class BulletIdleState : IState
{
    private readonly Bullet _sm;
    private float speed = 7f;

    public BulletIdleState(Bullet stateMachine)
    {
        _sm = stateMachine;
    }

    public void Enter()
    {
        // Al entrar en Idle, aseguramos que la bala se detenga por completo
        if (_sm.Rb != null)
        {
            // Nota: En Unity 6 se usa 'linearVelocity', en versiones anteriores 'velocity'
            // Using velocity for compatibility if linearVelocity is not available, 
            // but user code used linearVelocity. I'll stick to linearVelocity if user is on Unity 6,
            // but standard Unity 2022/2023 uses velocity. 
            // Checking user code context... User provided 'linearVelocity'. 
            // If this fails to compile, user might be on older Unity. 
            // I will use 'velocity' as it is safer for most versions unless I know for sure.
            // Wait, user explicitly wrote: // Nota: En Unity 6 se usa 'linearVelocity'
            // I should respect that if they are on Unity 6. 
            // However, 'velocity' is deprecated in 6 but still works? Or removed?
            // Let's check the previous file content. It used 'linearVelocity'.
            // Wait, previous file content:
            // 14:             rb.linearVelocity = Vector2.right * speed;
            // So the user IS using linearVelocity. I will keep it.
            _sm.Rb.linearVelocity = Vector2.right * speed; 
            _sm.Rb.angularVelocity = 0f;
        }
    }

    public void Execute()
    {
        // Esperamos la señal de disparo
        if (_sm.isShooting)
        {
            _sm.ChangeState(_sm.StateShooting);
        }
    }

    public void Exit() { }
}

// Renamed NewBulletShootingState -> BulletShootingState
public class BulletShootingState : IState
{
    private readonly Bullet _sm;

    public BulletShootingState(Bullet stateMachine)
    {
        _sm = stateMachine;
    }

    public void Enter()
    {
        // Al entrar, aplicamos la velocidad
        if (_sm.Rb != null)
        {
            // Aplicamos la velocidad hacia la derecha relativa al mundo o local
            // Asumiendo juego 2D lateral:
            _sm.Rb.linearVelocity = Vector2.right * _sm.speed;
        }
    }

    public void Execute()
    {
        // Aquí podrías poner lógica de rotación si la bala girara, etc.
    }

    public void Exit()
    {
        // Al salir del estado de disparo (chocó), no necesitamos hacer nada específico aquí
        // porque el estado Idle se encargará de frenar la bala en su 'Enter'.
    }
}