using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Item/WeaponItem")]
public class WeaponItem : Item
{
    public int damage;
    public float attackSpeed;
    public enum DamageType
    {
        Bludgeoning,
        Piercing,
        Slashing,
        Magic
    }
    public DamageType damageType;
    public enum ClassType
    {
        Warrior,
        Archer,
        Mage,
        Thief
    }

    public ClassType classType;

    public enum WeaponType
    {
        Sword,
        Axe,
        Bow,
        Dagger,
        Staff,
        Mace,
        Wand
    }

    public WeaponType weaponType;
}
