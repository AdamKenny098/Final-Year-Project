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
}
