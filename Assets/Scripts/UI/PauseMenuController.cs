using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenuRoot;
    public string mainMenuScene = "Main Menu";

    void Start()
    {
        if (pauseMenuRoot != null)
            pauseMenuRoot.SetActive(false);
    }

    public void OnStateChanged(GameState prev, GameState next)
    {
        if (pauseMenuRoot != null)
            pauseMenuRoot.SetActive(next == GameState.Paused);
    }

    public void Resume()
    {
        if (GameStates.Instance != null)
            GameStates.Instance.SetState(GameState.Exploration);
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(mainMenuScene, LoadSceneMode.Single);
    }
}
