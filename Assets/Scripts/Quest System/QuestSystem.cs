using UnityEngine;
using System.Collections.Generic;

public enum QuestState
{
    Inactive,
    Active,
    Completed,
    Failed
}

public class QuestSystem : MonoBehaviour
{
    public static QuestSystem Instance { get; private set; }

    public QuestUIManager questUI;

    public List<QuestData> startingQuests;

    List<QuestInstance> activeQuests = new();
    List<QuestInstance> completedQuests = new();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void RefreshUI()
    {
        if (questUI != null)
            questUI.Refresh();
    }

    void Start()
    {
        foreach (var questData in startingQuests)
            StartQuest(questData);
    }

    public void StartQuest(QuestData data)
    {
        if (data == null)
            return;

        var instance = new QuestInstance(data);
        instance.StartQuest();
        activeQuests.Add(instance);
        RefreshUI();
    }

    public void CompleteQuest(QuestInstance quest)
    {
        Debug.Log($"[QuestSystem] Quest completed: {quest.data.questId}");
        if (!activeQuests.Contains(quest))
            return;

        activeQuests.Remove(quest);
        completedQuests.Add(quest);

        quest.data.reward?.Grant();
        RefreshUI();
    }

    //These snapshots exist bc iterating over the lsits and erasing an element lead to an error
    public void NotifyEnemyKilled(EnemyType enemyType)
    {
        var snapshot = activeQuests.ToArray();
        foreach (var quest in snapshot)
            quest.NotifyEnemyKilled(enemyType);
            RefreshUI();
    }

    public void NotifyItemCollected(string itemId, int amount)
    {
        var snapshot = activeQuests.ToArray();
        foreach (var quest in snapshot)
            quest.NotifyItemCollected(itemId, amount);
            RefreshUI();
    }

    public void NotifyFloorReached(int floorIndex)
    {
        var snapshot = activeQuests.ToArray();
        foreach (var quest in snapshot)
            quest.NotifyFloorReached(floorIndex);
            RefreshUI();
    }

    public void NotifyAreaDiscovered(int floorIndex, string areaId)
    {
        var snapshot = activeQuests.ToArray();
        foreach (var quest in snapshot)
            quest.NotifyAreaDiscovered(floorIndex, areaId);
            RefreshUI();
    }

    public QuestInstance[] GetActiveQuestsSnapshot()
    {
        return activeQuests.ToArray();
    }


}
