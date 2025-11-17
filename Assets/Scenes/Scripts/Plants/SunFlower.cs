using UnityEngine;

public class SunFlower : MonoBehaviour
{

    [Header("Settings")]
    public GameObject sunObject;
    public float cooldown;

    [Header("Animation")]
    public Animator animator;
    public SpriteRenderer headSprite;
    public SpriteRenderer headSprite1;

    private static readonly int SpawningSun = Animator.StringToHash("SpawningSun");
    private bool canSpawn = true;

    private void Start()
    {
        animator.SetBool(SpawningSun, false);
        canSpawn = true;
        Invoke(nameof(SpawnSunSequence), cooldown);
    }

    void SpawnSunSequence()
    {
        if (!canSpawn) return;
        canSpawn = false;
        animator.SetBool(SpawningSun, true);
    }

    public void SpawnSun()
    {
        Vector3 spawnPosition = new Vector3(transform.position.x + Random.Range(-.5f, .5f), transform.position.y + Random.Range(0f, .5f), 0);
        GameObject mySun = Instantiate(sunObject, spawnPosition, Quaternion.identity);
        mySun.GetComponent<Sun>().dropToYPos = transform.position.y - 0.5f;
    }

    public void EndSpawn()
    {
        animator.SetBool(SpawningSun, false);
        canSpawn = true;
        Invoke(nameof(SpawnSunSequence), cooldown);
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
