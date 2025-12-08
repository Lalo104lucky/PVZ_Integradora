using System.Collections;
using UnityEngine;

public class Papapum : MonoBehaviour
{
   [Header("Tiempos")]
    public float tiempoParaSalir = 14f; 
    public float tiempoDuracionExplosion = 1f; 

    private Animator animator;
    private BoxCollider2D miCollider; 

    void Start()
    {
        animator = GetComponent<Animator>();
        miCollider = GetComponent<BoxCollider2D>(); // Asegúrate de usar el collider correcto

        // 1. Al inicio, desactivamos el collider para que los zombies no la toquen
        if (miCollider != null)
        {
            miCollider.enabled = false;
        }

        // Iniciamos la cuenta regresiva
        StartCoroutine(RutinaArmado());
    }

    IEnumerator RutinaArmado()
    {
        // Esperamos el tiempo bajo tierra
        yield return new WaitForSeconds(tiempoParaSalir);

        // Activamos la animacion de salir (Idle)
        // El animator pasará solo de 'Idle' a 'Move' gracias al "Exit Time" que configuramos
        animator.SetBool("Listo", true);

        // Esperamos un poquitito (ej. 1 seg) lo que dura la animación de salir 'Idle'
        // para activar el collider justo cuando ya esté arriba bailando 'Move'
        // Ajusta este tiempo según lo que dure tu animación 'PapapumIdle'
        yield return new WaitForSeconds(1.0f); 

        if (miCollider != null)
        {
            miCollider.enabled = true;
            miCollider.isTrigger = true; // Importante para detectar cruce sin chocar fisico
        }
    }

    // Esta función detecta cuando algo entra en el collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificamos si lo que tocó es un Zombie
        // IMPORTANTE: Tu zombie debe tener el Tag "Zombie"
        if (collision.CompareTag("Zombie"))
        {
            StartCoroutine(RutinaExplosion(collision.gameObject));
        }
    }

    IEnumerator RutinaExplosion(GameObject zombie)
    {
        // 1. Disparamos la animación de explosión
        animator.SetTrigger("Explotar");

        // 2. Destruimos al zombie inmediatamente
        Destroy(zombie);

        // 3. Esperamos a que termine la animación de explosión de la papa
        yield return new WaitForSeconds(tiempoDuracionExplosion);

        // 4. Destruimos la papa
        Destroy(gameObject);
    }
}

