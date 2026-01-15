using UnityEngine;

[System.Serializable]
public class Stats
{
    public int level = 1;

    public int maxHealth;
    public int health;

    public int maxMana;
    public int mana;

    public int maxStamina;
    public int stamina;

    public int strength;
    public int dexterity;
    public int intelligence;
    public int charisma;

    public void FillToMax()
    {
        health = maxHealth;
        mana = maxMana;
        stamina = maxStamina;
    }
}
