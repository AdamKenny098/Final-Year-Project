using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject playerMenuRoot;
    public UITabBar playerMenuTabs;
    public GameObject tabsRoot;
    public GameObject combatUIRoot;
    public GameObject minimapRoot;

    void Awake()
    {
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
        if (playerMenuTabs != null)
            playerMenuTabs.CloseAll();

        if (tabsRoot != null)
            tabsRoot.SetActive(false);

        if (playerMenuRoot != null)
            playerMenuRoot.SetActive(false);
    }

    public void OnStateChanged(GameState prev, GameState next)
    {
        if (GameStates.Instance != null && GameStates.Instance.currentState != GameState.Exploration)
        {
            if (combatUIRoot != null)
                combatUIRoot.SetActive(false);
            
            if (minimapRoot != null)
                minimapRoot.SetActive(false);
        }

        else
        {
            if (combatUIRoot != null)
                combatUIRoot.SetActive(true);
            
            if (minimapRoot != null)
                minimapRoot.SetActive(true);
        }

        bool active = next == GameState.Menu;

        if (!active)
        {
            ForceClosePlayerMenu();
            return;
        }

        if (playerMenuRoot != null)
            playerMenuRoot.SetActive(true);

        if (tabsRoot != null)
            tabsRoot.SetActive(true);

        if (playerMenuTabs != null)
            playerMenuTabs.OpenDefault();

        
    }
}