using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName ="New ZombieType", menuName ="Zombie")]
public class ZombieTypes : ScriptableObject
{
    public int health;

    public float speed;

    public int damage;

    public float range = .5f;

    public float eatCooldown = 1f;

    public Sprite sprite;

    public Sprite deathSprite;

    public AudioClip[] hitClips;
    
}
