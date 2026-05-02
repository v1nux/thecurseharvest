using UnityEngine;
using UnityEngine.InputSystem;

public class FarmingSystem : MonoBehaviour
{
    [SerializeField] private float interactRange = 1.5f;
    [SerializeField] private HotbarControler hotbar;

    private Camera mainCamera;
    private int selectedSlot = 0;
    private PlayerMovement playerMovement;

    void Start()
    {
        mainCamera = Camera.main;
        playerMovement = GetComponent<PlayerMovement>();

        if (hotbar == null)
            hotbar = FindFirstObjectByType<HotbarControler>();
    }

    void Update()
    {
        for (int i = 0; i < 8; i++)
        {
            Key key = i < 7 ? (Key)((int)Key.Digit1 + i) : Key.Digit0;

            if (Keyboard.current[key].wasPressedThisFrame)
                selectedSlot = i;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
            TryInteract();
    }

    void TryInteract()
    {
        Item equippedItem = hotbar.GetSelectedItem();
        if (equippedItem == null) return;

        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        float dist = Vector2.Distance(transform.position, mouseWorldPos);
        if (dist > interactRange)
        {
            Debug.Log("Too far to interact!");
            return;
        }

        Vector2 direction = (mouseWorldPos - (Vector2)transform.position).normalized;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            direction = new Vector2(Mathf.Sign(direction.x), 0);
        else
            direction = new Vector2(0, Mathf.Sign(direction.y));

        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos);
        if (hit == null) return;

        // AXE / TREE
        if (equippedItem.Name == "Axe")
        {
            HarvestableProp prop = hit.GetComponentInParent<HarvestableProp>();

            if (prop != null)
            {
                if (playerMovement != null)
                    playerMovement.PlayAxeAnimation(direction);

                prop.HitProp();

                return;
            }
        }

        // FARM TILE
        FarmTile tile = hit.GetComponent<FarmTile>();
        if (tile != null)
        {
            tile.Interact(equippedItem);
        }
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