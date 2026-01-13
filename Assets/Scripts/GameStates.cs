using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Exploration,
    Talking,
    Trading,
    Paused,
    Menu,
}


public class GameStates : MonoBehaviour
{
    public static GameStates Instance;

    public GameState currentState = GameState.Exploration;
    public void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetState(GameState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;
        ApplyState(newState);
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

            case GameState.Talking:
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
        }
    }


}
