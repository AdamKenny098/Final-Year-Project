using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Loot/Loot Table")]
public class LootTable : ScriptableObject
{
    public List<LootTableItem> loot = new List<LootTableItem>();

    public List<LootTableItem> Roll()
    {
        List<LootTableItem> results = new List<LootTableItem>();

        foreach (LootTableItem entry in loot)
        {
            int roll = Random.Range(1, 101);
            if (roll >= entry.minRoll && roll <= entry.maxRoll)
            {
                results.Add(entry);
            }
        }

        return results;
    }

    public List<Item> GetAllItems()
    {
        List<Item> items = new List<Item>();

        foreach (LootTableItem entry in loot)
        {
            if (entry.item != null)
            {
                items.Add(entry.item);
            }
        }

        return items;
    }
}
