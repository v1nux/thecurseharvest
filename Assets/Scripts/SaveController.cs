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
    private PropsSpawner[] propsSpawners;

    void Awake()
    {
        Instance = this;
        saveFilePath = Path.Combine(Application.persistentDataPath, "savegame.json");

        inventoryController = FindFirstObjectByType<InventoryController>();
        hotbarController = FindFirstObjectByType<HotbarControler>();
        confiner = FindFirstObjectByType<CinemachineConfiner2D>();
        statsManager = FindFirstObjectByType<PlayerStatsManager>();
        propsSpawners = FindObjectsByType<PropsSpawner>(FindObjectsSortMode.None);
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
        if (DayManager.Instance != null)
        {
            data.dayNumber = DayManager.Instance.dayNumber;
            data.seasonIndex = DayManager.Instance.seasonIndex;
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

        data.forestProps.Clear();

        foreach (PropsSpawner spawner in propsSpawners)
        {
            if (spawner != null)
                data.forestProps.AddRange(spawner.GetSaveData());
        }

        File.WriteAllText(saveFilePath, JsonUtility.ToJson(data, true));

        Debug.Log("Saved to: " + saveFilePath);
    }

    public void LoadGame()
    {
        if (!File.Exists(saveFilePath))
    {
        Debug.LogWarning("No save found — keeping default scene setup");
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
        if (DayManager.Instance != null)
        {
            DayManager.Instance.dayNumber = data.dayNumber;
            DayManager.Instance.seasonIndex = data.seasonIndex;
            DayManager.Instance.UpdateDayUI();
        }

        foreach (PropsSpawner spawner in propsSpawners)
        {
            if (spawner != null)
                spawner.LoadFromSave(data.forestProps);
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