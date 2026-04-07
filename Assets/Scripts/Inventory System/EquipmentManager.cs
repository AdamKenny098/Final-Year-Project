using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance;

    [Header("References")]
    public Inventory playerInventory;
    public Character playerCharacter;

    [Header("Equipped Armor")]
    public ArmorItem helmet;
    public ArmorItem chestplate;
    public ArmorItem leggings;
    public ArmorItem boots;

    [Header("Equipped Weapon")]
    public WeaponItem equippedWeapon;

    [Header("Base Stats Snapshot")]
    [SerializeField] int baseMaxHealth;
    [SerializeField] int baseMaxMana;
    [SerializeField] int baseMaxStamina;
    [SerializeField] int baseStrength;
    [SerializeField] int baseDexterity;
    [SerializeField] int baseIntelligence;
    [SerializeField] int baseCharisma;
    [SerializeField] int baseArmorBonus;
    [SerializeField] int baseShieldBonus;

    bool baseStatsCached;

    void Awake()
    {
        if (!Instance)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (playerInventory == null)
            playerInventory = GetComponent<Inventory>();

        if (playerCharacter == null)
            playerCharacter = GetComponent<Character>();

        CacheBaseStats();
        RecalculateStats();
    }

    public void CacheBaseStats()
    {
        if (playerCharacter == null || playerCharacter.stats == null)
            return;

        Stats stats = playerCharacter.stats;

        baseMaxHealth = stats.maxHealth;
        baseMaxMana = stats.maxMana;
        baseMaxStamina = stats.maxStamina;

        baseStrength = stats.strength;
        baseDexterity = stats.dexterity;
        baseIntelligence = stats.intelligence;
        baseCharisma = stats.charisma;

        baseArmorBonus = stats.armorBonus;
        baseShieldBonus = stats.shieldBonus;

        baseStatsCached = true;
    }

    public bool Equip(Item item)
    {
        if (item == null)
            return false;

        if (item is ArmorItem armorItem)
            return EquipArmor(armorItem);

        if (item is WeaponItem weaponItem)
            return EquipWeapon(weaponItem);

        return false;
    }

    public bool EquipArmor(ArmorItem newArmor)
    {
        if (newArmor == null || playerInventory == null)
            return false;

        if (!playerInventory.RemoveItem(newArmor, 1))
            return false;

        ArmorItem oldArmor = GetArmorInSlot(newArmor.armorType);

        if (oldArmor != null)
            playerInventory.AddItem(oldArmor, 1);

        SetArmorInSlot(newArmor.armorType, newArmor);
        RecalculateStats();
        return true;
    }

    public bool EquipWeapon(WeaponItem newWeapon)
    {
        if (newWeapon == null || playerInventory == null)
            return false;

        if (!playerInventory.RemoveItem(newWeapon, 1))
            return false;

        if (equippedWeapon != null)
            playerInventory.AddItem(equippedWeapon, 1);

        equippedWeapon = newWeapon;
        RecalculateStats();
        return true;
    }

    public bool UnequipArmor(ArmorItem.ArmorType armorType)
    {
        if (playerInventory == null)
            return false;

        ArmorItem item = GetArmorInSlot(armorType);
        if (item == null)
            return false;

        playerInventory.AddItem(item, 1);
        SetArmorInSlot(armorType, null);
        RecalculateStats();
        return true;
    }

    public bool UnequipWeapon()
    {
        if (playerInventory == null || equippedWeapon == null)
            return false;

        playerInventory.AddItem(equippedWeapon, 1);
        equippedWeapon = null;
        RecalculateStats();
        return true;
    }

    public int GetEquippedWeaponDamage()
    {
        if (equippedWeapon == null)
            return 0;

        return equippedWeapon.GetDamage();
    }

    public float GetEquippedWeaponAttackSpeed()
    {
        if (equippedWeapon == null)
            return 1f;

        return equippedWeapon.attackSpeed;
    }

    public DamageType GetEquippedWeaponDamageType()
    {
        if (equippedWeapon == null)
            return DamageType.Slashing;

        return equippedWeapon.damageType;
    }

    public void RecalculateStats()
    {
        if (!baseStatsCached)
            CacheBaseStats();

        if (playerCharacter == null || playerCharacter.stats == null)
            return;

        Stats stats = playerCharacter.stats;

        bool healthWasFull = stats.health >= stats.maxHealth;
        bool manaWasFull = stats.mana >= stats.maxMana;
        bool staminaWasFull = stats.stamina >= stats.maxStamina;

        stats.maxHealth = baseMaxHealth;
        stats.maxMana = baseMaxMana;
        stats.maxStamina = baseMaxStamina;

        stats.strength = baseStrength;
        stats.dexterity = baseDexterity;
        stats.intelligence = baseIntelligence;
        stats.charisma = baseCharisma;

        stats.armorBonus = baseArmorBonus;
        stats.shieldBonus = baseShieldBonus;

        ApplyArmor(helmet, stats);
        ApplyArmor(chestplate, stats);
        ApplyArmor(leggings, stats);
        ApplyArmor(boots, stats);
        ApplyWeapon(equippedWeapon, stats);

        if (healthWasFull)
            stats.health = stats.maxHealth;
        else
            stats.health = Mathf.Min(stats.health, stats.maxHealth);

        if (manaWasFull)
            stats.mana = stats.maxMana;
        else
            stats.mana = Mathf.Min(stats.mana, stats.maxMana);

        if (staminaWasFull)
            stats.stamina = stats.maxStamina;
        else
            stats.stamina = Mathf.Min(stats.stamina, stats.maxStamina);
    }

    void ApplyArmor(ArmorItem item, Stats stats)
    {
        if (item == null || stats == null)
            return;

        stats.armorBonus += item.GetDefense();
        stats.strength += item.strength;
        stats.dexterity += item.dexterity;
        stats.intelligence += item.intelligence;
        stats.charisma += item.charisma;
    }

    void ApplyWeapon(WeaponItem item, Stats stats)
    {
        if (item == null || stats == null)
            return;

        stats.strength += item.strength;
        stats.dexterity += item.dexterity;
        stats.intelligence += item.intelligence;
        stats.charisma += item.charisma;
    }

    ArmorItem GetArmorInSlot(ArmorItem.ArmorType armorType)
    {
        switch (armorType)
        {
            case ArmorItem.ArmorType.Helmet:
                return helmet;
            case ArmorItem.ArmorType.Chestplate:
                return chestplate;
            case ArmorItem.ArmorType.Leggings:
                return leggings;
            case ArmorItem.ArmorType.Boots:
                return boots;
        }

        return null;
    }

    void SetArmorInSlot(ArmorItem.ArmorType armorType, ArmorItem item)
    {
        switch (armorType)
        {
            case ArmorItem.ArmorType.Helmet:
                helmet = item;
                break;
            case ArmorItem.ArmorType.Chestplate:
                chestplate = item;
                break;
            case ArmorItem.ArmorType.Leggings:
                leggings = item;
                break;
            case ArmorItem.ArmorType.Boots:
                boots = item;
                break;
        }
    }
}