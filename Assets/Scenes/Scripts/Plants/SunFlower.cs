using UnityEngine;

public class SunFlower : MonoBehaviour
{

    [Header("Settings")]
    public GameObject sunObject;
    public float cooldown;

    [Header("Animation")]
    public Animator animator;
    public SpriteRenderer headSprite;

    private static readonly int isSpawningSun = Animator.StringToHash("isSpawningSun");

    private void Start()
    {
        InvokeRepeating("SpawnSunSequence", cooldown, cooldown);
    }

    void SpawnSunSequence()
    {
        animator.SetTrigger(isSpawningSun);

        Invoke(nameof(SpawnSun), 0.8f);
    }

    void SpawnSun()
    {
        Vector3 spawnPosition = new Vector3(transform.position.x + Random.Range(-.5f, .5f), transform.position.y + Random.Range(0f, .5f), 0);
        GameObject mySun = Instantiate(sunObject, spawnPosition, Quaternion.identity);
        mySun.GetComponent<Sun>().dropToYPos = transform.position.y - 0.5f;
    }
}
