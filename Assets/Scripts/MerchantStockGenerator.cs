using UnityEngine;
using System.Collections.Generic;

public class MerchantStockGenerator : MonoBehaviour
{
    public LootTable stockTable;
    public Inventory merchantInventory;

    void Start()
    {
        GenerateStock();
    }


    public void GenerateStock()
    {
        if (stockTable == null || merchantInventory == null)
            return;

        merchantInventory.Clear();

        List<LootTableItem> results = stockTable.Roll();

        foreach (LootTableItem entry in results)
        {
            int amount = Random.Range(entry.minAmount, entry.maxAmount + 1);

            merchantInventory.AddItem(entry.item, amount);
        }
    }
}
