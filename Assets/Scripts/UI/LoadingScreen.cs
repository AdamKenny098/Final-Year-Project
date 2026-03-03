using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    [Header("UI")]
    public Slider progressSlider;
    public TMP_Text tipText;

    [Header("Tips")]
    [TextArea(2, 4)]
    public string[] tips;

    public float tipRotateSeconds = 2.5f;

    float tipTimer;

    void OnEnable()
    {
        tipTimer = 0f;
        SetTip(GetRandomTip());
        SetProgress(0f);
    }

    void Update()
    {
        var operation = MenuController.Instance.currentLoadOperation;
        float progress = Mathf.Clamp01(operation.progress / 0.9f); //Only values between 0 and 1
        SetProgress(progress);

        tipTimer += Time.unscaledDeltaTime;
        if (tips != null && tips.Length > 0 && tipTimer >= tipRotateSeconds)
        {
            tipTimer = 0f;
            SetTip(GetRandomTip());
        }

        if (operation.progress >= 0.9f)
        {
            SetProgress(1f);
        }
    }

    void SetProgress(float value)
    {
        if (progressSlider) progressSlider.value = value;
    }

    void SetTip(string tip)
    {
        if (tipText) tipText.text = tip;
    }

    string GetRandomTip()
    {
        if (tips == null || tips.Length == 0) return "";
        return tips[Random.Range(0, tips.Length)];
    }
}
