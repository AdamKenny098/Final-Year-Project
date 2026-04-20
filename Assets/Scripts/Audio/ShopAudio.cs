using UnityEngine;

public class ShopAudio : MonoBehaviour
{
    [Header("Source")]
    [SerializeField] private AudioSource audioSource;

    [Header("Clips")]
    [SerializeField] private AudioClip sellClip;
    [SerializeField] private AudioClip shopSuccessClip;
    [SerializeField] private AudioClip shopFailureClip;
    [SerializeField] private AudioClip selectItemClip;

    [Header("Volume")]
    [Range(0f, 1f)] [SerializeField] private float sellVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float shopSuccessVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float shopFailureVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float selectItemVolume = 1f;

    private void Reset()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void Play(AudioClip clip, float volume)
    {
        if (audioSource == null || clip == null)
            return;

        audioSource.PlayOneShot(clip, volume);
    }

    public void PlaySell()
    {
        Play(sellClip, sellVolume);
    }

    public void PlayShopSuccess()
    {
        Play(shopSuccessClip, shopSuccessVolume);
    }

    public void PlayShopFailure()
    {
        Play(shopFailureClip, shopFailureVolume);
    }

    public void PlaySelectItem()
    {
        Play(selectItemClip, selectItemVolume);
    }
}