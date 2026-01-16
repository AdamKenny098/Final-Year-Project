using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Entity//, IInteractable
{
    [Header("Progression")]
    private int level = 1;
    public int currentXP = 0;
    public ClassSystem.Classes characterClass;

    public override void Awake()
    {
        stats = new Stats();
        stats.level = Mathf.Max(1, level);
        base.Awake();
    }

    public void Start()
    {
        ApplyClassToStats();
    }

    public void ApplyClassToStats()
    {
        if (ClassSystem.Instance == null)
        {
            Debug.LogError("ClassSystem.Instance is null. Ensure ClassSystem exists in the scene.");
            return;
        }

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