using UnityEngine;
using UnityEngine.SceneManagement;

public class Lose : MonoBehaviour
{
    public Animator animator;
    private static readonly int ShowDeath = Animator.StringToHash("ShowDeath");
    private bool hasLost;
    public AudioClip loseMusic;
    public AudioClip scream;
    public AudioSource music;
    private AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        if (animator == null)
        {
            return;
        }

        animator.SetBool(ShowDeath, false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    { 
        if(collision.CompareTag("Zombie"))
        {
            if (hasLost || collision.GetComponent<Zombie>().dead)
                return;
            hasLost = true;
            source.PlayOneShot(loseMusic);
            source.PlayOneShot(scream);
            music.Stop();
            animator.SetBool(ShowDeath, true);
            Invoke("RestartScene", 5f);
        }
    }

    void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
