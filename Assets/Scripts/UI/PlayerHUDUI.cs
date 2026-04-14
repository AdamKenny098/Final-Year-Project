using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUDUI : MonoBehaviour
{
    [Header("Refs")]
    public Character player;

    [Header("Bottom Left - Bars")]
    public Slider healthBar;
    public Slider staminaBar;
    public Slider manaBar;

    [Header("Bottom Left - Values")]
    public TMP_Text healthValueText;
    public TMP_Text staminaValueText;
    public TMP_Text manaValueText;

    [Header("Top Left")]
    public TMP_Text levelText;
    public TMP_Text goldText;
    public TMP_Text xpText;

    [Header("Top Right")]
    public TMP_Text floorText;

    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player")?.GetComponentInParent<Character>();

        RefreshAll();
    }

    void Update()
    {
        RefreshAll();
    }

    void RefreshAll()
    {
        if (player == null || player.stats == null)
            return;

        UpdateBars();
        UpdateTopLeft();
        UpdateTopRight();
    }

    void UpdateBars()
    {
        Stats s = player.stats;

        if (healthBar != null)
        {
            healthBar.maxValue = s.maxHealth;
            healthBar.value = s.health;
        }

        if (staminaBar != null)
        {
            staminaBar.maxValue = s.maxStamina;
            staminaBar.value = s.stamina;
        }

        if (manaBar != null)
        {
            manaBar.maxValue = s.maxMana;
            manaBar.value = s.mana;
        }

        if (healthValueText != null)
            healthValueText.text = $"{s.health} / {s.maxHealth}";

        if (staminaValueText != null)
            staminaValueText.text = $"{s.stamina} / {s.maxStamina}";

        if (manaValueText != null)
            manaValueText.text = $"{s.mana} / {s.maxMana}";
    }

    void UpdateTopLeft()
    {
        if (levelText != null)
            levelText.text = $"Level: {player.stats.level}";

        if (goldText != null)
            goldText.text = $"Gold: {GetGoldAmount()}";

        if (xpText != null)
            xpText.text = $"XP: {player.currentXP} / {player.GetXPToNextLevel()}";
    }

    void UpdateTopRight()
    {
        if (floorText != null)
            floorText.text = $"Floor {GetDisplayedFloor()}";
    }

    int GetGoldAmount()
    {
        if (ShopSystem.Instance == null)
            return 0;

        return ShopSystem.Instance.GetPlayerGold();
    }

    int GetDisplayedFloor()
    {
        if (LabyrinthManager.Instance == null)
            return 1;

        return LabyrinthManager.Instance.currentFloorIndex;
    }

    int GetMaxXPForCurrentLevel()
    {
        int level = Mathf.Max(1, player.stats.level);

        // Temporary formula until you add a real XP progression table.
        return 100 + ((level - 1) * 50);
    }
}