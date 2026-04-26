using UnityEngine;

[System.Serializable]
public class QuestReward
{
    public int experience;
    public int gold;

    public void Grant(Character playerCharacter, Inventory playerInventory, Item goldItem)
    {
        if (experience > 0)
        {
            if (playerCharacter != null)
            {
                playerCharacter.AddXP(experience);
            }
            else
            {
                Debug.LogWarning("[QuestReward] Cannot grant XP because playerCharacter is missing.");
            }
        }

        if (gold > 0)
        {
            if (playerInventory != null && goldItem != null)
            {
                playerInventory.AddItem(goldItem, gold);
            }
            else
            {
                Debug.LogWarning("[QuestReward] Cannot grant gold because playerInventory or goldItem is missing.");
            }
        }

        Debug.Log($"[QuestReward] Granted {experience} XP and {gold} gold.");
    }
}