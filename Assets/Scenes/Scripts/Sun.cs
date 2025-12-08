using UnityEngine;
using UnityEngine.InputSystem;

public class Sun : MonoBehaviour
{

    public float dropToYPos;
    public int sunValue = 25;
    private float speed = 1f;

    public bool isPooled = false;

    private void OnEnable()
    {
        // Auto-hide after random time instead of Destroy
        Invoke(nameof(HideSun), Random.Range(6f, 12f));
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void Update()
    {
        if (transform.position.y > dropToYPos)
            transform.position -= new Vector3(0, speed * Time.deltaTime, 0);
    }

    public void ResetSun(Vector3 startPos, float targetY)
    {
        transform.position = startPos;
        dropToYPos = targetY;
        gameObject.SetActive(true);
    }

    public void Collect()
    {
        if (isPooled)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void HideSun()
    {
        if (isPooled)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
