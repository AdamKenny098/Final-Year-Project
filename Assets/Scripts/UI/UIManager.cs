using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject playerMenuRoot;
    public UITabBar playerMenuTabs;

    void Awake()
    {
        if (playerMenuRoot != null)
            playerMenuRoot.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            TogglePlayerMenu();
    }

    void TogglePlayerMenu()
    {
        if (GameStates.Instance == null) return;

        var currentState  = GameStates.Instance.currentState;

        if (currentState == GameState.Paused || currentState == GameState.Talking || currentState == GameState.Trading)
            return;

        GameStates.Instance.SetState(currentState == GameState.Menu ? GameState.Exploration : GameState.Menu);
    }

    public void OnStateChanged(GameState prev, GameState next)
    {
        bool active = next == GameState.Menu;

        if (playerMenuRoot != null)
            playerMenuRoot.SetActive(active);

        if (active  && playerMenuTabs != null)
            playerMenuTabs.OpenDefault();
    }
}
