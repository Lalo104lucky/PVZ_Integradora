using UnityEngine;

public class Plant : MonoBehaviour
{

    public int health;
    public Tile myTile;
    public double damage = 5;

    public double range = 5;

    public GameObject plantUp;

    private void Start()
    {
        gameObject.layer = 10;
    }

    public void Hit(int damage)
    {
        health -= damage;
        var wallnut = GetComponent<Wallnut>();
        if (wallnut != null)
            wallnut.OnPlantHit();
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (myTile != null)
        {
            myTile.hasPlant = false;
        }
        Destroy(gameObject);
    }
}
