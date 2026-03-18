using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Item/ArmorItem")]
public class ArmorItem : Item
{
    public enum ArmorType
    {
        Helmet,
        Chestplate,
        Leggings,
        Boots,
    }

    public ArmorType armorType;
    public int defenseValue;

    [Header("Stat Bonuses")]
    public int strength;
    public int dexterity;
    public int intelligence;
    public int charisma;

    public int GetDefense()
    {
        float multiplier = Rarity.Instance.GetMultiplier(rarity);
        return Mathf.RoundToInt(defenseValue * multiplier);
    }
}