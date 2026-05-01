using UnityEngine;
using UnityEngine.InputSystem;

public class FarmingSystem : MonoBehaviour
{
    [SerializeField] private float interactRange = 1.5f;
    [SerializeField] private HotbarControler hotbar;

    private Camera mainCamera;
    private int selectedSlot = 0;

    void Start()
    {
        mainCamera = Camera.main;
        if (hotbar == null)
            hotbar = FindObjectOfType<HotbarControler>();
    }

    void Update()
    {
        // track selected hotbar slot (keys 1-8)
        for (int i = 0; i < 8; i++)
        {
            Key key = i < 7 ? (Key)((int)Key.Digit1 + i) : Key.Digit0;
            if (Keyboard.current[key].wasPressedThisFrame)
                selectedSlot = i;
        }

        // left click to use tool on farm tile
        if (Mouse.current.leftButton.wasPressedThisFrame)
            TryInteract();
    }

    void TryInteract()
    {
        Item equippedItem = GetEquippedItem();
        if (equippedItem == null) return;

        // only act on farming item types
        if (equippedItem.itemType == Item.ItemType.Misc) return;

        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(
                                Mouse.current.position.ReadValue());

        float dist = Vector2.Distance(transform.position, mouseWorldPos);
        if (dist > interactRange)
        {
            Debug.Log("Too far to interact!");
            return;
        }

        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos);
        if (hit == null) return;

        FarmTile tile = hit.GetComponent<FarmTile>();
        if (tile == null) return;

        tile.Interact(equippedItem);
    }

    Item GetEquippedItem()
    {
        if (hotbar == null) return null;
        if (selectedSlot >= hotbar.hotbarPanel.transform.childCount) return null;

        Slot slot = hotbar.hotbarPanel.transform
                    .GetChild(selectedSlot)
                    .GetComponent<Slot>();

        if (slot == null || slot.currentItem == null) return null;

        return slot.currentItem.GetComponent<Item>();
    }
}