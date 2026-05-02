using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public int itemID;
    public int amount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        InventoryController inv = FindFirstObjectByType<InventoryController>();

        if (inv != null)
        {
            inv.AddItem(itemID, amount);

            if (PickupUI.Instance != null)
                PickupUI.Instance.ShowPickup(itemID, amount);

            Destroy(gameObject);
        }
    }
}