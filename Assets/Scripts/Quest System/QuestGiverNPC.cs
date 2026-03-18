using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class QuestGiverNPC : NPC, IInteractable
{
    [Header("Quest Giver")]
    public List<QuestData> offeredQuests = new List<QuestData>();

    public List<QuestData> GetAvailableQuests()
    {
        List<QuestData> result = new List<QuestData>();

        if (QuestSystem.Instance == null)
            return result;

        for (int i = 0; i < offeredQuests.Count; i++)
        {
            QuestData quest = offeredQuests[i];
            if (quest == null) continue;

            if (QuestSystem.Instance.IsQuestActive(quest.questId))
                continue;

            if (QuestSystem.Instance.IsQuestCompleted(quest.questId))
                continue;

            result.Add(quest);
        }

        return result;
    }

    public virtual void Interact()
    {
        if (dialogueInkJSON == null) return;

        DialogueSystem.Instance.StartDialogue(this);
        GameStates.Instance.SetState(GameState.Talking);

        var player = FindObjectOfType<FirstPersonController>();
        if (player != null)
            player.FaceTarget(transform);
    }
}