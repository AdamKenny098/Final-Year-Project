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

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip tipChangeClip;
    [Range(0f, 1f)] public float tipChangeVolume = 1f;

    float tipTimer;
    int currentTipIndex = -1;

    void OnEnable()
    {
        tipTimer = 0f;
        currentTipIndex = -1;
        SetProgress(0f);
    }

    void Update()
    {
        float progress = 0f;

        if (GameManager.Instance != null && GameManager.Instance.currentLoadOperation != null)
        {
            progress = Mathf.Clamp01(GameManager.Instance.currentLoadOperation.progress / 0.9f);
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
            ShowNextTip(true);
        }
    }

    void SetProgress(float value)
    {
        if (progressSlider != null)
            progressSlider.value = value;
    }

    void SetTip(string tip)
    {
        if (tipText != null)
            tipText.text = tip;
    }

    void ShowNextTip(bool playSound)
    {
        int nextTipIndex = GetNextTipIndex();

        if (nextTipIndex < 0)
        {
            SetTip("");
            return;
        }

        bool changed = nextTipIndex != currentTipIndex;

        currentTipIndex = nextTipIndex;
        SetTip(tips[currentTipIndex]);

        if (playSound && changed && audioSource != null && tipChangeClip != null)
        {
            audioSource.PlayOneShot(tipChangeClip, tipChangeVolume);
        }
    }

    int GetNextTipIndex()
    {
        if (tips == null || tips.Length == 0)
            return -1;

        if (tips.Length == 1)
            return 0;

        int newIndex = currentTipIndex;

        while (newIndex == currentTipIndex)
        {
            newIndex = Random.Range(0, tips.Length);
        }

        return newIndex;
    }
}