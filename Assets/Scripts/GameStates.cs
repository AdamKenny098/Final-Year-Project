using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Exploration,
    Talking,
    Trading,
    Paused,
    Cutscene
}


public class GameStates : MonoBehaviour
{
    public static GameStates Instance;

    public GameState currentState = GameState.Exploration;

    public void SetState(GameState newState)
    {
        currentState = newState;
    }

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


}
