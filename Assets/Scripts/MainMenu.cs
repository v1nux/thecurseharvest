using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    [Header("Panels")]
    [SerializeField] private GameObject settingsPanel;

    [Header("Scene Name")]
    [SerializeField] private string gameSceneName = "GameScene";

    void Start()
    {
        // hide settings panel by default
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        // check if save exists — grey out load button if not
        if (loadButton != null)
        {
            bool hasSave = System.IO.File.Exists(
                System.IO.Path.Combine(
                    Application.persistentDataPath, "savegame.json"));

            loadButton.interactable = hasSave;
        }

        // wire buttons
        startButton?.onClick.AddListener(StartGame);
        loadButton?.onClick.AddListener(LoadGame);
        settingsButton?.onClick.AddListener(OpenSettings);
        quitButton?.onClick.AddListener(QuitGame);
    }

    void StartGame()
    {
        // delete old save and start fresh
        string path = System.IO.Path.Combine(
                      Application.persistentDataPath, "savegame.json");

        if (System.IO.File.Exists(path))
            System.IO.File.Delete(path);

        SceneManager.LoadScene(gameSceneName);
    }

    void LoadGame()
    {
        // load game scene — SaveController loads save automatically on Start
        SceneManager.LoadScene(gameSceneName);
    }

    void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit called");
    }
}