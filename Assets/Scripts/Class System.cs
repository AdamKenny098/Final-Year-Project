using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassSystem : MonoBehaviour
{

    public enum Classes
    {
        Warrior,
        Archer,
        Mage,
        Thief,
        Merchant
    }

    public Dictionary<Classes, ClassStats> classData;

    void Awake()
    {
        classData = new Dictionary<Classes, ClassStats>()
        {
            {
                Classes.Warrior,
                new ClassStats
                {
                    className = "Warrior",
                    baseHealth = 150,
                    baseMana = 30,
                    baseStamina = 120,
                    baseStrength = 18,
                    baseDexterity = 10,
                    baseIntelligence = 8,
                    baseCharisma = 9,

                    healthPerLevel = 15,
                    manaPerLevel = 6,
                    staminaPerLevel = 10
                }
            },
            {
                Classes.Archer,
                new ClassStats
                {
                    className = "Archer",
                    baseHealth = 110,
                    baseMana = 40,
                    baseStamina = 80,
                    baseStrength = 12,
                    baseDexterity = 18,
                    baseIntelligence = 10,
                    baseCharisma = 11,

                    healthPerLevel = 10,
                    manaPerLevel = 8,
                    staminaPerLevel = 8
                }
            },
            {
                Classes.Mage,
                new ClassStats
                {
                    className = "Mage",
                    baseHealth = 80,
                    baseMana = 150,
                    baseStamina = 30,
                    baseStrength = 6,
                    baseDexterity = 9,
                    baseIntelligence = 20,
                    baseCharisma = 10,

                    healthPerLevel = 5,
                    manaPerLevel = 15,
                    staminaPerLevel = 5
                }
            },
            {
                Classes.Thief,
                new ClassStats
                {
                    className = "Thief",
                    baseHealth = 95,
                    baseMana = 35,
                    baseStamina = 100,
                    baseStrength = 10,
                    baseDexterity = 17,
                    baseIntelligence = 11,
                    baseCharisma = 12,

                    healthPerLevel = 10,
                    manaPerLevel = 5,
                    staminaPerLevel = 5
                }
            },
            {
                Classes.Merchant,
                new ClassStats
                {
                    className = "Merchant",
                    baseHealth = 100,
                    baseMana = 50,
                    baseStamina = 50,
                    baseStrength = 8,
                    baseDexterity = 10,
                    baseIntelligence = 13,
                    baseCharisma = 18,

                    healthPerLevel = 20,
                    manaPerLevel = 10,
                    staminaPerLevel = 10
                }
            }
        };
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

public class ClassStats
{
    public string className;
    public int baseHealth;
    public int baseMana;
    public int baseStamina;
    public int baseStrength;
    public int baseDexterity;
    public int baseIntelligence;
    public int baseCharisma;

    public int healthPerLevel;
    public int manaPerLevel;
    public int staminaPerLevel;
}


