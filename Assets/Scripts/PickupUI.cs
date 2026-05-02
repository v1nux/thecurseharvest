using UnityEngine;
using TMPro;
using System.Collections;

public class PickupUI : MonoBehaviour
{
    public static PickupUI Instance;

    [SerializeField] private TextMeshProUGUI pickupText;

    private ItemDictionary itemDictionary;

    void Awake()
    {
        Instance = this;
        pickupText.text = "";

        itemDictionary = FindFirstObjectByType<ItemDictionary>();
    }

    public void ShowPickup(int itemID, int amount)
    {
        StopAllCoroutines();
        StartCoroutine(ShowText(itemID, amount));
    }

    IEnumerator ShowText(int itemID, int amount)
    {
        string itemName = "Unknown";

        if (itemDictionary != null)
        {
            GameObject prefab = itemDictionary.GetItemPrefab(itemID);

            if (prefab != null)
            {
                Item item = prefab.GetComponent<Item>();
                if (item != null)
                    itemName = item.Name;
            }
        }

        pickupText.text = itemName + " =" + amount;

        yield return new WaitForSeconds(1.5f);

        pickupText.text = "";
    }
}