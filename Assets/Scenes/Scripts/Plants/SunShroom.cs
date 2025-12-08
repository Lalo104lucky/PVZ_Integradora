using UnityEngine;

public class SunShroom : MonoBehaviour
{
    [Header("Ajustes PvZ")]
    public GameObject sunPrefab;   // Arrastra aquí tu prefab del Sol
    public float cooldown = 24f;   // En PvZ tarda aprox 24s en dar sol
    public float tiempoParaCrecer = 120f; // En PvZ tarda 2 min en crecer
    
    [Header("Animación")]
    public Animator animator;

    // Variables internas
    private bool haCrecido = false;
    private float timer;

    // IDs para optimizar el Animator
    private static readonly int IsGrownParam = Animator.StringToHash("isGrown");

    void Start()
    {
        // Iniciamos el timer en un valor aleatorio para que no todas den sol al mismo tiempo
        timer = Random.Range(0, 5f);
        
        // Programamos el crecimiento
        Invoke(nameof(Crecer), tiempoParaCrecer);
    }

    void Update()
    {
        // Contador para generar sol
        timer += Time.deltaTime;

        if (timer >= cooldown)
        {
            SpawnSun();
            timer = 0; // Reiniciar contador
        }
    }

    void Crecer()
    {
        if (haCrecido) return; // Si ya creció, no hacemos nada

        haCrecido = true;
        Debug.Log("¡La Seta Solar ha crecido!");

        if (animator != null)
        {
            // Activamos la transición a la animación de crecer
            animator.SetBool(IsGrownParam, true);
        }
    }

    void SpawnSun()
    {
        // Posición un poco aleatoria para que no salga siempre en el mismo pixel
        Vector3 spawnPosition = new Vector3(
            transform.position.x + Random.Range(-0.3f, 0.3f), 
            transform.position.y + Random.Range(0.1f, 0.4f), 
            0
        );
        
        GameObject mySun = Instantiate(sunPrefab, spawnPosition, Quaternion.identity);

        // Buscamos el script del Sol para asignarle el valor
        Sun sunScript = mySun.GetComponent<Sun>();

        if (sunScript != null)
        {
            // Configura hasta donde cae el sol (efecto visual)
            sunScript.dropToYPos = transform.position.y - 0.5f;

            // LÓGICA CLAVE DE LA SETA SOLAR:
            if (haCrecido)
            {
                sunScript.sunValue = 25; // Grande = Sol normal
                // (Opcional) Si la seta grande es amarilla, puedes cambiar el color del sol aquí si quisieras
            }
            else
            {
                sunScript.sunValue = 15; // Pequeña = Sol pequeño
                // Aquí podrías cambiar la escala del sol para que se vea pequeñito
                mySun.transform.localScale = new Vector3(0.6f, 0.6f, 1f); 
            }
        }
    }
}