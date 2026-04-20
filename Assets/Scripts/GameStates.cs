using UnityEngine;

public enum GameState
{
    Exploration,
    Combat,
    Talking,
    Trading,
    Paused,
    Menu,
    Death,
    Loading
}

public class GameStates : MonoBehaviour
{
    public static GameStates Instance;

    public GameState currentState = GameState.Exploration;

    [Header("Receivers (drag in Inspector)")]
    public UIManager ui;
    public PauseMenuController pause;

    void Awake()
    {
        if (!Instance) Instance = this;
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        // If you forgot to wire refs, try to find them once (optional)
        if (ui == null) ui = FindFirstObjectByType<UIManager>();
        if (pause == null) pause = FindFirstObjectByType<PauseMenuController>();

        ApplyState(currentState);
        Notify(currentState, currentState);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            HandleEscape();
    }

    void HandleEscape()
    {
        switch (currentState)
        {
            case GameState.Exploration:
                SetState(GameState.Paused);
                break;

            case GameState.Combat:
                SetState(GameState.Paused);
                break;

            case GameState.Paused:
            case GameState.Menu:
                SetState(GameState.Exploration);
                break;

            case GameState.Trading:
                ShopSystem.Instance.CloseShop();
                // make sure CloseShop() eventually calls SetState(Exploration) or do it here after it closes
                break;

            case GameState.Talking:
                DialogueSystem.Instance.HideDialogue();
                SetState(GameState.Exploration);
                break;
            
            case GameState.Death:
            case GameState.Loading:
                break;
        }
    }

    public void SetState(GameState newState)
    {
        if (currentState == newState) return;

        var prev = currentState;
        currentState = newState;

        ApplyState(newState);
        Notify(prev, newState);
    }

    void Notify(GameState prev, GameState next)
    {
        if (ui != null) ui.OnStateChanged(prev, next);
        if (pause != null) pause.OnStateChanged(prev, next);
    }

    void ApplyState(GameState state)
    {
        switch (state)
        {
            case GameState.Exploration:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Time.timeScale = 1f;
                break;

            case GameState.Combat:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Time.timeScale = 1f;
                break;

            case GameState.Talking:
            case GameState.Trading:
            case GameState.Loading:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 1f;
                break;

            case GameState.Menu:
            case GameState.Paused:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0f;
                break;

            case GameState.Death:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0f;
                break;
        }
    }
}
