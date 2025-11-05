using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public enum DungeonState
    {
        Generating,
        Built,
        Optimizing,
        Decorated
    }

    public DungeonState currentState = DungeonState.Generating;

    public bool dungeonGenerated = false;
    public bool dungeonOptimized = false;
    public bool dungeonDecorated = false;
    public bool dungeonBuilt = false;

    public void SetDungeonState(DungeonState newState)
    {
        currentState = newState;
    }

    public void Awake()
    {
        dungeonGenerated = false;
        dungeonBuilt = false;
        dungeonOptimized = false;
        dungeonDecorated = false;
    }

    public void Start()
    {
        DungeonGenerator dungeonGen = GetComponent<DungeonGenerator>();
        DungeonRoomBuilder roomBuilder = GetComponent<DungeonRoomBuilder>();
        DungeonRoomOptimizer roomOptimizer = GetComponent<DungeonRoomOptimizer>();
        DunegonRoomDecorator roomDecorator = GetComponent<DunegonRoomDecorator>();

        dungeonGen.CreateDungeon();
        dungeonGenerated = true;
        SetDungeonState(DungeonState.Built);
    }

    public void Update()
    {
        DungeonRoomBuilder roomBuilder = GetComponent<DungeonRoomBuilder>();
        DungeonRoomOptimizer roomOptimizer = GetComponent<DungeonRoomOptimizer>();
        DunegonRoomDecorator roomDecorator = GetComponent<DunegonRoomDecorator>();

        if (dungeonGenerated && !dungeonBuilt)
        {
            roomBuilder.StartBuildProcess();
            dungeonBuilt = true;
            SetDungeonState(DungeonState.Optimizing);
        }

        if (dungeonBuilt && !dungeonOptimized)
        {
            roomOptimizer.StartOptimization();
            dungeonOptimized = true;
            SetDungeonState(DungeonState.Decorated);
        }

        if (dungeonOptimized && !dungeonDecorated)
        {
            roomDecorator.StartDecorationProcess();
            dungeonDecorated = true;
        }
    }
}
