using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Objectives/Slay Enemy")]
public class ObjectiveSlayEnemySO : QuestObjectiveSO
{
    public EnemyType targetEnemy;
    public int requiredKills = 1;

    public override void OnEnemyKilled(QuestInstance quest, EnemyType enemyType)
    {
        if (enemyType != targetEnemy)
            return;

        quest.AddProgress(this, 1, requiredKills);
    }

    public override string GetUIText(QuestInstance quest)
    {
        int current = quest.GetProgress(this);
        if (current == int.MaxValue) current = requiredKills;
        return $"Slay {requiredKills} {targetEnemy} ({current}/{requiredKills})";
    }
}
