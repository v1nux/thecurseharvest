using Cinemachine;
using System.IO;
using UnityEngine;

public class SaveController : MonoBehaviour
{
    public static SaveController Instance;

    private string saveFilePath;

    private InventoryController inventoryController;
    private HotbarControler hotbarController;
    private CinemachineConfiner2D confiner;
    private PlayerStatsManager statsManager;

    void Awake()
    {
        Instance = this;
        saveFilePath = Path.Combine(Application.persistentDataPath, "savegame.json");

        inventoryController = FindObjectOfType<InventoryController>();
        hotbarController = FindObjectOfType<HotbarControler>();
        confiner = FindObjectOfType<CinemachineConfiner2D>();
        statsManager = FindObjectOfType<PlayerStatsManager>();
    }

    void Start()
    {
        LoadGame();
    }

    public void SaveGame()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Player not found!");
            return;
        }

        SaveData data = new SaveData();

        // player
        data.playerPosition = player.transform.position;

        // stats
        if (statsManager != null)
        {
            data.health = statsManager.GetHealth();
            data.stamina = statsManager.GetStamina();
            data.maxHealth = statsManager.GetMaxHealth();
            data.maxStamina = statsManager.GetMaxStamina();
            data.level = statsManager.GetLevel();
            data.currentExp = statsManager.GetExp();
            data.expToNextLevel = statsManager.GetExpToNext();
            data.strength = statsManager.GetStrength();
            data.defense = statsManager.GetDefense();
            data.speed = statsManager.GetSpeed();
        }

        // day system
        if (GameManager.Instance != null)
        {
            data.dayNumber = GameManager.Instance.dayNumber;
            data.seasonIndex = GameManager.Instance.seasonIndex;
        }

        // map boundary
        if (confiner != null && confiner.m_BoundingShape2D != null)
        {
            data.mapBoundaryName = confiner.m_BoundingShape2D.gameObject.name;
        }

        // inventory
        if (inventoryController != null)
            data.inventorySaveData = inventoryController.GetInventoryItems();

        if (hotbarController != null)
            data.hotbarSaveData = hotbarController.GetHotbarItems();

        File.WriteAllText(saveFilePath, JsonUtility.ToJson(data, true));

        Debug.Log("Saved to: " + saveFilePath);
    }

    public void LoadGame()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("No save found — creating new one");
            SaveGame();
            return;
        }

        SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveFilePath));

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            player.transform.position = data.playerPosition;

        // boundary
        if (confiner != null && !string.IsNullOrEmpty(data.mapBoundaryName))
        {
            GameObject boundaryObj = GameObject.Find(data.mapBoundaryName);

            if (boundaryObj != null)
            {
                confiner.m_BoundingShape2D =
                    boundaryObj.GetComponent<PolygonCollider2D>();

                confiner.InvalidateCache();
            }
        }

        // stats
        if (statsManager != null)
        {
            statsManager.SetMaxHealth(data.maxHealth);
            statsManager.SetMaxStamina(data.maxStamina);
            statsManager.SetHealth(data.health);
            statsManager.SetStamina(data.stamina);
            statsManager.SetLevel(data.level);
            statsManager.SetExp(data.currentExp);
            statsManager.SetExpToNext(data.expToNextLevel);
            statsManager.SetStrength(data.strength);
            statsManager.SetDefense(data.defense);
            statsManager.SetSpeed(data.speed);
        }

        // inventory
        if (inventoryController != null && data.inventorySaveData != null)
            inventoryController.SetInventoryItems(data.inventorySaveData);

        if (hotbarController != null && data.hotbarSaveData != null)
            hotbarController.SetHotbarItems(data.hotbarSaveData);

        // day system
        if (GameManager.Instance != null)
        {
            GameManager.Instance.dayNumber = data.dayNumber;
            GameManager.Instance.seasonIndex = data.seasonIndex;
        }

        Debug.Log("Game Loaded Successfully");
    }

    public bool HasSave()
    {
        return File.Exists(saveFilePath);
    }

    public void DeleteSave()
    {
        if (File.Exists(saveFilePath))
            File.Delete(saveFilePath);

        Debug.Log("Save deleted");
    }
}