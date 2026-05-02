using UnityEngine;
using UnityEngine.InputSystem;

public class FarmingSystem : MonoBehaviour
{
    [SerializeField] private float interactRange = 1.5f;
    [SerializeField] private HotbarControler hotbar;

    private Camera mainCamera;
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
        if (Mouse.current.leftButton.wasPressedThisFrame)
            TryInteract();
    }

    void TryInteract()
    {
        if (hotbar == null) return;

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

        Collider2D[] hits = Physics2D.OverlapPointAll(mouseWorldPos);

        // AXE / TREE
        if (equippedItem.Name == "Axe")
        {
            foreach (Collider2D h in hits)
            {
                HarvestableProp prop = h.GetComponentInParent<HarvestableProp>();

                if (prop != null)
                {
                    Debug.Log("Found harvestable: " + prop.name);

                    if (playerMovement != null)
                        playerMovement.PlayAxeAnimation(direction);

                    prop.HitProp();
                    return;
                }
            }

            Debug.Log("No HarvestableProp found where you clicked.");
            return;
        }

        // FARM TILE
        foreach (Collider2D h in hits)
        {
            FarmTile tile = h.GetComponent<FarmTile>();

            if (tile != null)
            {
                tile.Interact(equippedItem);
                return;
            }
        }
    }
}