using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject playerMenuRoot;
    public UITabBar playerMenuTabs;
    public GameObject tabsRoot;
    public GameObject combatUIRoot;
    public GameObject minimapRoot;
    public GameObject explorationUIRoot;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip menuToggleSfx;
    [Range(0f, 1f)] [SerializeField] private float menuToggleVolume = 1f;

    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (playerMenuRoot != null)
            playerMenuRoot.SetActive(false);

        if (tabsRoot != null)
            tabsRoot.SetActive(false);

        if (playerMenuTabs != null)
            playerMenuTabs.CloseAll();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            TogglePlayerMenu();
    }

    void TogglePlayerMenu()
    {
        if (GameStates.Instance == null) return;

        var currentState = GameStates.Instance.currentState;

        if (currentState == GameState.Paused || currentState == GameState.Talking || currentState == GameState.Trading)
            return;

        GameStates.Instance.SetState(currentState == GameState.Menu ? GameState.Exploration : GameState.Menu);
    }

    void ForceClosePlayerMenu()
    {
        bool wasOpen = playerMenuRoot != null && playerMenuRoot.activeSelf;

        if (playerMenuTabs != null)
            playerMenuTabs.CloseAll();

        if (tabsRoot != null)
            tabsRoot.SetActive(false);

        if (playerMenuRoot != null)
            playerMenuRoot.SetActive(false);

        if (wasOpen)
            PlaymenuToggle();
    }

    public void OnStateChanged(GameState prev, GameState next)
    {
        if (GameStates.Instance != null && GameStates.Instance.currentState != GameState.Exploration)
        {
            if (explorationUIRoot != null)
                explorationUIRoot.SetActive(false);
        }
        else
        {
            if (explorationUIRoot != null)
                explorationUIRoot.SetActive(true);
        }

        bool active = next == GameState.Menu;

        if (!active)
        {
            ForceClosePlayerMenu();
            return;
        }

        bool wasClosed = playerMenuRoot != null && !playerMenuRoot.activeSelf;

        if (playerMenuRoot != null)
            playerMenuRoot.SetActive(true);

        if (tabsRoot != null)
            tabsRoot.SetActive(true);

        if (playerMenuTabs != null)
            playerMenuTabs.OpenDefault();

        if (wasClosed)
            PlaymenuToggle();
    }

    void PlaymenuToggle()
    {
        if (audioSource == null || menuToggleSfx == null)
            return;

        audioSource.PlayOneShot(menuToggleSfx, menuToggleVolume);
    }
}