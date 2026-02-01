using UnityEngine;

public class QuestUIManager : MonoBehaviour
{
    [Header("UI")]
    public Transform contentRoot;

    [Header("Prefabs")]
    public QuestEntryUI questEntryPrefab;

    public void Refresh()
    {
        if (contentRoot == null || questEntryPrefab == null)
            return;

        for (int i = contentRoot.childCount - 1; i >= 0; i--)
            Destroy(contentRoot.GetChild(i).gameObject);

        if (QuestSystem.Instance == null)
            return;

        var quests = QuestSystem.Instance.GetActiveQuestsSnapshot();

        for (int i = 0; i < quests.Length; i++)
        {
            var entry = Instantiate(questEntryPrefab, contentRoot);
            entry.Bind(quests[i]);
        }
    }
}

