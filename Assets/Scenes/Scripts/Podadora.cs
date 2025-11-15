using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;

public class Podadora : MonoBehaviour
{
    public bool isMoving;

    public float speed = 1;

    public AudioClip sound;

    private AudioSource source;

    private void Start()
    {
        source = gameObject.AddComponent<AudioSource>();   
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Zombie"))
        {
            if(!isMoving)
            {
                source.PlayOneShot(sound);
            }
            collision.GetComponent<Zombie>().Hit(100, false);

            isMoving = true;
            Destroy(gameObject, 8);
        }
    }

    private void Update()
    {
        if(isMoving)
        {
            transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
        }
    }
}
