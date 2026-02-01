using UnityEngine;

public abstract class QuestObjectiveSO : ScriptableObject
{
    public virtual void OnStart(QuestInstance quest) { }

    public virtual void OnEnemyKilled(QuestInstance quest, EnemyType enemyType) { }
    public virtual void OnItemCollected(QuestInstance quest, string itemId, int amount) { }
    public virtual void OnFloorReached(QuestInstance quest, int floorIndex) { }
    public virtual void OnAreaDiscovered(QuestInstance quest, int floorIndex, string areaId) { }
    public virtual string GetUIText(QuestInstance quest) => name;

}
