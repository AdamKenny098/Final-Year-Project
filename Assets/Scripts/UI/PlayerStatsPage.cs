using UnityEngine;
using TMPro;

public class PlayerStatsPage : MonoBehaviour
{
    [Header("Refs")]
    public TMP_Text output;
    public Character character;

    [Header("Update")]
    public float refreshInterval = 0.25f;

    float timer;

    void Awake()
    {
        if (output == null) output = GetComponent<TMP_Text>();
    }

    void Start()
    {
        Refresh();
    }

    void Update()
    {
        timer += Time.unscaledDeltaTime;
        if (timer >= refreshInterval)
        {
            timer = 0f;
            Refresh();
        }
    }

    public void Refresh()
    {
        if (output == null) return;

        if (character == null)
        {
            output.text = "No Character assigned.";
            return;
        }

        Stats stats = character.stats;

        int progLevel = character.level;
        int statLevel = stats != null ? stats.level : 0;

        int hp = stats != null ? stats.health : 0;
        int hpMax = stats != null ? stats.maxHealth : 0;

        int mp = stats != null ? stats.mana : 0;
        int mpMax = stats != null ? stats.maxMana : 0;

        int sp = stats != null ? stats.stamina : 0;
        int spMax = stats != null ? stats.maxStamina : 0;

        int str = stats != null ? stats.strength : 0;
        int dex = stats != null ? stats.dexterity : 0;
        int intel = stats != null ? stats.intelligence : 0;
        int cha = stats != null ? stats.charisma : 0;

        int strMod = stats != null ? stats.StrMod : 0;
        int dexMod = stats != null ? stats.DexMod : 0;
        int intMod = stats != null ? stats.IntMod : 0;
        int chaMod = stats != null ? stats.ChaMod : 0;

        int prof = stats != null ? stats.ProficiencyBonus : 0;
        int ac = stats != null ? stats.ArmorClass : 0;

        int spellDC = stats != null ? stats.SpellSaveDC : 0;
        int spellAtk = stats != null ? stats.SpellAttackBonus : 0;

        int meleeAtk = stats != null ? stats.MeleeAttackBonus : 0;
        int rangedAtk = stats != null ? stats.RangedAttackBonus : 0;

        int armorBonus = stats != null ? stats.armorBonus : 0;
        int shieldBonus = stats != null ? stats.shieldBonus : 0;

        int xp = character.currentXP;
        var cls = character.characterClass;

        string levelLine = (statLevel != progLevel && statLevel > 0)
            ? $"Level: {progLevel}  (Stats Lvl: {statLevel})"
            : $"Level: {progLevel}";

        output.text =
            "<b>CHARACTER</b>\n" +
            $"Class: {cls}\n" +
            $"{levelLine}\n" +
            $"XP: {xp}\n\n" +

            "<b>VITALS</b>\n" +
            $"HP: {hp}/{hpMax}\n" +
            $"Mana: {mp}/{mpMax}\n" +
            $"Stamina: {sp}/{spMax}\n\n" +

            "<b>ATTRIBUTES</b>\n" +
            $"STR {str} ({FmtMod(strMod)})   DEX {dex} ({FmtMod(dexMod)})\n" +
            $"INT {intel} ({FmtMod(intMod)})   CHA {cha} ({FmtMod(chaMod)})\n\n" +

            "<b>COMBAT</b>\n" +
            $"AC: {ac}  (Armor +{armorBonus}, Shield +{shieldBonus})\n" +
            $"Proficiency: {FmtMod(prof)}\n" +
            $"Melee Atk: {FmtMod(meleeAtk)}   Ranged Atk: {FmtMod(rangedAtk)}\n\n" +

            "<b>MAGIC</b>\n" +
            $"Spell DC: {spellDC}\n" +
            $"Spell Atk: {FmtMod(spellAtk)}";
    }

    string FmtMod(int v)
    {
        return v >= 0 ? $"+{v}" : v.ToString();
    }
}
