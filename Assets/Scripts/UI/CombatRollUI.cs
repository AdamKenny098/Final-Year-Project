using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatRollUI : MonoBehaviour
{
    public static CombatRollUI Instance;

    [Header("References")]
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text rollText;
    [SerializeField] TMP_Text resultText;

    [Header("Timing")]
    [SerializeField] float visibleDuration = 1.2f;
    [SerializeField] float fadeDuration = 0.25f;

    Coroutine showRoutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void ShowAttackRoll(string abilityName, int d20, int bonus, int total, int targetNumber, RollOutcome outcome, int damage)
    {
        string title = string.IsNullOrWhiteSpace(abilityName) ? "Attack" : abilityName;
        string line1 = "Roll: " + d20 + " + " + bonus + " = " + total + " vs AC " + targetNumber;

        string line2 = "";
        if (outcome == RollOutcome.Crit)
            line2 = "CRIT for " + damage;
        else if (outcome == RollOutcome.Hit)
            line2 = "HIT for " + damage;
        else
            line2 = "MISS";

        Show(title, line1, line2);
    }

    public void ShowSaveRoll(string abilityName, SaveType saveType, int d20, int bonus, int total, int targetNumber, bool saved, int damage)
    {
        string title = string.IsNullOrWhiteSpace(abilityName) ? "Ability" : abilityName;
        string saveName = saveType.ToString();
        string line1 = saveName + " Save: " + d20 + " + " + bonus + " = " + total + " vs DC " + targetNumber;

        string line2 = "";
        if (saved)
            line2 = damage > 0 ? "SAVED, " + damage + " damage" : "SAVED";
        else
            line2 = "FAILED, " + damage + " damage";

        Show(title, line1, line2);
    }

    void Show(string title, string line1, string line2)
    {
        if (titleText != null) titleText.text = title;
        if (rollText != null) rollText.text = line1;
        if (resultText != null) resultText.text = line2;

        if (showRoutine != null)
            StopCoroutine(showRoutine);

        showRoutine = StartCoroutine(ShowRoutine());
    }

    IEnumerator ShowRoutine()
    {
        if (canvasGroup == null)
            yield break;

        canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(visibleDuration);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        showRoutine = null;
    }
}