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
    public bool doorwaysCleared;

    public GameObject nextFloorPrefab;
    public GameObject lastFloorPrefab;
    public DungeonGenerator generator;
    public DungeonRoomBuilder builder;
    public DungeonRoomOptimizer optimizer;
    public DungeonRoomDecorator decorator;
    public DungeonEntitySpawner spawner;

    public static DungeonManager Instance;
    public Transform activeFloorRoot;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

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
        dungeonFinalized = false;
        doorwaysCleared = false;
    }

    public void StartPipeline()
    {
        // Initial step: generate dungeon layout
        dungeonGenerated = true;
        SetDungeonState(DungeonState.Built);
    }

    private void Update()
    {
        // === STEP 1: BUILD STRUCTURE ===
        if (dungeonGenerated && !dungeonBuilt)
        {
            builder.StartBuildProcess();

            dungeonBuilt = true;
            SetDungeonState(DungeonState.Optimizing);
        }

        // === STEP 2: OPTIMIZE MESHES ===
        if (dungeonBuilt && !dungeonOptimized)
        {
            optimizer.StartOptimization();
            decorator.PopulateWorkableRooms();

            dungeonOptimized = true;
            SetDungeonState(DungeonState.DecoratedGenerically);
        }

        // === STEP 3: GENERIC DECORATION (Pillars, Torches, etc.) ===
        if (dungeonOptimized && !dungeonPillarsPlaced)
        {
            decorator.generatePillars = true;
            decorator.DecorateRoomsGenerically();
            dungeonPillarsPlaced = true;
        }

        if (dungeonPillarsPlaced && !dungeonTorchPillarsPlaced)
        {
            decorator.replacePillarsWithTorchPillars = true;
            decorator.DecorateRoomsGenerically();
            dungeonTorchPillarsPlaced = true;
        }

        if (dungeonTorchPillarsPlaced && !dungeonTorchesPlaced)
        {
            decorator.generateTorches = true;
            decorator.DecorateRoomsGenerically();
            dungeonTorchesPlaced = true;
        }

        if (dungeonTorchesPlaced && !dungeonGenericDecorCleaned)
        {
            decorator.cleanGenericDecor = true;
            decorator.DecorateRoomsGenerically();
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
            decorator.DecorateRooms();
            dungeonDecoratedSpecifically = true;
        }

        // === STEP 5: FINALIZE & SPAWN ===
        if (dungeonDecoratedSpecifically && !dungeonFinalized)
        {
            decorator.FinalizeDecor();

            optimizer.CollectBounds();
            optimizer.DestroyOldRoomShellObjects();
            dungeonFinalized = true;
        }

        if (dungeonFinalized && !doorwaysCleared)
        {
            decorator.ClearDoorways();

            SpawnNextFloorExit();

            doorwaysCleared = true;
            SetDungeonState(DungeonState.Completed);

            Debug.Log("Dungeon generation pipeline completed!");
        }


    }

    private void SetDungeonState(DungeonState newState)
    {
        currentState = newState;
    }

    public UnityEngine.Vector3 GetValidPointInRoom(Room room)
    {
        BoxCollider box = room.GetComponent<BoxCollider>();

        UnityEngine.Vector3 localCenter = box.center;
        UnityEngine.Vector3 localSize = box.size;

        UnityEngine.Vector3 localPoint = localCenter + new UnityEngine.Vector3(
            Random.Range(-localSize.x * 0.4f, localSize.x * 0.4f),
            0f,
            Random.Range(-localSize.z * 0.4f, localSize.z * 0.4f)
        );

        UnityEngine.Vector3 worldPoint = room.transform.TransformPoint(localPoint);

        worldPoint.y = box.bounds.min.y + 0.05f;

        return worldPoint;
    }

    void SpawnNextFloorExit()
    {
        Room targetRoom = null;
        float maxArea = 0f;

        foreach (Room room in builder.allRooms)
        {
            if (room.isCorridor) continue;

            if (room.roomArea > maxArea)
            {
                maxArea = room.roomArea;
                targetRoom = room;
            }
        }

        if (targetRoom == null)
        {
            Debug.LogError("No valid room found for next floor exit.");
            return;
        }

        UnityEngine.Vector3 spawnPos = GetValidPointInRoom(targetRoom);
        Instantiate(nextFloorPrefab, spawnPos, UnityEngine.Quaternion.identity, activeFloorRoot);
    }

    public void SpawnLastFloorExit(Room playerRoom)
    {
        Room targetRoom = playerRoom;
        float maxArea = 0f;

        if (targetRoom == null)
        {
            Debug.LogError("No valid room found for next floor exit.");
            return;
        }

        UnityEngine.Vector3 spawnPos = GetValidPointInRoom(targetRoom);
        Instantiate(lastFloorPrefab, spawnPos, UnityEngine.Quaternion.identity, activeFloorRoot);
    }

    public void ResetPipeline()
    {
        dungeonGenerated = false;
        dungeonBuilt = false;
        dungeonOptimized = false;

        dungeonPillarsPlaced = false;
        dungeonTorchPillarsPlaced = false;
        dungeonTorchesPlaced = false;
        dungeonGenericDecorCleaned = false;

        dungeonDecoratedGenerically = false;
        dungeonDecoratedSpecifically = false;
        dungeonFinalized = false;
        doorwaysCleared = false;

        currentState = DungeonState.Generating;
    }

    public void BeginFloor(DungeonGenerator currentGenerator, Transform floorRoot)
    {
        ResetPipeline();

        generator = currentGenerator;
        activeFloorRoot = floorRoot;

        builder = generator.GetComponent<DungeonRoomBuilder>();
        optimizer = generator.GetComponent<DungeonRoomOptimizer>();
        decorator = generator.GetComponent<DungeonRoomDecorator>();
        spawner = generator.GetComponent<DungeonEntitySpawner>();

        builder.ParentObjects(floorRoot);
        optimizer.FillReferences(builder, decorator, floorRoot);
        decorator.FillReferences(builder, floorRoot);
        spawner.FillReferences(builder, decorator, floorRoot);

        generator.CreateDungeon();

        if (QuestSystem.Instance != null && builder != null && LabyrinthManager.Instance != null)
        {
            int floorIndex = LabyrinthManager.Instance ? LabyrinthManager.Instance.currentFloorIndex : 0;
            QuestSystem.Instance.RegisterFloorAreaCount(floorIndex, builder.allRooms.Count);
        }

        dungeonGenerated = true;
    }

}
