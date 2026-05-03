using UnityEngine;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance;

    [Header("References")]
    public ItemDictionary itemDictionary;
    public Transform hotbarPanel;
    public Transform backpackPanel;
    public GameObject slotPrefab;

    [Header("Slot Counts")]
    public int hotbarSlotCount = 8;
    public int backpackSlotCount = 24;

    void Awake()
    {
        Instance = this;

        if (itemDictionary == null)
            itemDictionary = FindFirstObjectByType<ItemDictionary>();
    }

    void Start()
    {
        CreateSlotsIfMissing(hotbarPanel, hotbarSlotCount);
        CreateSlotsIfMissing(backpackPanel, backpackSlotCount);
    }

    void CreateSlotsIfMissing(Transform panel, int count)
    {
        if (panel == null) return;

        if (panel.childCount == 0)
        {
            for (int i = 0; i < count; i++)
            {
                Instantiate(slotPrefab, panel);
            }
        }
    }

    public bool AddItem(int itemID, int amount)
    {
        GameObject itemPrefab = itemDictionary.GetItemPrefab(itemID);

        if (itemPrefab == null)
        {
            Debug.LogError("No item prefab found for ID: " + itemID);
            return false;
        }

        int remaining = amount;

        // Stack first
        remaining = AddToExistingStacks(hotbarPanel, itemID, remaining);
        if (remaining <= 0) return true;

        remaining = AddToExistingStacks(backpackPanel, itemID, remaining);
        if (remaining <= 0) return true;

        // Empty slots only if still has remaining
        remaining = AddToEmptySlots(hotbarPanel, itemPrefab, itemID, remaining);
        if (remaining <= 0) return true;

        remaining = AddToEmptySlots(backpackPanel, itemPrefab, itemID, remaining);
        if (remaining <= 0) return true;

        return false;
    }

    int AddToExistingStacks(Transform panel, int itemID, int amount)
    {
        if (panel == null) return amount;

        foreach (Transform slotTransform in panel)
        {
            Slot slot = slotTransform.GetComponent<Slot>();

            if (slot != null && slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();

                if (item != null && item.ID == itemID && item.isStackable)
                {
                    int space = item.maxStack - item.amount;
                    int addAmount = Mathf.Min(space, amount);

                    item.amount += addAmount;
                    item.UpdateAmountText();

                    amount -= addAmount;

                    if (amount <= 0)
                        return 0;
                }
            }
        }

        return amount;
    }

    int AddToEmptySlots(Transform panel, GameObject itemPrefab, int itemID, int amount)
    {
        if (panel == null) return amount;

        foreach (Transform slotTransform in panel)
        {
            Slot slot = slotTransform.GetComponent<Slot>();

            if (slot != null && slot.currentItem == null)
            {
                GameObject newItem = Instantiate(itemPrefab, slot.transform);

                RectTransform rt = newItem.GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.anchorMin = Vector2.zero;
                    rt.anchorMax = Vector2.one;
                    rt.offsetMin = Vector2.zero;
                    rt.offsetMax = Vector2.zero;
                    rt.localScale = Vector3.one;
                }

                Item item = newItem.GetComponent<Item>();
                int addAmount = amount;

                if (item != null && item.isStackable)
                {
                    addAmount = Mathf.Min(item.maxStack, amount);
                    item.amount = addAmount;
                    item.UpdateAmountText();
                }

                slot.currentItem = newItem;
                amount -= addAmount;

                if (amount <= 0)
                    return 0;
            }
        }

        return amount;
    }
}