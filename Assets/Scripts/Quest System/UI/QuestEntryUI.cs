using UnityEngine;
using TMPro;

public class QuestEntryUI : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text descriptionText;

    public Transform objectivesRoot;
    public QuestObjectiveUI objectiveEntryPrefab;

    public void Bind(QuestInstance quest)
    {
        if (quest == null || quest.data == null)
            return;

        if (titleText != null) titleText.text = quest.data.title;
        if (descriptionText != null) descriptionText.text = quest.data.description;

        if (objectivesRoot == null || objectiveEntryPrefab == null)
            return;

        for (int i = objectivesRoot.childCount - 1; i >= 0; i--)
            Destroy(objectivesRoot.GetChild(i).gameObject);

        var objs = quest.data.objectives;
        for (int i = 0; i < objs.Count; i++)
        {
            var obj = objs[i];
            if (obj == null) continue;

            var line = Instantiate(objectiveEntryPrefab, objectivesRoot);
            line.Bind(quest, obj);
        }
    }
}

