using UnityEngine;
using System.Collections.Generic;


public class MerchantStockGenerator : MonoBehaviour
{
    public LootTable stockTable;
    public Inventory merchantInventory;
    public int minHealthPotions = 3;
    public int minManaPotions = 2;
    public int extraItemCount = 3;

    [Header("Stock Sub-Sections")]
    public List<WeaponItem> weapons;
    public List<ArmorItem> armors;
    public List<ConsumableItem> consumables;
    public List<Item> miscItems;
    [Header("Gold")]
    public Item goldItem;
    public int minGold = 50;
    public int maxGold = 500;

    [Header("Rarity Roll Chances")]
    [Range(0, 100)] public int commonChance = 50;
    [Range(0, 100)] public int uncommonChance = 25;
    [Range(0, 100)] public int rareChance = 15;
    [Range(0, 100)] public int epicChance = 8;
    [Range(0, 100)] public int legendaryChance = 2;

    void Start()
    {
        GenerateStock();
    }

    public void GenerateStock()
    {
        merchantInventory.Clear();

        AddGold(merchantInventory);

        AddWeaponsByType(merchantInventory);
        AddArmorCoverage(merchantInventory);
        AddConsumables(merchantInventory);
        AddRandomExtras(merchantInventory);
    }

    public void AddWeaponsByType(Inventory inventory)
    {
        foreach (WeaponItem.ClassType type in System.Enum.GetValues(typeof(WeaponItem.ClassType)))
        {
            int targetRarity = RollTargetRarity();
            WeaponItem chosen = GetRandomWeaponByClassAndRarity(type, targetRarity);

            if (chosen != null)
                inventory.AddItem(chosen, 1);
        }
    }

    public void AddArmorCoverage(Inventory inventory)
    {
        foreach (ArmorItem.ArmorType type in System.Enum.GetValues(typeof(ArmorItem.ArmorType)))
        {
            int targetRarity = RollTargetRarity();
            ArmorItem chosen = GetRandomArmorByTypeAndRarity(type, targetRarity);

            if (chosen != null)
                inventory.AddItem(chosen, 1);
        }
    }

    public void AddConsumables(Inventory inventory)
    {
        AddConsumableType(
            ConsumableItem.ConsumableType.HealthPotion,
            minHealthPotions,
            inventory
        );

        AddConsumableType(
            ConsumableItem.ConsumableType.ManaPotion,
            minManaPotions,
            inventory
        );
    }

    public void AddConsumableType(ConsumableItem.ConsumableType type, int count, Inventory inventory)
    {
        List<ConsumableItem> candidates = new List<ConsumableItem>();
        foreach (ConsumableItem consumable in consumables)
        {
            if (consumable.consumableType == type)
            {
                candidates.Add(consumable);
            }
        }

        if (candidates.Count == 0)
            return;

        for (int i = 0; i < count; i++)
        {
            ConsumableItem chosen =
                    candidates[Random.Range(0, candidates.Count)];
            inventory.AddItem(chosen, 1);
        }
    }

    void AddRandomExtras(Inventory inventory)
    {
        for (int i = 0; i < extraItemCount; i++)
        {
            if (miscItems.Count == 0)
                return;

            Item chosen = miscItems[Random.Range(0, miscItems.Count)];
            inventory.AddItem(chosen, 1);
        }
    }

    void AddGold(Inventory inventory)
    {
        if (goldItem == null)
            return;

        int amount = Random.Range(minGold, maxGold + 1);
        inventory.AddItem(goldItem, amount);
    }

    int RollTargetRarity()
    {
        int roll = Random.Range(1, 101);

        if (roll <= commonChance)
            return 1;

        roll -= commonChance;
        if (roll <= uncommonChance)
            return 2;

        roll -= uncommonChance;
        if (roll <= rareChance)
            return 3;

        roll -= rareChance;
        if (roll <= epicChance)
            return 4;

        return 5;
    }

    WeaponItem GetRandomWeaponByClassAndRarity(WeaponItem.ClassType type, int targetRarity)
    {
        List<WeaponItem> exactMatches = new List<WeaponItem>();
        List<WeaponItem> fallbackMatches = new List<WeaponItem>();

        foreach (WeaponItem weapon in weapons)
        {
            if (weapon.classType != type)
                continue;

            fallbackMatches.Add(weapon);

            if (weapon.rarity == targetRarity)
                exactMatches.Add(weapon);
        }

        if (exactMatches.Count > 0)
            return exactMatches[Random.Range(0, exactMatches.Count)];

        if (fallbackMatches.Count > 0)
            return fallbackMatches[Random.Range(0, fallbackMatches.Count)];

        return null;
    }

    ArmorItem GetRandomArmorByTypeAndRarity(ArmorItem.ArmorType type, int targetRarity)
    {
        List<ArmorItem> exactMatches = new List<ArmorItem>();
        List<ArmorItem> fallbackMatches = new List<ArmorItem>();

        foreach (ArmorItem armor in armors)
        {
            if (armor.armorType != type)
                continue;

            fallbackMatches.Add(armor);

            if (armor.rarity == targetRarity)
                exactMatches.Add(armor);
        }

        if (exactMatches.Count > 0)
            return exactMatches[Random.Range(0, exactMatches.Count)];

        if (fallbackMatches.Count > 0)
            return fallbackMatches[Random.Range(0, fallbackMatches.Count)];

        return null;
    }
}
