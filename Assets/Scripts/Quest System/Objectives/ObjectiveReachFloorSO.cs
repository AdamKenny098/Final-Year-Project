using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Objectives/Reach Floor")]
public class ObjectiveReachFloorSO : QuestObjectiveSO
{
    public int targetFloor;

    public override void OnFloorReached(QuestInstance quest, int floorIndex)
    {
        if (floorIndex >= targetFloor)
            quest.CompleteObjective(this);
    }
}
