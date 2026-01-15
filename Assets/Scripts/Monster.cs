using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Character
{
    [Header("Monster Settings")]
    public int xpReward = 25;
    public LootDropper lootDropper;

    public void Awake()
    {
        base.Awake();
    }

    public override void Die()
    {
        if (lootDropper != null)
            lootDropper.DropLoot();

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

