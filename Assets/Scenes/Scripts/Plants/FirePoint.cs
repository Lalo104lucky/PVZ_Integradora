using UnityEngine;

// Asegura que el GameObject tenga el componente necesario
[RequireComponent(typeof(BoxCollider2D))]
public class FirePoint : MonoBehaviour
{
    [Header("Status")]
    // Variable leída por la StateMachine
    public bool zombieInRange = false;

    [Header("Settings")]
    // Unity usa float para físicas. Double genera conversiones innecesarias.
    public float range = 0f;

    private BoxCollider2D _boxCollider2D;
    private int _zombiesInsideCount = 0; // Contador de zombies

    void Awake()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
        
        // Aseguramos que sea Trigger para no chocar físicamente
        _boxCollider2D.isTrigger = true; 
    }

    void Start()
    {
        // Configuramos el tamaño del collider basado en el rango recibido
        // Nota: Asegúrate de que el 'offset' del collider en el editor esté bien puesto
        // para que crezca hacia adelante y no solo desde el centro.
        _boxCollider2D.size = new Vector2(range, 0.1f); // 0.1f es más visible en el editor que 0.001f
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Zombie"))
        {
            _zombiesInsideCount++;
            UpdateStatus();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Zombie"))
        {
            _zombiesInsideCount--;
            UpdateStatus();
        }
    }

    // Método para actualizar el booleano basado en el contador
    private void UpdateStatus()
    {
        // Si el contador baja de 0 por algún error raro, lo reseteamos a 0
        if (_zombiesInsideCount < 0) _zombiesInsideCount = 0;

        // Si hay 1 o más zombies, es true. Si es 0, es false.
        zombieInRange = _zombiesInsideCount > 0;
    }
}