using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Audio;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject newGamePanel;
    [SerializeField] private GameObject loadPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject quitDialog;

    [Header("New Game")]
    [SerializeField] private TMP_InputField nameInput;

    [Header("Load Panel")]
    [SerializeField] private Transform saveSlotContainer;
    [SerializeField] private GameObject saveSlotPrefab;

    [Header("Settings")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private AudioMixer audioMixer;

    [Header("Continue Button")]
    [SerializeField] private Button continueButton;

    [Header("Fade")]
    [SerializeField] private Image fadePanel;
    [SerializeField] private float fadeDuration = 1f;

    private string saveFolderPath;
    private string selectedSaveFile;
    private Resolution[] resolutions;

    void Start()
    {
        saveFolderPath = Path.Combine(Application.persistentDataPath, "Saves");

        // create saves folder if it doesn't exist
        if (!Directory.Exists(saveFolderPath))
            Directory.CreateDirectory(saveFolderPath);

        // show only main panel
        ShowPanel(mainPanel);

        // disable continue button if no saves exist
        continueButton.interactable = HasAnySave();

        // setup resolution dropdown
        SetupResolutions();

        // load settings
        LoadSettings();

        // start faded in
        StartCoroutine(FadeIn());
    }

    // ── panel navigation ──
    void ShowPanel(GameObject panel)
    {
        mainPanel.SetActive(false);
        newGamePanel.SetActive(false);
        loadPanel.SetActive(false);
        settingsPanel.SetActive(false);
        quitDialog.SetActive(false);

        if (panel != null)
            panel.SetActive(true);
    }

    // ── main menu buttons ──
    public void OnStartClicked()
    {
        ShowPanel(newGamePanel);
        nameInput.text = "";
        nameInput.Select();
    }

    public void OnContinueClicked()
    {
        // load most recent save
        string mostRecent = GetMostRecentSave();
        if (mostRecent != null)
            StartCoroutine(LoadGameRoutine(mostRecent));
    }

    public void OnLoadClicked()
    {
        ShowPanel(loadPanel);
        PopulateSaveSlots();
    }

    public void OnSettingsClicked()
    {
        ShowPanel(settingsPanel);
    }

    public void OnQuitClicked()
    {
        quitDialog.SetActive(true);
    }

    // ── new game ──
    public void OnConfirmNewGame()
    {
        string playerName = nameInput.text.Trim();
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("Please enter a name!");
            return;
        }

        // create new save file
        SaveData data = new SaveData();
        data.playerName = playerName;
        data.lastPlayed = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        data.saveFileName = playerName + "_" +
                             System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".json";

        string path = Path.Combine(saveFolderPath, data.saveFileName);
        File.WriteAllText(path, JsonUtility.ToJson(data, true));

        // save selected file name to PlayerPrefs so game knows which to load
        PlayerPrefs.SetString("CurrentSave", data.saveFileName);
        PlayerPrefs.Save();

        StartCoroutine(LoadGameRoutine(data.saveFileName));
    }

    public void OnNewGameBack()
    {
        ShowPanel(mainPanel);
    }

    // ── load panel ──
    void PopulateSaveSlots()
    {
        // clear existing slots
        foreach (Transform child in saveSlotContainer)
            Destroy(child.gameObject);

        // find all save files
        string[] files = Directory.GetFiles(saveFolderPath, "*.json");

        if (files.Length == 0)
        {
            Debug.Log("No save files found");
            return;
        }

        foreach (string file in files)
        {
            string json = File.ReadAllText(file);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            GameObject slot = Instantiate(saveSlotPrefab, saveSlotContainer);
            SaveSlotUI slotUI = slot.GetComponent<SaveSlotUI>();
            slotUI.Setup(Path.GetFileName(file), data, this);
        }
    }

    public void LoadSave(string fileName)
    {
        PlayerPrefs.SetString("CurrentSave", fileName);
        PlayerPrefs.Save();
        StartCoroutine(LoadGameRoutine(fileName));
    }

    public void DeleteSave(string fileName)
    {
        string path = Path.Combine(saveFolderPath, fileName);
        if (File.Exists(path))
            File.Delete(path);

        PopulateSaveSlots();
        continueButton.interactable = HasAnySave();
        Debug.Log("Deleted save: " + fileName);
    }

    public void OnLoadBack()
    {
        ShowPanel(mainPanel);
    }

    // ── settings ──
    void SetupResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentRes = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
                currentRes = i;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentRes;
        resolutionDropdown.RefreshShownValue();
    }

    public void OnVolumeChanged(float value)
    {
        if (audioMixer != null)
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("Volume", value);
    }

    public void OnBrightnessChanged(float value)
    {
        // use post processing exposure or gamma
        PlayerPrefs.SetFloat("Brightness", value);
        // if you have a Global Volume with ColorAdjustments:
        // colorAdjustments.postExposure.value = Mathf.Lerp(-2f, 2f, value);
    }

    public void OnResolutionChanged(int index)
    {
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        PlayerPrefs.SetInt("Resolution", index);
    }

    public void OnFullscreenToggled(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    void LoadSettings()
    {
        if (volumeSlider != null)
            volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);

        if (brightnessSlider != null)
            brightnessSlider.value = PlayerPrefs.GetFloat("Brightness", 0.5f);

        if (resolutionDropdown != null)
            resolutionDropdown.value = PlayerPrefs.GetInt("Resolution", 0);
    }

    public void OnSettingsBack()
    {
        PlayerPrefs.Save();
        ShowPanel(mainPanel);
    }

    // ── quit dialog ──
    public void OnQuitYes()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void OnQuitNo()
    {
        quitDialog.SetActive(false);
    }

    // ── helpers ──
    bool HasAnySave()
    {
        string[] files = Directory.GetFiles(saveFolderPath, "*.json");
        return files.Length > 0;
    }

    string GetMostRecentSave()
    {
        string[] files = Directory.GetFiles(saveFolderPath, "*.json");
        if (files.Length == 0) return null;

        string mostRecent = files[0];
        System.DateTime latest = File.GetLastWriteTime(files[0]);

        foreach (string file in files)
        {
            System.DateTime t = File.GetLastWriteTime(file);
            if (t > latest)
            {
                latest = t;
                mostRecent = file;
            }
        }

        return Path.GetFileName(mostRecent);
    }

    // ── scene loading with fade ──
    IEnumerator LoadGameRoutine(string saveFileName)
    {
        PlayerPrefs.SetString("CurrentSave", saveFileName);
        PlayerPrefs.Save();

        yield return StartCoroutine(FadeOut());
        SceneManager.LoadScene("Game");
    }

    IEnumerator FadeOut()
    {
        float elapsed = 0f;
        Color c = Color.black;
        c.a = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            fadePanel.color = c;
            yield return null;
        }
        c.a = 1f;
        fadePanel.color = c;
    }

    IEnumerator FadeIn()
    {
        float elapsed = 0f;
        Color c = Color.black;
        c.a = 1f;
        fadePanel.color = c;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            fadePanel.color = c;
            yield return null;
        }
        c.a = 0f;
        fadePanel.color = c;
    }
}