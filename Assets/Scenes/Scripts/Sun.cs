using UnityEngine;
using UnityEngine.InputSystem;

public class Sun : MonoBehaviour
{

    public float dropToYPos;
    private float speed = 2f;
    private void Start()
    {

        Destroy(gameObject, Random.Range(6, 12));
    }

    private void Update()
    {
        if (transform.position.y > dropToYPos)
            transform.position -= new Vector3(0, speed * Time.deltaTime, 0);
    }
}
