using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [SerializeField] GameObject gameOverUI;
    [SerializeField] string mainMenuSceneName = "Main Menu";

    bool gameOverTriggered;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (gameOverUI != null)
            gameOverUI.SetActive(false);
    }

    public void TriggerGameOver()
    {
        if (gameOverTriggered)
            return;

        gameOverTriggered = true;

        Time.timeScale = 0f;

        if (gameOverUI != null)
            gameOverUI.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        gameOverTriggered = false;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public bool IsGameOver()
    {
        return gameOverTriggered;
    }
}