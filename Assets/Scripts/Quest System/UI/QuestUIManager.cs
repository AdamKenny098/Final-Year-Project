using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUIManager : MonoBehaviour
{
    [Header("Root")]
    public GameObject root;

    [Header("Header")]
    public TMP_Text headerText;

    [Header("Quest List")]
    public Transform questListRoot;
    public Button questButtonPrefab;

    [Header("Quest Details")]
    public TMP_Text questTitleText;
    public TMP_Text questDescriptionText;
    public TMP_Text rewardText;
    public Transform objectiveListRoot;
    public TMP_Text objectiveLinePrefab;

    public List<GameObject> spawnedButtons = new();
    public List<GameObject> spawnedObjectiveLines = new();

    public QuestInstance[] currentQuests = System.Array.Empty<QuestInstance>();
    public QuestInstance selectedQuest;

    void Awake()
    {
        if (root != null)
            root.SetActive(false);
    }

    void OnEnable()
    {
        Refresh();
    }

    public void Open()
    {
        if (root != null)
            root.SetActive(true);

        Refresh();
    }

    public void Close()
    {
        if (root != null)
            root.SetActive(false);

        currentQuests = System.Array.Empty<QuestInstance>();
        selectedQuest = null;

        ClearSpawnedButtons();
        ClearObjectiveLines();
        ClearDetails();
    }

    public void Refresh()
    {
        if (QuestSystem.Instance == null)
        {
            currentQuests = System.Array.Empty<QuestInstance>();
            selectedQuest = null;

            RebuildQuestButtons();
            ClearDetails();
            return;
        }

        currentQuests = QuestSystem.Instance.GetActiveQuestsSnapshot();

        if (headerText != null)
            headerText.text = "Quest Journal";

        QuestInstance previousSelection = selectedQuest;
        selectedQuest = null;

        RebuildQuestButtons();

        if (currentQuests.Length == 0)
        {
            ClearDetails();
            return;
        }

        if (previousSelection != null && previousSelection.data != null)
        {
            string previousId = previousSelection.data.questId;

            for (int i = 0; i < currentQuests.Length; i++)
            {
                QuestInstance q = currentQuests[i];
                if (q != null && q.data != null && q.data.questId == previousId)
                {
                    SelectQuest(q);
                    return;
                }
            }
        }

        SelectQuest(currentQuests[0]);
    }

    void RebuildQuestButtons()
    {
        ClearSpawnedButtons();

        if (questListRoot == null || questButtonPrefab == null)
            return;

        for (int i = 0; i < currentQuests.Length; i++)
        {
            QuestInstance quest = currentQuests[i];
            if (quest == null || quest.data == null)
                continue;

            Button button = Instantiate(questButtonPrefab, questListRoot, false);
            spawnedButtons.Add(button.gameObject);

            TMP_Text label = button.GetComponentInChildren<TMP_Text>();
            if (label != null)
                label.text = quest.data.title;

            QuestInstance capturedQuest = quest;
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

    void ClearObjectiveLines()
    {
        for (int i = 0; i < spawnedObjectiveLines.Count; i++)
        {
            if (spawnedObjectiveLines[i] != null)
                Destroy(spawnedObjectiveLines[i]);
        }

        spawnedObjectiveLines.Clear();

        if (objectiveListRoot == null)
            return;

        for (int i = objectiveListRoot.childCount - 1; i >= 0; i--)
            Destroy(objectiveListRoot.GetChild(i).gameObject);
    }

    void ClearDetails()
    {
        if (questTitleText != null)
            questTitleText.text = "No Active Quest";

        if (questDescriptionText != null)
            questDescriptionText.text = "";

        if (rewardText != null)
            rewardText.text = "";

        ClearObjectiveLines();
    }

    public void SelectQuest(QuestInstance quest)
    {
        selectedQuest = quest;

        if (selectedQuest == null || selectedQuest.data == null)
        {
            ClearDetails();
            return;
        }

        QuestData data = selectedQuest.data;

        if (questTitleText != null)
            questTitleText.text = data.title;

        if (questDescriptionText != null)
            questDescriptionText.text = data.description;

        if (rewardText != null)
        {
            if (data.reward != null)
                rewardText.text = $"Reward: {data.reward.experience} XP, {data.reward.gold} Gold";
            else
                rewardText.text = "Reward: None";
        }

        RebuildObjectiveLines(selectedQuest);
    }

    void RebuildObjectiveLines(QuestInstance quest)
    {
        ClearObjectiveLines();

        if (quest == null || quest.data == null)
            return;

        if (objectiveListRoot == null || objectiveLinePrefab == null)
            return;

        List<QuestObjectiveSO> objectives = quest.data.objectives;
        if (objectives == null)
            return;

        for (int i = 0; i < objectives.Count; i++)
        {
            QuestObjectiveSO objective = objectives[i];
            if (objective == null)
                continue;

            TMP_Text line = Instantiate(objectiveLinePrefab, objectiveListRoot, false);
            spawnedObjectiveLines.Add(line.gameObject);
            line.text = objective.GetUIText(quest);
        }
    }
}