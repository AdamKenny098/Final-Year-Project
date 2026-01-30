using System.Collections.Generic;

public class QuestInstance
{
    public QuestData data;
    public QuestState State { get; private set; }

    Dictionary<QuestObjectiveSO, int> progress = new();
    Dictionary<QuestObjectiveSO, HashSet<string>> uniqueProgress = new();

    public QuestInstance(QuestData data)
    {
        this.data = data;
        State = QuestState.Inactive;

        for (int i = 0; i < data.objectives.Count; i++)
        {
            var obj = data.objectives[i];
            if (obj == null) continue;

            if (!progress.ContainsKey(obj))
                progress.Add(obj, 0);
        }
    }

    public void StartQuest()
    {
        State = QuestState.Active;

        foreach (var obj in data.objectives)
        {
            if (obj == null) continue;
            obj.OnStart(this);
        }
    }

    public void AddProgress(QuestObjectiveSO obj, int amount, int required)
    {
        if (State != QuestState.Active)
            return;

        if (!progress.ContainsKey(obj))
            progress[obj] = 0;

        progress[obj] += amount;

        if (progress[obj] >= required)
            CompleteObjective(obj);
    }

    public void AddUniqueProgress(QuestObjectiveSO obj, string uniqueId, int required)
    {
        if (State != QuestState.Active)
            return;

        if (!uniqueProgress.TryGetValue(obj, out var set))
        {
            set = new HashSet<string>();
            uniqueProgress[obj] = set;
        }

        if (!set.Add(uniqueId))
            return;

        AddProgress(obj, 1, required);
    }

    public void CompleteObjective(QuestObjectiveSO obj)
    {
        if (State != QuestState.Active)
            return;

        // Mark as complete by forcing progress high
        progress[obj] = int.MaxValue;

        CheckCompletion();
    }

    void CheckCompletion()
    {
        foreach (var obj in data.objectives)
        {
            if (obj == null) continue;

            if (!progress.TryGetValue(obj, out var value))
                return;

            if (value <= 0)
                return;
        }

        State = QuestState.Completed;
        QuestSystem.Instance.CompleteQuest(this);
    }

    // ===== Fact routing =====

    public void NotifyEnemyKilled(EnemyType enemyType)
    {
        if (State != QuestState.Active)
            return;

        foreach (var obj in data.objectives)
        {
            if (obj == null) continue;
            obj.OnEnemyKilled(this, enemyType);
        }
    }

    public void NotifyItemCollected(string itemId, int amount)
    {
        if (State != QuestState.Active)
            return;

        foreach (var obj in data.objectives)
        {
            if (obj == null) continue;
            obj.OnItemCollected(this, itemId, amount);
        }
    }

    public void NotifyFloorReached(int floorIndex)
    {
        if (State != QuestState.Active)
            return;

        foreach (var obj in data.objectives)
        {
            if (obj == null) continue;
            obj.OnFloorReached(this, floorIndex);
        }
    }

    public void NotifyAreaDiscovered(int floorIndex, string areaId)
    {
        if (State != QuestState.Active)
            return;

        foreach (var obj in data.objectives)
        {
            if (obj == null) continue;
            obj.OnAreaDiscovered(this, floorIndex, areaId);
        }
    }
}
