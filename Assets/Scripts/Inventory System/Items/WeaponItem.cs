using UnityEngine;

[CreateAssetMenu(menuName = "Data/Item/WeaponItem")]
public class WeaponItem : Item
{
    public int damage;
    public float attackSpeed;

    public DamageType damageType;

    [Header("Stat Bonuses")]
    public int strength;
    public int dexterity;
    public int intelligence;
    public int charisma;

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

    public int GetDamage()
    {
        float multiplier = Rarity.Instance.GetMultiplier(rarity);
        return Mathf.RoundToInt(damage * multiplier);
    }
}