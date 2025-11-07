using UnityEngine;

public class Plant : MonoBehaviour
{

    public int health;

    private void Start()
    {
        gameObject.layer = 10;
    }

    public void Hit(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
