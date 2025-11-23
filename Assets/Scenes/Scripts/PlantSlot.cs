using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlantSlot : MonoBehaviour
{
    public Sprite plantSprite;
    public GameObject plantObject;
    public int price;
    public Image icon;
    public TextMeshProUGUI priceText;

    [Header("Audio")]
    public AudioClip seedLiftSound;

    private GameManager gms;

    private void Start()
    {
        gms = GameObject.Find("GameManager").GetComponent<GameManager>();
        Button btn = GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(SelectThisPlant);
    }

    private void SelectThisPlant()
    {
        if (gms == null || plantObject == null) return;

        if (seedLiftSound != null)
        {
            AudioSource.PlayClipAtPoint(seedLiftSound, Camera.main.transform.position, 1f);
        }

        gms.SelectPlant(plantObject, plantSprite, price, this);

        if (gms.suns < price)
        {
            gms.NotEnoughSuns();
        }
    }

    private void OnValidate()
    {
        if (icon != null && priceText != null)
        {
            if (plantSprite != null)
            {
                icon.enabled = true;
                icon.sprite = plantSprite;
                priceText.text = price.ToString();
            }
            else
            {
                icon.enabled = false;
                priceText.text = "";
            }
        }
    }
}