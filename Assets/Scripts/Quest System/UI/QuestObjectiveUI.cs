using UnityEngine;
using TMPro;

public class QuestObjectiveUI : MonoBehaviour
{
    public TMP_Text objectiveText;

    public void Bind(QuestInstance quest, QuestObjectiveSO obj)
    {
        if (quest == null || obj == null || objectiveText == null)
            return;

        objectiveText.text = obj.GetUIText(quest);
    }
}
