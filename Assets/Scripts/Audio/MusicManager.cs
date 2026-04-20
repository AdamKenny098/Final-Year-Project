using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Clips")]
    [SerializeField] private AudioClip safeClip;
    [SerializeField] private AudioClip explorationClip;
    [SerializeField] private AudioClip combatClip;
    [SerializeField] private AudioClip deathClip;

    [Header("Settings")]
    [SerializeField] private float volume = 1f;
    [SerializeField] private bool playOnStart = true;

    private AudioSource musicSource;
    private AudioClip currentClip;
    private bool wasPausedByManager;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        musicSource = GetComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f;
        musicSource.volume = volume;
    }

    void Start()
    {
        if (playOnStart)
            RefreshMusic();
    }

    void Update()
    {
        if (ShouldSilenceMusic())
        {
            PauseMusic();
            return;
        }

        RefreshMusic();
    }

    bool ShouldSilenceMusic()
    {
        if (LabyrinthManager.Instance != null && LabyrinthManager.Instance.isLoadingFloor)
            return true;

        if (GameStates.Instance == null)
            return false;

        GameState state = GameStates.Instance.currentState;

        return state == GameState.Paused || state == GameState.Menu;
    }

    void PauseMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Pause();
            wasPausedByManager = true;
        }
    }

    public void RefreshMusic()
    {
        AudioClip targetClip = ResolveClip();

        if (targetClip == null)
            return;

        bool clipChanged = currentClip != targetClip;

        if (clipChanged)
        {
            currentClip = targetClip;
            musicSource.clip = currentClip;
        }

        musicSource.volume = volume;

        if (wasPausedByManager)
        {
            if (clipChanged)
            {
                musicSource.Play();
            }
            else
            {
                musicSource.UnPause();
            }

            wasPausedByManager = false;
            return;
        }

        if (!musicSource.isPlaying)
        {
            if (musicSource.clip != currentClip)
                musicSource.clip = currentClip;

            musicSource.Play();
        }
    }

    private AudioClip ResolveClip()
    {
        GameState state = GameStates.Instance != null
            ? GameStates.Instance.currentState
            : GameState.Exploration;

        int floorIndex = LabyrinthManager.Instance != null
            ? LabyrinthManager.Instance.currentFloorIndex
            : 0;

        switch (state)
        {
            case GameState.Combat:
                return combatClip;

            case GameState.Death:
                return deathClip;

            default:
                return floorIndex == 0 ? safeClip : explorationClip;
        }
    }
}