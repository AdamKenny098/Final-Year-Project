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
        DecoratedGenerically,
        DecoratedSpecifically,
        Completed
    }

    public DungeonState currentState = DungeonState.Generating;

    // Flags for pipeline control
    public bool dungeonGenerated;
    public bool dungeonBuilt;
    public bool dungeonOptimized;
    public bool dungeonPillarsPlaced;
    public bool dungeonTorchPillarsPlaced;
    public bool dungeonTorchesPlaced;
    public bool dungeonGenericDecorCleaned;
    public bool dungeonDecoratedGenerically;
    public bool dungeonDecoratedSpecifically;
    public bool dungeonFinalized;


    private DungeonGenerator dungeonGen;
    private DungeonRoomBuilder roomBuilder;
    private DungeonRoomOptimizer roomOptimizer;
    private DungeonRoomDecorator roomDecorator;
    private DungeonEntitySpawner entitySpawner;


    private void Awake()
    {
        // Reset flags
        dungeonGenerated = false;
        dungeonBuilt = false;
        dungeonOptimized = false;
        dungeonDecoratedGenerically = false;
        dungeonDecoratedSpecifically = false;

        dungeonPillarsPlaced = false;
        dungeonTorchPillarsPlaced = false;
        dungeonTorchesPlaced = false;
        dungeonGenericDecorCleaned = false;
        

        // Cache component references
        dungeonGen = GetComponent<DungeonGenerator>();
        roomBuilder = GetComponent<DungeonRoomBuilder>();
        roomOptimizer = GetComponent<DungeonRoomOptimizer>();
        roomDecorator = GetComponent<DungeonRoomDecorator>();
        entitySpawner = GetComponent<DungeonEntitySpawner>();

    }

    private void Start()
    {
        // Initial step: generate dungeon layout
        dungeonGen.CreateDungeon();
        dungeonGenerated = true;
        SetDungeonState(DungeonState.Built);
    }

    private void Update()
    {
        // === STEP 1: BUILD STRUCTURE ===
        if (dungeonGenerated && !dungeonBuilt)
        {
            roomBuilder.StartBuildProcess();
            dungeonBuilt = true;
            SetDungeonState(DungeonState.Optimizing);
        }

        // === STEP 2: OPTIMIZE MESHES ===
        if (dungeonBuilt && !dungeonOptimized)
        {
            roomOptimizer.StartOptimization();
            roomDecorator.PopulateWorkableRooms();
            dungeonOptimized = true;
            SetDungeonState(DungeonState.DecoratedGenerically);
        }

        // === STEP 3: GENERIC DECORATION (Pillars, Torches, etc.) ===
        if (dungeonOptimized && !dungeonPillarsPlaced)
        {
            roomDecorator.generatePillars = true;
            roomDecorator.DecorateRoomsGenerically();
            dungeonPillarsPlaced = true;
        }

        if (dungeonPillarsPlaced && !dungeonTorchPillarsPlaced)
        {
            roomDecorator.replacePillarsWithTorchPillars = true;
            roomDecorator.DecorateRoomsGenerically();
            dungeonTorchPillarsPlaced = true;
        }

        if (dungeonTorchPillarsPlaced && !dungeonTorchesPlaced)
        {
            roomDecorator.generateTorches = true;
            roomDecorator.DecorateRoomsGenerically();
            dungeonTorchesPlaced = true;
        }

        if (dungeonTorchesPlaced && !dungeonGenericDecorCleaned)
        {
            roomDecorator.cleanGenericDecor = true;
            roomDecorator.DecorateRoomsGenerically();
            dungeonGenericDecorCleaned = true;
        }

        // Mark the generic decoration phase as done
        if (!dungeonDecoratedGenerically && dungeonPillarsPlaced && dungeonTorchPillarsPlaced && dungeonTorchesPlaced && dungeonGenericDecorCleaned)
        {
            dungeonDecoratedGenerically = true;
            SetDungeonState(DungeonState.DecoratedSpecifically);
        }

        // === STEP 4: SPECIFIC DECORATION (Room items by type) ===
        if (dungeonDecoratedGenerically && !dungeonDecoratedSpecifically)
        {
            roomDecorator.DecorateRooms();
            dungeonDecoratedSpecifically = true;
        }

        // === STEP 5: FINALIZE & SPAWN ===
        if (dungeonDecoratedSpecifically && !dungeonFinalized)
        {
            roomDecorator.FinalizeDecor();

            roomOptimizer.CollectBounds();

            entitySpawner.SpawnAll();

            dungeonFinalized = true;
            SetDungeonState(DungeonState.Completed);

            Debug.Log("Dungeon generation pipeline completed!");
        }


    }

    private void SetDungeonState(DungeonState newState)
    {
        currentState = newState;
    }

}
