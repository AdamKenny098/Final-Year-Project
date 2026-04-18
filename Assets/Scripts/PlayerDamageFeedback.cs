using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDamageFeedback : MonoBehaviour
{
    [SerializeField] Image damageOverlay;
    [SerializeField] float maxAlpha = 0.45f;
    [SerializeField] float fadeDuration = 0.35f;
    [SerializeField] Color flashColor = new Color(0.75f, 0f, 0f, 1f);

    Coroutine flashRoutine;

    void Awake()
    {
        if (damageOverlay != null)
        {
            Color c = flashColor;
            c.a = 0f;
            damageOverlay.color = c;
            damageOverlay.raycastTarget = false;
        }
    }

    public void PlayDamageFlash(float intensity01)
    {
        if (damageOverlay == null)
            return;

        intensity01 = Mathf.Clamp01(intensity01);
        float targetAlpha = Mathf.Lerp(0.12f, maxAlpha, intensity01);

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashRoutine(targetAlpha));
    }

    IEnumerator FlashRoutine(float startAlpha)
    {
        Color c = flashColor;
        c.a = startAlpha;
        damageOverlay.color = c;

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            c.a = Mathf.Lerp(startAlpha, 0f, t);
            damageOverlay.color = c;

            yield return null;
        }

        c.a = 0f;
        damageOverlay.color = c;
        flashRoutine = null;
    }
}