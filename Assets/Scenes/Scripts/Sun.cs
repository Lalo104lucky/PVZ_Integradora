using UnityEngine;
using UnityEngine.InputSystem;

public class Sun : MonoBehaviour
{

    private float dropToYPos;
    private float speed = 2f;
    private void Start()
    {
        transform.position = new Vector3(Random.Range(-4.8f, 6f), 6f, 0f);
        dropToYPos = Random.Range(2f, -3f);
        Destroy(gameObject, Random.Range(6, 12));
    }

    private void Update()
    {
        if (transform.position.y >= dropToYPos)
            transform.position -= new Vector3(0, speed * Time.deltaTime, 0);
    }
}
