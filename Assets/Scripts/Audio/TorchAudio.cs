using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TorchAudio : MonoBehaviour
{
    [SerializeField] private AudioClip whooshClip;
    [SerializeField] private AudioClip crackleClip;
    [SerializeField] private float volume = 1f;

    private AudioSource audioSource;
    private Coroutine playRoutine;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = volume;
    }

    void OnEnable()
    {
        if (playRoutine != null)
            StopCoroutine(playRoutine);

        audioSource.Stop();
        playRoutine = StartCoroutine(PlayTorchAudio());
    }

    void OnDisable()
    {
        if (playRoutine != null)
            StopCoroutine(playRoutine);

        audioSource.Stop();
    }

    private IEnumerator PlayTorchAudio()
    {
        if (whooshClip != null)
        {
            audioSource.PlayOneShot(whooshClip, volume);
            yield return new WaitForSeconds(whooshClip.length);
        }

        if (crackleClip != null)
        {
            audioSource.clip = crackleClip;
            audioSource.loop = true;
            audioSource.Play();
        }

        playRoutine = null;
    }
}