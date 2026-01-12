using UnityEngine;

[System.Serializable]
public class LootTableItem
{
    public Item item;
    [Range(0, 100)] public int minRoll;
    [Range(0, 100)] public int maxRoll;

    public int minAmount;
    public int maxAmount;
}

