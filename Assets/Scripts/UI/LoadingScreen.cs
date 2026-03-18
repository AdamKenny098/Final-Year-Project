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
        float progress = 0f;

        if (MenuController.Instance != null && MenuController.Instance.currentLoadOperation != null)
        {
            progress = Mathf.Clamp01(MenuController.Instance.currentLoadOperation.progress / 0.9f);
        }
        else if (LabyrinthManager.Instance != null && LabyrinthManager.Instance.isLoadingFloor)
        {
            progress = LabyrinthManager.Instance.floorLoadProgress;
        }

        SetProgress(progress);

        tipTimer += Time.unscaledDeltaTime;
        if (tips != null && tips.Length > 0 && tipTimer >= tipRotateSeconds)
        {
            tipTimer = 0f;
            SetTip(GetRandomTip());
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
