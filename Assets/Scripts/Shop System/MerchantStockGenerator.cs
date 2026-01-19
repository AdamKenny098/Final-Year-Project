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

    void Start()
    {
        GenerateStock();
    }

    public void GenerateStock()
    {
        List<Item> availableItems = new List<Item>();

        if (stockTable != null)
        {
            List<LootTableItem> rolledItems = stockTable.Roll();
            foreach (LootTableItem lootItem in rolledItems)
            {
                availableItems.Add(lootItem.item);
            }
        }

        GenerateStock(availableItems);
    }

    public void GenerateStock(List<Item> availableItems)
    {
        merchantInventory.Clear();

        AddGold(merchantInventory);

        CategoriseItems(availableItems);

        AddWeaponsByType(merchantInventory);
        AddArmorCoverage(merchantInventory);
        AddConsumables(merchantInventory);
        AddRandomExtras(merchantInventory);
    }

    public void CategoriseItems(List<Item> items)
    {
        weapons = new List<WeaponItem>();
        armors = new List<ArmorItem>();
        consumables = new List<ConsumableItem>();
        miscItems = new List<Item>();

        foreach (Item item in items)
        {
            if (item is WeaponItem weapon)
            {
                weapons.Add(weapon);
            }
            else if (item is ArmorItem armor)
            {
                armors.Add(armor);
            }
            else if (item is ConsumableItem consumable)
            {
                consumables.Add(consumable);
            }
            else
            {
                miscItems.Add(item);
            }
        }
    }

    public void AddWeaponsByType(Inventory inventory)
    {
        foreach (WeaponItem.ClassType type in System.Enum.GetValues(typeof(WeaponItem.ClassType)))
        {
            List<WeaponItem> candidates = new List<WeaponItem>();

            foreach (WeaponItem weapon in weapons)
            {
                if (weapon.classType == type)
                {
                    candidates.Add(weapon);
                }
            }

            if (candidates.Count == 0)
                continue;

            WeaponItem chosen = candidates[Random.Range(0, candidates.Count)];
            inventory.AddItem(chosen, 1);
        }
    }

    public void AddArmorCoverage(Inventory inventory)
    {
        foreach (ArmorItem.ArmorType type in System.Enum.GetValues(typeof(ArmorItem.ArmorType)))
        {
            List<ArmorItem> candidates = new List<ArmorItem>();

            foreach (ArmorItem armor in armors)
            {
                if (armor.armorType == type)
                {
                    candidates.Add(armor);
                }
            }

            if (candidates.Count == 0)
                continue;

            ArmorItem chosen = candidates[Random.Range(0, candidates.Count)];
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
}
