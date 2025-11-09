using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject currentPlant;
    public Sprite currentPlantSprite;
    public Transform tiles;
    public LayerMask tileMask;
    public int suns;
    public TextMeshProUGUI sunText;
    public LayerMask sunMask;
    public AudioClip plantSFX;
    private AudioSource source;

    public AudioSource sunSource;
    public AudioClip sunClip;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void BuyPlant(GameObject plant, Sprite sprite)
    {
        currentPlant = plant;
        currentPlantSprite = sprite;
    }

    private void Update()
    {
        sunText.text = suns.ToString();

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        if (!Camera.main.pixelRect.Contains(mousePosition))
            return;

        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(mousePosition);
        worldPoint.z = 0;

        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, tileMask);

        foreach (Transform tile in tiles)
            tile.GetComponent<SpriteRenderer>().enabled = false;

        if (hit.collider && currentPlant)
        {
            hit.collider.GetComponent<SpriteRenderer>().sprite = currentPlantSprite;
            hit.collider.GetComponent<SpriteRenderer>().enabled = true;

            if (Mouse.current.leftButton.wasPressedThisFrame && !hit.collider.GetComponent<Tile>().hasPlant)
            {
                Plant(hit.collider.gameObject);
            }
        }

        RaycastHit2D sunHit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, sunMask);

        if (sunHit.collider && Mouse.current.leftButton.wasPressedThisFrame)
        {
            sunSource.pitch = Random.Range(0.9f, 1.1f);
            sunSource.PlayOneShot(sunClip);
            suns += 25;
            Destroy(sunHit.collider.gameObject);
        }

    }

    void Plant(GameObject hit)
    {
        source.PlayOneShot(plantSFX);
        Instantiate(currentPlant, hit.transform.position, Quaternion.identity);
        hit.GetComponent<Tile>().hasPlant = true;
        currentPlant = null;
        currentPlantSprite = null;
    }
}
