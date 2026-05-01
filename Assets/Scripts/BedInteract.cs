using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;

public class BedInteract : MonoBehaviour
{
    [SerializeField] private GameObject sleepPromptPanel;
    [SerializeField] private Transform wakeUpPosition;

    [Header("Sleep Transition")]
    [SerializeField] private Image sleepFadePanel;
    [SerializeField] private TextMeshProUGUI sleepText;
    [SerializeField] private float fadeTime = 1f;
    [SerializeField] private float blackScreenTime = 1.2f;

    private bool playerInside = false;
    private bool sleeping = false;
    private GameObject player;

    void Start()
    {
        if (sleepPromptPanel != null)
            sleepPromptPanel.SetActive(false);

        if (sleepFadePanel != null)
        {
            Color c = sleepFadePanel.color;
            c.a = 0f;
            sleepFadePanel.color = c;
        }

        if (sleepText != null)
            sleepText.text = "";
    }

    void Update()
    {
        if (sleeping) return;

        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            if (sleepPromptPanel != null)
                sleepPromptPanel.SetActive(true);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            player = other.gameObject;
            Debug.Log("Press E to sleep");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;

            if (sleepPromptPanel != null)
                sleepPromptPanel.SetActive(false);
        }
    }

    public void SleepYes()
    {
        if (!sleeping)
            StartCoroutine(SleepRoutine());
    }

    public void SleepNo()
    {
        if (sleepPromptPanel != null)
            sleepPromptPanel.SetActive(false);
    }

    IEnumerator SleepRoutine()
    {
        sleeping = true;

        if (sleepPromptPanel != null)
            sleepPromptPanel.SetActive(false);

        if (sleepText != null)
            sleepText.text = "Sleeping...";

        yield return StartCoroutine(Fade(0f, 1f));

        if (wakeUpPosition != null && player != null)
        {
            player.transform.position = wakeUpPosition.position;

            var confiner = FindObjectOfType<CinemachineConfiner2D>();
            if (confiner != null)
                confiner.InvalidateCache();
        }

        if (DayManager.Instance != null)
            DayManager.Instance.AdvanceDay();

        if (SaveController.Instance != null)
            SaveController.Instance.SaveGame();

        if (sleepText != null && DayManager.Instance != null)
            sleepText.text = DayManager.Instance.GetDayString() + "\nGame Saved";

        yield return new WaitForSeconds(blackScreenTime);

        if (sleepText != null)
            sleepText.text = "";

        yield return StartCoroutine(Fade(1f, 0f));

        sleeping = false;
    }

    IEnumerator Fade(float from, float to)
    {
        if (sleepFadePanel == null)
            yield break;

        float timer = 0f;
        Color c = sleepFadePanel.color;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, timer / fadeTime);
            sleepFadePanel.color = c;
            yield return null;
        }

        c.a = to;
        sleepFadePanel.color = c;
    }
}