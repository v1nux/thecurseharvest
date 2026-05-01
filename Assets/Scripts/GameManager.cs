using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int dayNumber = 1;
    public int seasonIndex = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void NextDay()
    {
        dayNumber++;

        if (dayNumber > 30)
        {
            dayNumber = 1;
            seasonIndex = (seasonIndex + 1) % 2;
        }

        Debug.Log($"Day: {dayNumber}, Season: {GetSeasonName()}");

        // Optional auto-save
        if (SaveController.Instance != null)
            SaveController.Instance.SaveGame();
    }

    public string GetSeasonName()
    {
        string[] seasons = { "Tag-init", "Tag-ulan" };

        if (seasonIndex < 0 || seasonIndex >= seasons.Length)
            return "Unknown";

        return seasons[seasonIndex];
    }
}