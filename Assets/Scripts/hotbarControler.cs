using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;

public class HotbarControler : MonoBehaviour
{
    public GameObject hotbarPanel;
    public GameObject slotPrefab;
    public int slotCount = 8;

    private ItemDictionary itemDictionary;

    private Key[] hotbarKeys;

    public void Awake()
    {
        itemDictionary = FindFirstObjectByType<ItemDictionary>();

        hotbarKeys = new Key[slotCount];
        for (int i = 0; i < slotCount; i++)
        {
            hotbarKeys[i] = i < 7 ? (Key)((int)Key.Digit1 + i) : Key.Digit0; // Assign keys 1-8 to the hotbar slots

        }
    }


    void Update()
    {
        for (int i = 0; i < slotCount; i++) 
        {
            if (Keyboard.current[hotbarKeys[i]].wasPressedThisFrame)
            {
                UseItemSlot(i);
            }
        }
    }

    void UseItemSlot(int index) 
    { 
        Slot slot = hotbarPanel.transform.GetChild(index).GetComponent<Slot>();
        if(slot.currentItem != null) 
        {
            Item item = slot.currentItem.GetComponent<Item>();
            if (item != null) 
            {
                item.UseItem();
            }
        }
    }

     public List<InventorySaveData> GetHotbarItems()
    {
        List<InventorySaveData> hotbarData = new List<InventorySaveData>();
        foreach (Transform slotTransform in hotbarPanel.transform)
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

                hotbarData.Add(new InventorySaveData { itemID = item.ID, slotIndex = slotTransform.GetSiblingIndex() });
            }
        }
        return hotbarData;
    }

    // fixed � was missing parameter name
    public void SetHotbarItems(List<InventorySaveData> inventorySaveData)
    {
        // clear existing slots
        foreach (Transform child in hotbarPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // create fresh slots
        for (int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab, hotbarPanel.transform);
        }

        // populate slots with saved items
        foreach (InventorySaveData data in inventorySaveData)
        {
            if (data.slotIndex < slotCount)
            {
                Slot slot = hotbarPanel.transform
                            .GetChild(data.slotIndex)
                            .GetComponent<Slot>();

                // fixed � was passing data.slotIndex, should be data.itemID
                GameObject itemPrefab = itemDictionary.GetItemPrefab(data.itemID);

                if (itemPrefab != null)
                {
                    GameObject item = Instantiate(itemPrefab, slot.transform);
                    item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    slot.currentItem = item;
                }
            }
        }
    }
}
