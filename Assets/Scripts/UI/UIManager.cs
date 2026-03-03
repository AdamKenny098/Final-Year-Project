using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Player Menu")]
    public GameObject playerMenuRoot;
    public UITabBar playerMenuTabs;

    bool isOpen;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        if (playerMenuRoot != null)
            playerMenuRoot.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            TogglePlayerMenu();
    }

    public void TogglePlayerMenu()
    {
        if (playerMenuRoot == null) return;

        if (isOpen) ClosePlayerMenu();
        else OpenPlayerMenu();
    }

    public void OpenPlayerMenu()
    {
        if (playerMenuRoot == null) return;

        isOpen = true;
        playerMenuRoot.SetActive(true);

        SetCursorForMenu(true);

        if (playerMenuTabs != null)
            playerMenuTabs.OpenDefault();

        if (GameStates.Instance != null)
            GameStates.Instance.SetState(GameState.Paused);
    }

    public void ClosePlayerMenu()
    {
        if (playerMenuRoot == null) return;

        isOpen = false;
        playerMenuRoot.SetActive(false);

        SetCursorForMenu(false);

        if (GameStates.Instance != null)
            GameStates.Instance.SetState(GameState.Exploration);
    }


    public bool IsPlayerMenuOpen => isOpen;

    void SetCursorForMenu(bool menuOpen)
    {
        Cursor.visible = menuOpen;
        Cursor.lockState = menuOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }

}
