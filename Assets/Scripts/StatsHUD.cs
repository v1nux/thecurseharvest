using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsHUD : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Stamina")]
    [SerializeField] private Slider staminaBar;
    [SerializeField] private TextMeshProUGUI staminaText;

    private PlayerStatsManager playerStats;

    void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStatsManager>();

        if (playerStats == null)
            Debug.LogError("PlayerStatsManager not found!");
    }

    void Update()
    {
        if (playerStats == null) return;

        // health
        if (healthBar != null)
            healthBar.value = playerStats.GetHealth() / playerStats.GetMaxHealth();
        if (healthText != null)
            healthText.text = Mathf.CeilToInt(playerStats.GetHealth())
                            + "\n/" + (int)playerStats.GetMaxHealth();

        // stamina
        if (staminaBar != null)
            staminaBar.value = playerStats.GetStamina() / playerStats.GetMaxStamina();
        if (staminaText != null)
            staminaText.text = Mathf.CeilToInt(playerStats.GetStamina())
                             + "\n/" + (int)playerStats.GetMaxStamina();
    }
}