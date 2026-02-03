using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Objectives/Map Floor")]
public class ObjectiveMapFloorSO : QuestObjectiveSO
{
    public int targetFloor;
    public int totalAreasOverride = 0;

    int GetRequired()
    {
        if (totalAreasOverride > 0)
            return totalAreasOverride;

        if (QuestSystem.Instance == null)
            return 0;

        return QuestSystem.Instance.GetFloorAreaCount(targetFloor);
    }

    public override void OnAreaDiscovered(QuestInstance quest, int floorIndex, string areaId)
    {
        if (floorIndex != targetFloor)
            return;

        int required = GetRequired();
        if (required <= 0)
            return;

        quest.AddUniqueProgress(this, areaId, required);
    }

    public override string GetUIText(QuestInstance quest)
    {
        int required = GetRequired();
        int current = quest.GetProgress(this);

        if (current == int.MaxValue && required > 0)
            current = required;

        if (required <= 0)
            return $"Map Floor {targetFloor}";

        return $"Map Floor {targetFloor} ({current}/{required})";
    }
}
