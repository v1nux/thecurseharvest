using UnityEngine;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour
{
    private ItemDictionary itemDictionary;

    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCount;
    public GameObject[] itemPrefabs;

    void Start()
    {
        itemDictionary = Object.FindFirstObjectByType<ItemDictionary>();
    }

    public List<InventorySaveData> GetInventoryItems()
    {
        List<InventorySaveData> invData = new List<InventorySaveData>();
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot == null)
            {
                Debug.LogWarning("Missing Slot component on: " + slotTransform.name);
                continue;
            }

            if (slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                if (item == null)  // Added: guard against missing Item component
                {
                    Debug.LogWarning("Missing Item component on: " + slot.currentItem.name);
                    continue;
                }

                invData.Add(new InventorySaveData { itemID = item.ID, slotIndex = slotTransform.GetSiblingIndex() });
            }
        }
        return invData;
    }

    public void AddItem(int itemID, int amount)
    {
        if (itemDictionary == null)
            itemDictionary = FindFirstObjectByType<ItemDictionary>();

        GameObject itemPrefab = itemDictionary.GetItemPrefab(itemID);

        if (itemPrefab == null)
        {
            Debug.LogError("No item prefab found for ID: " + itemID);
            return;
        }

        for (int i = 0; i < amount; i++)
        {
            foreach (Transform slotTransform in inventoryPanel.transform)
            {
                Slot slot = slotTransform.GetComponent<Slot>();

                if (slot != null && slot.currentItem == null)
                {
                    GameObject item = Instantiate(itemPrefab, slot.transform);

                    RectTransform rt = item.GetComponent<RectTransform>();
                    if (rt != null)
                    {
                        rt.anchoredPosition = Vector2.zero;
                        rt.localScale = Vector3.one;
                    }

                    slot.currentItem = item;
                    Debug.Log("Added item to inventory: " + itemID);
                    break;
                }
            }
        }
    }

    // fixed � was missing parameter name
   public void SetInventoryItems(List<InventorySaveData> inventorySaveData)
    {
        if (inventorySaveData == null || inventorySaveData.Count == 0)
        {
            Debug.Log("No saved inventory data — keeping default inventory.");
            return;
        }

        // Clear only ITEMS, not slots
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();

            if (slot != null && slot.currentItem != null)
            {
                Destroy(slot.currentItem);
                slot.currentItem = null;
            }
        }

        // Recreate slots ONLY if missing
        if (inventoryPanel.transform.childCount == 0)
        {
            for (int i = 0; i < slotCount; i++)
            {
                Instantiate(slotPrefab, inventoryPanel.transform);
            }
        }

        foreach (InventorySaveData data in inventorySaveData)
        {
            if (data.slotIndex >= inventoryPanel.transform.childCount) continue;

            Slot slot = inventoryPanel.transform
                .GetChild(data.slotIndex)
                .GetComponent<Slot>();

            if (slot == null) continue;

            GameObject itemPrefab = itemDictionary.GetItemPrefab(data.itemID);

            if (itemPrefab == null)
            {
                Debug.LogError("No item prefab found for ID: " + data.itemID);
                continue;
            }

            GameObject item = Instantiate(itemPrefab, slot.transform);

            RectTransform rt = item.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchoredPosition = Vector2.zero;
                rt.localScale = Vector3.one;
            }

            slot.currentItem = item;
        }
    }
}