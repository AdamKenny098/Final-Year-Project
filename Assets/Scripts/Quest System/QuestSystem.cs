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

    Dictionary<int, int> floorAreaCounts = new();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        foreach (var questData in startingQuests)
            StartQuest(questData);
    }

    void RefreshUI()
    {
        if (questUI != null)
            questUI.Refresh();
    }

    public void StartQuest(QuestData data)
    {
        if (data == null)
            return;

        if (string.IsNullOrEmpty(data.questId))
        {
            Debug.LogWarning("[QuestSystem] Starting quest failed because questId is empty: " + data.name);
            return;
        }

        if (IsQuestActive(data.questId) || IsQuestCompleted(data.questId))
            return;

        var instance = new QuestInstance(data);
        instance.StartQuest();
        activeQuests.Add(instance);
        RefreshUI();
    }

    public bool TryStartQuest(QuestData questData)
    {
        if (questData == null)
            return false;

        if (string.IsNullOrEmpty(questData.questId))
        {
            Debug.LogWarning("[QuestSystem] Tried to start a quest with an empty questId: " + questData.name);
            return false;
        }

        if (IsQuestActive(questData.questId))
        {
            Debug.Log("[QuestSystem] Quest already active: " + questData.questId);
            return false;
        }

        if (IsQuestCompleted(questData.questId))
        {
            Debug.Log("[QuestSystem] Quest already completed: " + questData.questId);
            return false;
        }

        QuestInstance newQuest = new QuestInstance(questData);
        newQuest.StartQuest();
        activeQuests.Add(newQuest);

        Debug.Log("[QuestSystem] Started quest: " + questData.questId);

        RefreshUI();
        return true;
    }

    public void CompleteQuest(QuestInstance quest)
    {
        if (quest == null || quest.data == null)
            return;

        Debug.Log("[QuestSystem] Quest completed: " + quest.data.questId);

        if (!activeQuests.Contains(quest))
            return;

        activeQuests.Remove(quest);
        completedQuests.Add(quest);

        quest.data.reward?.Grant();
        RefreshUI();
    }

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

    public void RegisterFloorAreaCount(int floorIndex, int totalAreas)
    {
        floorAreaCounts[floorIndex] = totalAreas;
        RefreshUI();
    }

    public int GetFloorAreaCount(int floorIndex)
    {
        return floorAreaCounts.TryGetValue(floorIndex, out var count) ? count : 0;
    }

    public bool IsQuestActive(string questId)
    {
        if (string.IsNullOrEmpty(questId))
            return false;

        for (int i = 0; i < activeQuests.Count; i++)
        {
            var q = activeQuests[i];
            if (q != null && q.data != null && q.data.questId == questId)
                return true;
        }

        return false;
    }

    public bool IsQuestCompleted(string questId)
    {
        if (string.IsNullOrEmpty(questId))
            return false;

        for (int i = 0; i < completedQuests.Count; i++)
        {
            var q = completedQuests[i];
            if (q != null && q.data != null && q.data.questId == questId)
                return true;
        }

        return false;
    }
}