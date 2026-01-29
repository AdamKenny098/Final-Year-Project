using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum FloorType
{
    Standard,
    Elite,
    Boss,
    Safe
}

public class LabyrinthManager : MonoBehaviour
{
    public static LabyrinthManager Instance;

    public List<FloorDefinition> floorPlan = new();
    public GameObject dungeonGeneratorPrefab;
    public GameObject safeFloorPrefab;

    public Transform labyrinthRoot;

    private Dictionary<int, FloorInstance> floors = new();
    public int currentFloorIndex;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        StartRun(20);
    }

    public void StartRun(int floorCount)
    {
        floorPlan.Clear();

        for (int i = 0; i < floorCount; i++)
        {
            floorPlan.Add(new FloorDefinition
            {
                depthIndex = i,
                floorType = (i == 0) ? FloorType.Safe : FloorType.Standard,
                difficultyScalar = 1f + i * 0.15f
            });
        }

        LoadFloor(0);
    }

    public void LoadFloor(int index)
    {
        if (index < 0 || index >= floorPlan.Count)
            return;

        if (floors.TryGetValue(currentFloorIndex, out var current))
            current.root.SetActive(false);

        currentFloorIndex = index;

        if (!floors.TryGetValue(index, out var floor))
        {
            floor = CreateFloor(index);
            floors.Add(index, floor);
        }

        floor.root.SetActive(true);
    }

    public void GoToNextFloor()
    {
        LoadFloor(currentFloorIndex + 1);
    }

    public void GoToLastFloor()
    {
        LoadFloor(currentFloorIndex - 1);
    }

    FloorInstance CreateFloor(int index)
    {
        FloorDefinition def = floorPlan[index];

        GameObject floorRoot = new($"Floor_{index}");
        floorRoot.transform.SetParent(labyrinthRoot);
        floorRoot.transform.localPosition = Vector3.zero;
        floorRoot.transform.localRotation = Quaternion.identity;


        FloorInstance instance = new()
        {
            floorIndex = index,
            floorType = def.floorType,
            root = floorRoot,
            generated = true
        };

        if (def.floorType == FloorType.Safe)
            GenerateSafeFloor(instance);
        else
            GenerateDungeonFloor(instance);

        return instance;
    }

    void GenerateDungeonFloor(FloorInstance floor)
    {
        GameObject genObj = Instantiate(dungeonGeneratorPrefab, floor.root.transform);
        DungeonManager.Instance.BeginFloor(
            genObj.GetComponent<DungeonGenerator>(),
            floor.root.transform
        );
    }


    void GenerateSafeFloor(FloorInstance floor)
    {
        GameObject safe = Instantiate(safeFloorPrefab, floor.root.transform);
        safe.name = "SafeFloor";
    }

    public void TravelDown()
    {
        int targetIndex = currentFloorIndex + 1;

        if (targetIndex >= floorPlan.Count)
            return;

        LoadFloor(targetIndex);
    }

    public void TravelUp()
    {
        int targetIndex = currentFloorIndex - 1;

        if (targetIndex < 0)
            return;

        LoadFloor(targetIndex);
    }



}

