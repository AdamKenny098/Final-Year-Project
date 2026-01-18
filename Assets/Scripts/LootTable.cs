using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Loot/Loot Table")]
public class LootTable : ScriptableObject
{
    public List<LootTableItem> loot = new List<LootTableItem>();

    public List<LootTableItem> Roll()
    {
        List<LootTableItem> results = new List<LootTableItem>();

        int roll = Random.Range(1, 101);

        foreach (LootTableItem entry in loot)
        {
            if (roll >= entry.minRoll && roll <= entry.maxRoll)
            {
                results.Add(entry);
            }
        }

        return results;
    }
}
