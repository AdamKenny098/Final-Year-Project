using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Entity
{
    public ClassSystem classSystem;
    public ClassSystem.Classes entityClass;
    public ClassStats stats;
    public List<Unlock> unlocks;
    public List<Unlock> unlockedAbilities = new List<Unlock>();
    public  Dictionary<string, float> abilityCooldownTimers = new Dictionary<string, float>();


    public int attack, defense, speed, stamina, mana, level, height,
               currentMana, currentStamina, strength, dexterity, intelligence, charisma,
               currentXP;

    public float XPToNextLevel = 100f;

    public string species;
    public bool isHostile;

    // Start is called before the first frame update
    void Start()
    {
        //Apply the correct class and stats at runtime
        classSystem = FindObjectOfType<ClassSystem>();
        entityClass = ClassSystem.Classes.Mage;
        stats = classSystem.GetStats(entityClass);
        unlocks = classSystem.GetUnlocks(entityClass);
        unlockedAbilities = classSystem.GetUnlocks(entityClass);
        ApplyLevelUpToStats();

        abilityCooldownTimers = new Dictionary<string, float>();
        foreach (var ability in unlockedAbilities)
        {
            abilityCooldownTimers[ability.unlockName] = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UseAbility("Firebolt");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            LevelUp();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UseAbility("Teleport");
        }
    }

    public void LevelUp()
    {
        currentXP = 0;
        level++;
        XPToNextLevel *= 1.25f;
        ApplyLevelUpToStats();
        UnityEngine.Debug.Log(level);
    }

    void ApplyLevelUpToStats()
    {
        //level - 1 so upon level 2 the player doesn't get a double level up
        currentHealth = stats.baseHealth + stats.healthPerLevel * (level - 1);
        currentMana = stats.baseMana + stats.manaPerLevel * (level - 1);
        currentStamina = stats.baseStamina + stats.staminaPerLevel * (level - 1);
    }

    void ApplyUnlockedAbilities()
    {
        foreach (Unlock unlockable in unlocks)
        {
            if (unlockable.unlockLevel == level && !unlockedAbilities.Contains(unlockable))
            {
                unlockedAbilities.Add(unlockable);
                abilityCooldownTimers[unlockable.unlockName] = 0f;
            }
        }
    }

    public void UseAbility(string abilityName)
    {
        //https://discussions.unity.com/t/search-for-objects-in-a-list/662122/3
        Unlock ability = unlockedAbilities.Find(a => a.unlockName == abilityName);
        if (ability == null)
        {
            return;
        }

        if (abilityCooldownTimers[abilityName] > 0)
        {
            return;
        }
        
        abilityCooldownTimers[abilityName] = ability.unlockCoolDown;
        UnityEngine.Debug.Log("Ability used");
    }
}
