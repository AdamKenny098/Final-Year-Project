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
}
