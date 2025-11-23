using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public GameObject currentPlant;
    public Sprite currentPlantSprite;
    public int currentPlantPrice;
    public PlantSlot currentSlot;
    public Transform tiles;
    public LayerMask tileMask;
    public int suns;
    public TextMeshProUGUI sunText;
    public LayerMask sunMask;

    public AudioClip plantSFX;
    private AudioSource source;

    public AudioSource sunSource;
    public AudioClip sunClip;

    public AudioClip buzzerClip;
    private AudioSource buzzerSource;
    private Color originalSunTextColor;
    private Coroutine flashCoroutine;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        buzzerSource = source;
        if (sunText != null)
            originalSunTextColor = sunText.color;
        UpdateSunText();
    }

    public void Win()
    {
        int nextLevel = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextLevel >= SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(0);
            return;
        }

        PlayerPrefs.SetInt("levelSave", nextLevel);
        SceneManager.LoadScene(nextLevel);
    }

    public void SelectPlant(GameObject plantPrefab, Sprite sprite, int price, PlantSlot slot)
    {

        if (currentSlot != null && currentSlot != slot)
        {
            Image prevImg = currentSlot.GetComponent<Image>();
            if (prevImg != null)
                prevImg.color = Color.white;
        }

        currentPlant = plantPrefab;
        currentPlantSprite = sprite;
        currentPlantPrice = price;
        currentSlot = slot;

        if (slot != null)
        {
            Image img = slot.GetComponent<Image>();
            if (img != null)
                img.color = new Color(1f, 1f, 0.7f);
        }
    }

    private void Update()
    {
        UpdateSunText();

        if (Mouse.current == null) return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        if (!Camera.main.pixelRect.Contains(mousePosition)) return;

        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(mousePosition);
        worldPoint.z = 0;

        if (tiles != null)
        {
            foreach (Transform t in tiles)
            {
                SpriteRenderer sr = t.GetComponent<SpriteRenderer>();
                if (sr != null) sr.enabled = false;
            }
        }

        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, tileMask);

        foreach (Transform tile in tiles)
            tile.GetComponent<SpriteRenderer>().enabled = false;

        if (hit.collider != null && currentPlant != null)
        {
            SpriteRenderer sr = hit.collider.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = currentPlantSprite;
                sr.enabled = true;
            }

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Tile tileComponent = hit.collider.GetComponent<Tile>();
                if (tileComponent != null && !tileComponent.hasPlant)
                {
                    Plant(hit.collider.gameObject);
                }
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
        if (hit == null || currentPlant == null) return;
        if (suns < currentPlantPrice)
        {
            NotEnoughSuns();
            return;
        }
        Debug.Log("Soles totales " + suns + " Planta que se quiere sembrar " + currentPlantPrice);

        suns -= currentPlantPrice;
        UpdateSunText();

        source.PlayOneShot(plantSFX);

        GameObject plantObj = Instantiate(currentPlant, hit.transform.position, Quaternion.identity);
        Tile tile = hit.GetComponent<Tile>();
        tile.hasPlant = true;

        Plant plant = plantObj.GetComponent<Plant>();
        if(plant != null)
        {
            plant.myTile = tile;
        }

        currentPlant = null;
        currentPlantSprite = null;

        if (currentSlot != null)
        {
            Image img = currentSlot.GetComponent<Image>();
            if (img != null) img.color = Color.white;
        }
        currentSlot = null;
    }

    void UpdateSunText()
    {
        if (sunText) sunText.text = suns.ToString();
    }

    public void NotEnoughSuns()
    {
        if (buzzerSource != null && buzzerClip != null)
        {
            buzzerSource.pitch = 1f;
            buzzerSource.PlayOneShot(buzzerClip);
        }

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashSunText());
    }

    private IEnumerator FlashSunText()
    {
        if (sunText != null)
        {
            sunText.color = Color.red;
            yield return new WaitForSeconds(0.4f);
            sunText.color = originalSunTextColor;
        }
        flashCoroutine = null;
    }
}
