using UnityEngine;

[System.Serializable]
public class QuestReward
{
    public int experience;
    public int gold;

    // Placeholder – hook into inventory later
    public void Grant()
    {
        if (experience > 0)
        {
            // PlayerStats.AddXP(experience);
        }

        if (gold > 0)
        {
            // PlayerInventory.AddGold(gold);
        }
    }
}
