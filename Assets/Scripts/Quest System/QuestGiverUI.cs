using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestGiverUI : MonoBehaviour
{
    public static QuestGiverUI Instance;

    [Header("Root")]
    public GameObject root;

    [Header("NPC Info")]
    public TMP_Text npcNameText;

    [Header("Quest List")]
    public Transform questListRoot;
    public Button questButtonPrefab;

    [Header("Quest Details")]
    public TMP_Text questTitleText;
    public TMP_Text questDescriptionText;
    public TMP_Text rewardText;
    public Transform objectiveListRoot;
    public TMP_Text objectiveLinePrefab;

    [Header("Buttons")]
    public Button acceptButton;
    public Button closeButton;

    QuestGiverNPC currentNPC;
    List<QuestData> currentQuests = new();
    QuestData selectedQuest;

    public List<GameObject> spawnedButtons = new();
    public List<GameObject> spawnedObjectiveLines = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (acceptButton != null)
            acceptButton.onClick.AddListener(AcceptSelectedQuest);

        if (closeButton != null)
            closeButton.onClick.AddListener(Close);

        if (root != null)
            root.SetActive(false);
    }

    public void Open(QuestGiverNPC npc, List<QuestData> quests)
    {
        currentNPC = npc;
        currentQuests = quests ?? new List<QuestData>();
        selectedQuest = null;

        if (root != null)
            root.SetActive(true);

        if (npcNameText != null)
            npcNameText.text = npc != null ? npc.firstName + " " + npc.lastName : "Quest Giver";

        RebuildQuestButtons();

        if (currentQuests.Count > 0)
            SelectQuest(currentQuests[0]);
        else
            ClearDetails();
    }

    public void Close()
    {
        if (root != null)
            root.SetActive(false);

        currentNPC = null;
        currentQuests.Clear();
        selectedQuest = null;

        ClearSpawnedButtons();

        if (DialogueSystem.Instance != null)
            DialogueSystem.Instance.ResumeDialogue();
    }

    void RefreshAvailableQuests()
    {
        if (currentNPC == null)
        {
            Close();
            return;
        }

        currentQuests = currentNPC.GetAvailableQuests();
        selectedQuest = null;

        RebuildQuestButtons();

        if (currentQuests.Count > 0)
            SelectQuest(currentQuests[0]);
        else
            Close();
    }

    void RebuildQuestButtons()
    {
        ClearSpawnedButtons();

        if (questListRoot == null || questButtonPrefab == null)
            return;

        for (int i = 0; i < currentQuests.Count; i++)
        {
            QuestData quest = currentQuests[i];
            if (quest == null) continue;

            Button button = Instantiate(questButtonPrefab, questListRoot);
            spawnedButtons.Add(button.gameObject);

            TMP_Text label = button.GetComponentInChildren<TMP_Text>();
            if (label != null)
                label.text = quest.title;

            QuestData capturedQuest = quest;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => SelectQuest(capturedQuest));
        }
    }

    void ClearSpawnedButtons()
    {
        for (int i = 0; i < spawnedButtons.Count; i++)
        {
            if (spawnedButtons[i] != null)
                Destroy(spawnedButtons[i]);
        }

        spawnedButtons.Clear();
    }

    void ClearDetails()
    {
        if (questTitleText != null) questTitleText.text = "No Quest Selected";
        if (questDescriptionText != null) questDescriptionText.text = "";
        if (rewardText != null) rewardText.text = "";

        if (acceptButton != null)
            acceptButton.interactable = false;
    }

    public void SelectQuest(QuestData quest)
    {
        selectedQuest = quest;

        if (selectedQuest == null)
        {
            ClearDetails();
            return;
        }

        if (questTitleText != null)
            questTitleText.text = selectedQuest.title;

        if (questDescriptionText != null)
            questDescriptionText.text = selectedQuest.description;

        if (rewardText != null)
        {
            if (selectedQuest.reward != null)
                rewardText.text = $"Reward: {selectedQuest.reward.experience} XP, {selectedQuest.reward.gold} Gold";
            else
                rewardText.text = "Reward: None";
        }

        if (acceptButton != null)
            acceptButton.interactable = true;
    }

    void AcceptSelectedQuest()
    {
        if (selectedQuest == null || QuestSystem.Instance == null)
            return;

        QuestData acceptedQuest = selectedQuest;

        bool started = QuestSystem.Instance.TryStartQuest(acceptedQuest);
        if (!started)
            return;

        RefreshAvailableQuests();
    }
}