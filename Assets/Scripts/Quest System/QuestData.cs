using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Quests/Quest")]
public class QuestData : ScriptableObject
{
    public string questId;
    public string title;

    [TextArea]
    public string description;
    public List<QuestObjectiveSO> objectives = new();

    public QuestReward reward;
}
