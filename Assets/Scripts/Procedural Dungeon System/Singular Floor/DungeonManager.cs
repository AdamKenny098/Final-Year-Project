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
        RoomTypesAssigned,
        PlansBuilt,
        SpecificPlanned,
        GenericPlanned,
        Instantiated,
        Validated,
        Completed
    }

    public DungeonState currentState = DungeonState.Generating;

    public bool dungeonGenerated;
    public bool dungeonBuilt;
    public bool dungeonOptimized;
    public bool roomTypesAssigned;
    public bool plansBuilt;
    public bool specificPlanned;
    public bool genericPlanned;
    public bool decorInstantiated;
    public bool dungeonValidated;
    public bool dungeonFinalized;
    public bool exitsSpawned;

    public GameObject nextFloorPrefab;
    public GameObject lastFloorPrefab;

    public DungeonGenerator generator;
    public DungeonRoomBuilder builder;
    public DungeonRoomOptimizer optimizer;
    public DungeonRoomDecorator decorator;
    public DungeonEntitySpawner spawner;

    public static DungeonManager Instance;
    public Transform activeFloorRoot;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        ResetPipeline();
    }

    void Update()
    {
        if (dungeonGenerated && !dungeonBuilt)
        {
            builder.StartBuildProcess();
            dungeonBuilt = true;
            SetDungeonState(DungeonState.Built);
            return;
        }

        if (dungeonBuilt && !dungeonOptimized)
        {
            optimizer.StartOptimization();
            decorator.PopulateWorkableRooms();
            dungeonOptimized = true;
            SetDungeonState(DungeonState.Optimizing);
            return;
        }

        if (dungeonOptimized && !roomTypesAssigned)
        {
            decorator.AssignRoomTypesIfNeeded();
            roomTypesAssigned = true;
            SetDungeonState(DungeonState.RoomTypesAssigned);
            return;
        }

        if (roomTypesAssigned && !plansBuilt)
        {
            decorator.BuildAllRoomPlans();
            plansBuilt = true;
            SetDungeonState(DungeonState.PlansBuilt);
            return;
        }

        if (plansBuilt && !specificPlanned)
        {
            decorator.PlanSpecificDecorFromPlans();
            specificPlanned = true;
            SetDungeonState(DungeonState.SpecificPlanned);
            return;
        }

        if (specificPlanned && !genericPlanned)
        {
            decorator.PlanGenericDecorFromPlans();
            genericPlanned = true;
            SetDungeonState(DungeonState.GenericPlanned);
            return;
        }

        if (genericPlanned && !decorInstantiated)
        {
            decorator.InstantiateAllDecorFromPlans();
            decorInstantiated = true;
            SetDungeonState(DungeonState.Instantiated);
            return;
        }

        if (decorInstantiated && !dungeonValidated)
        {
            decorator.ValidateAllDecor();
            dungeonValidated = true;
            SetDungeonState(DungeonState.Validated);
            return;
        }

        if (dungeonValidated && !dungeonFinalized)
        {
            decorator.FinalizeDecor();
            optimizer.CollectBounds();
            optimizer.DestroyOldRoomShellObjects();
            dungeonFinalized = true;
            return;
        }

        if (dungeonFinalized && !exitsSpawned)
        {
            SpawnNextFloorExit();
            ClearDecorAroundExits(1f);
            exitsSpawned = true;
            SetDungeonState(DungeonState.Completed);
            Debug.Log("Dungeon generation pipeline completed.");
        }
    }

    void SetDungeonState(DungeonState newState)
    {
        currentState = newState;
    }

    public Vector3 GetValidPointInRoom(Room room)
    {
        BoxCollider box = room.GetComponent<BoxCollider>();

        Vector3 localCenter = box.center;
        Vector3 localSize = box.size;

        Vector3 localPoint = localCenter + new Vector3(
            Random.Range(-localSize.x * 0.4f, localSize.x * 0.4f),
            0f,
            Random.Range(-localSize.z * 0.4f, localSize.z * 0.4f)
        );

        Vector3 worldPoint = room.transform.TransformPoint(localPoint);
        worldPoint.y = box.bounds.min.y + 0.05f;

        return worldPoint;
    }

    public void SpawnNextFloorExit()
    {
        Room targetRoom = null;
        float maxArea = 0f;

        foreach (Room room in builder.allRooms)
        {
            if (room == null || room.isCorridor)
                continue;

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

        if (decorator == null)
        {
            Debug.LogError("DungeonRoomDecorator missing.");
            return;
        }

        if (!decorator.TryGetExitPosition(targetRoom, out Vector3 spawnPos))
        {
            Debug.LogError("Failed to find valid next floor exit position.");
            return;
        }

        Instantiate(nextFloorPrefab, spawnPos, Quaternion.identity, activeFloorRoot);
    }

    public void SpawnLastFloorExit(Room playerRoom)
    {
        if (playerRoom == null)
        {
            Debug.LogError("No valid room found for last floor exit.");
            return;
        }

        if (decorator == null)
        {
            Debug.LogError("DungeonRoomDecorator missing.");
            return;
        }

        if (!decorator.TryGetExitPosition(playerRoom, out Vector3 spawnPos))
        {
            Debug.LogError("Failed to find valid last floor exit position.");
            return;
        }

        Instantiate(lastFloorPrefab, spawnPos, Quaternion.identity, activeFloorRoot);
    }

    public void ResetPipeline()
    {
        dungeonGenerated = false;
        dungeonBuilt = false;
        dungeonOptimized = false;
        roomTypesAssigned = false;
        plansBuilt = false;
        specificPlanned = false;
        genericPlanned = false;
        decorInstantiated = false;
        dungeonValidated = false;
        dungeonFinalized = false;
        exitsSpawned = false;

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
        dungeonGenerated = true;
    }

    void ClearDecorAroundExits(float radius = 1f)
    {
        if (activeFloorRoot == null)
            return;

        List<Transform> exits = new List<Transform>();

        for (int i = 0; i < activeFloorRoot.childCount; i++)
        {
            Transform child = activeFloorRoot.GetChild(i);
            if (child == null)
                continue;

            if (child.GetComponent<FloorExitDown>() != null ||
                child.GetComponent<FloorExitUp>() != null ||
                child.GetComponent<StairsInstance>() != null)
            {
                exits.Add(child);
            }
        }

        if (exits.Count == 0)
            return;

        GameObject[] decorObjects = GameObject.FindGameObjectsWithTag("Decor");

        for (int i = 0; i < decorObjects.Length; i++)
        {
            GameObject decor = decorObjects[i];
            if (decor == null)
                continue;

            for (int j = 0; j < exits.Count; j++)
            {
                Transform exitTransform = exits[j];
                if (exitTransform == null)
                    continue;

                Vector3 flatDecorPos = decor.transform.position;
                Vector3 flatExitPos = exitTransform.position;

                flatDecorPos.y = 0f;
                flatExitPos.y = 0f;

                if (Vector3.Distance(flatDecorPos, flatExitPos) <= radius)
                {
                    Destroy(decor);
                    break;
                }
            }
        }
    }
}