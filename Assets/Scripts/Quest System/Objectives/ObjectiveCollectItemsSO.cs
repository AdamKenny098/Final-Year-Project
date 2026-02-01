using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Objectives/Collect Items")]
public class ObjectiveCollectItemsSO : QuestObjectiveSO
{
    public string itemId;
    public int requiredAmount = 1;

    public override void OnItemCollected(QuestInstance quest, string collectedItemId, int amount)
    {
        if (collectedItemId != itemId)
            return;

        quest.AddProgress(this, amount, requiredAmount);
    }

    public override string GetUIText(QuestInstance quest)
    {
        int current = quest.GetProgress(this);
        if (current == int.MaxValue) current = requiredAmount;
        return $"Collect {requiredAmount} {itemId} ({current}/{requiredAmount})";
    }
}
