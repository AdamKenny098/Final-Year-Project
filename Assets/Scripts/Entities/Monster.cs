using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Goblin,
    Skeleton,
    Slime
}

public class Monster : Character
{
    [SerializeField] private EnemyType enemyType;
    [Header("Monster Settings")]
    public int xpReward = 25;
    public LootDropper lootDropper;

    public override void Awake()
    {
        base.Awake();
    }

    public override void Die()
    {
        if (lootDropper != null)
            lootDropper.DropLoot();

        if (QuestSystem.Instance != null)
            QuestSystem.Instance.NotifyEnemyKilled(enemyType);

            
        GrantXP();
    }

    void GrantXP()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        Character playerCharacter = player.GetComponentInParent<Character>();
        if (playerCharacter == null) return;

        playerCharacter.AddXP(xpReward);
    }
}

