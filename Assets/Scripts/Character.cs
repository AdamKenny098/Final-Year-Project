using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Entity//, IInteractable
{
    [Header("Progression")]
    public int level = 1;
    public int currentXP = 0;
    public ClassSystem.Classes characterClass;

    void Awake()
    {
        stats = new Stats();
        stats.level = Mathf.Max(1, level);
        ApplyClassToStats();
        base.Awake();
    }

    public void ApplyClassToStats()
    {
        ClassStats cs = ClassSystem.Instance.GetStats(characterClass);

        int lvl = Mathf.Max(1, stats.level); // Ensure level is at least 1

        stats.maxHealth = cs.baseHealth + cs.healthPerLevel * (lvl - 1);
        stats.maxMana = cs.baseMana + cs.manaPerLevel * (lvl - 1);
        stats.maxStamina = cs.baseStamina + cs.staminaPerLevel * (lvl - 1);

        stats.strength = cs.baseStrength;
        stats.dexterity = cs.baseDexterity;
        stats.intelligence = cs.baseIntelligence;
        stats.charisma = cs.baseCharisma;

        stats.FillToMax();
    }

    public void AddXP(int amount)
    {
        currentXP += amount;
    }

}