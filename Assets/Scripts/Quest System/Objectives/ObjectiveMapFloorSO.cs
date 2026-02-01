using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Objectives/Map Floor")]
public class ObjectiveMapFloorSO : QuestObjectiveSO
{
    public int targetFloor;
    public int totalAreas;

    public override void OnAreaDiscovered(QuestInstance quest, int floorIndex, string areaId)
    {
        if (floorIndex != targetFloor)
            return;

        quest.AddUniqueProgress(this, areaId, totalAreas);
    }

    public override string GetUIText(QuestInstance quest)
    {
        int current = quest.GetProgress(this);
        if (current == int.MaxValue) current = totalAreas;
        return $"Map Floor {targetFloor} ({current}/{totalAreas})";
    }
}
