using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public int itemID;
    public int amount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        InventoryController inv = InventoryController.Instance;

        if (inv == null)
        {
            Debug.LogError("No InventoryController found.");
            return;
        }

        bool added = inv.AddItem(itemID, amount);

        if (added)
        {
            if (PickupUI.Instance != null)
                PickupUI.Instance.ShowPickup(itemID, amount);

            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Inventory full. Item was not picked up.");
        }
    }
}