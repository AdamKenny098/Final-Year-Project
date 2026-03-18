using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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
    public string loadingSceneName = "Loading Screen";
    public bool isLoadingFloor;
    public float floorLoadProgress;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
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
        OnFloorChanged();

        if (MapInitiator.Instance != null)
            MapInitiator.Instance.RestartSearch();
    }

    public void GoToNextFloor()
    {
        if (isLoadingFloor) return;
        StartCoroutine(FloorLoadFlow(currentFloorIndex + 1));
    }

    public void GoToLastFloor()
    {
        if (isLoadingFloor) return;
        StartCoroutine(FloorLoadFlow(currentFloorIndex - 1));
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
        GoToNextFloor();
    }

    public void TravelUp()
    {
        GoToLastFloor();
    }

    void OnFloorChanged()
    {
        if (QuestSystem.Instance == null)
        return;

        QuestSystem.Instance.NotifyFloorReached(currentFloorIndex);
    }


    IEnumerator FloorLoadFlow(int targetIndex)
    {
        if (targetIndex < 0 || targetIndex >= floorPlan.Count)
            yield break;

        isLoadingFloor = true;
        floorLoadProgress = 0f;

        AsyncOperation overlayLoad = null;

        if (!string.IsNullOrEmpty(loadingSceneName))
        {
            overlayLoad = SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Additive);

            while (!overlayLoad.isDone)
            {
                floorLoadProgress = Mathf.Clamp01(overlayLoad.progress / 0.9f) * 0.3f;
                yield return null;
            }
        }

        yield return null;
        floorLoadProgress = 0.4f;

        if (floors.TryGetValue(currentFloorIndex, out var current))
            current.root.SetActive(false);

        floorLoadProgress = 0.55f;
        yield return null;

        currentFloorIndex = targetIndex;

        if (!floors.TryGetValue(targetIndex, out var floor))
        {
            floor = CreateFloor(targetIndex);
            floors.Add(targetIndex, floor);
        }

        floorLoadProgress = 0.85f;
        yield return null;

        floor.root.SetActive(true);
        OnFloorChanged();

        if (MapInitiator.Instance != null)
            MapInitiator.Instance.RestartSearch();

        floorLoadProgress = 1f;
        yield return null;

        if (!string.IsNullOrEmpty(loadingSceneName))
            yield return SceneManager.UnloadSceneAsync(loadingSceneName);

        isLoadingFloor = false;
        floorLoadProgress = 0f;
    }
}