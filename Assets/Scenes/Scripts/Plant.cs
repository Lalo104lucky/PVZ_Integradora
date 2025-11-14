using UnityEngine;

public class Plant : MonoBehaviour
{

    public int health;
    public Tile myTile;

    private void Start()
    {
        gameObject.layer = 10;
    }

    public void Hit(int damage)
    {
        health -= damage;
        if(health <= 0)
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
