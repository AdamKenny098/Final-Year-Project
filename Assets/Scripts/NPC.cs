using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Entity
{
    public ClassSystem.Classes entityClass;
    private ClassStats stats;

    public int attack, defense, speed, stamina, mana, level, height,
               currentMana, currentStamina, strength, dexterity, intelligence, charisma;

    public string species;
    public bool isHostile;

    // Start is called before the first frame update
    void Start()
    {   
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LevelUp()
    {
        level++;
        ApplyLevelUpToStats();
    }
    
    void ApplyLevelUpToStats()
    {
        //level - 1 so upon level 2 the player doesn't get a double level up
        currentHealth = stats.baseHealth + stats.healthPerLevel * (level - 1);
        currentMana = stats.baseMana + stats.manaPerLevel * (level - 1);
        currentStamina = stats.baseStamina + stats.staminaPerLevel * (level - 1);
    }
}
