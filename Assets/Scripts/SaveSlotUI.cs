using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI slotNameText;
    [SerializeField] private TextMeshProUGUI slotDayText;
    [SerializeField] private TextMeshProUGUI slotDateText;
    [SerializeField] private Button deleteButton;

    private string saveFileName;
    private MainMenuController menuController;

    public void Setup(string fileName, SaveData data, MainMenuController controller)
    {
        saveFileName = fileName;
        menuController = controller;

        slotNameText.text = data.playerName;
        slotDayText.text = GetSeasonName(data.seasonIndex) + " Day " + data.dayNumber;
        slotDateText.text = "Last played: " + data.lastPlayed;

        GetComponent<Button>().onClick.AddListener(OnSlotClicked);
        deleteButton.onClick.AddListener(OnDeleteClicked);
    }

    string GetSeasonName(int index)
    {
        string[] seasons = { "Tag-init", "Tag-ulan" };
        return index >= 0 && index < seasons.Length ? seasons[index] : "Tag-init";
    }

    void OnSlotClicked()
    {
        menuController.LoadSave(saveFileName);
    }

    void OnDeleteClicked()
    {
        menuController.DeleteSave(saveFileName);
    }
}