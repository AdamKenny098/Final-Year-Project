using System.Collections;
using UnityEngine;
using TMPro;

public class TradeFeedbackUI : MonoBehaviour
{
    public static TradeFeedbackUI Instance;

    public CanvasGroup group;
    public TMP_Text messageText;
    public float displayTime = 1.5f;

    private Coroutine currentRoutine;

    void Awake()
    {
        Instance = this;
        group.alpha = 0f;
    }

    public void Show(string message)
    {
        messageText.text = message;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowRoutine());
    }

    IEnumerator ShowRoutine()
    {
        group.alpha = 1f;
        yield return new WaitForSeconds(displayTime);
        group.alpha = 0f;
        currentRoutine = null;
    }
}

