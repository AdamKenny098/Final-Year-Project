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

        int previousFloorIndex = currentFloorIndex;

        if (floors.TryGetValue(currentFloorIndex, out var current))
            current.root.SetActive(false);

        currentFloorIndex = index;

        if (!floors.TryGetValue(index, out var floor))
        {
            floor = CreateFloor(index);
            floors.Add(index, floor);
        }

        floor.root.SetActive(true);

        if (index == 0 && previousFloorIndex != 0)
            RefreshSafeFloorMerchantStock();

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

        int previousFloorIndex = currentFloorIndex;

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
        RepositionPlayerAtPortal(previousFloorIndex, targetIndex);

        if (targetIndex == 0 && previousFloorIndex != 0)
            RefreshSafeFloorMerchantStock();
            RestorePlayerStats();

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

    public void RefreshSafeFloorMerchantStock()
    {
        if (!floors.TryGetValue(0, out var safeFloor) || safeFloor.root == null)
            return;

        MerchantStockGenerator[] merchants = safeFloor.root.GetComponentsInChildren<MerchantStockGenerator>(true);

        for (int i = 0; i < merchants.Length; i++)
        {
            if (merchants[i] != null)
                merchants[i].GenerateStock();
        }
    }

    void RepositionPlayerAtPortal(int previousFloorIndex, int targetFloorIndex)
    {
        if (!floors.TryGetValue(targetFloorIndex, out var targetFloor))
            return;

        if (targetFloor == null || targetFloor.root == null)
            return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return;

        bool movedDown = targetFloorIndex > previousFloorIndex;

        Transform portal = FindArrivalPortal(targetFloor.root, movedDown);
        if (portal == null)
            return;

        Vector3 spawnPos = GetSafeArrivalPosition(targetFloor.root, portal);
        StartCoroutine(MovePlayerSafely(player, spawnPos));
    }

    Transform FindArrivalPortal(GameObject floorRoot, bool movedDown)
    {
        if (floorRoot == null)
            return null;

        StairsInstance[] stairs = floorRoot.GetComponentsInChildren<StairsInstance>(true);

        for (int i = 0; i < stairs.Length; i++)
        {
            if (stairs[i] == null)
                continue;

            if (movedDown && stairs[i].direction == StairDirection.Up)
                return stairs[i].transform;

            if (!movedDown && stairs[i].direction == StairDirection.Down)
                return stairs[i].transform;
        }

        if (movedDown)
        {
            FloorExitUp[] upExits = floorRoot.GetComponentsInChildren<FloorExitUp>(true);
            if (upExits.Length > 0 && upExits[0] != null)
                return upExits[0].transform;
        }
        else
        {
            FloorExitDown[] downExits = floorRoot.GetComponentsInChildren<FloorExitDown>(true);
            if (downExits.Length > 0 && downExits[0] != null)
                return downExits[0].transform;
        }

        return null;
    }

    Vector3 GetSafeArrivalPosition(GameObject floorRoot, Transform portal)
    {
        Vector3 portalPos = portal.position;
        portalPos.y = 1.55f;

        Room portalRoom = FindRoomContainingPoint(floorRoot, portal.position);

        if (portalRoom == null)
            return portalPos;

        Vector3 toCenter = portalRoom.node.center - portal.position;
        toCenter.y = 0f;

        if (toCenter.sqrMagnitude < 0.001f)
            return new Vector3(portalRoom.node.center.x, 1.55f, portalRoom.node.center.z);

        toCenter.Normalize();

        Vector3 candidate = portal.position + toCenter * 1.25f;
        candidate.y = 1.55f;

        return candidate;
    }

    Room FindRoomContainingPoint(GameObject floorRoot, Vector3 point)
    {
        if (floorRoot == null)
            return null;

        Room[] rooms = floorRoot.GetComponentsInChildren<Room>(true);

        for (int i = 0; i < rooms.Length; i++)
        {
            Room room = rooms[i];
            if (room == null || room.isCorridor)
                continue;

            Vector3 center = room.node.center;
            float halfWidth = room.node.width * 0.5f;
            float halfLength = room.node.length * 0.5f;

            if (point.x >= center.x - halfWidth && point.x <= center.x + halfWidth && point.z >= center.z - halfLength && point.z <= center.z + halfLength)
            {
                return room;
            }
        }

        return null;
    }

    IEnumerator MovePlayerSafely(GameObject player, Vector3 spawnPos)
    {
        if (player == null)
            yield break;

        CharacterController controller = player.GetComponent<CharacterController>();
        Rigidbody rb = player.GetComponent<Rigidbody>();

        if (controller != null)
            controller.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        player.transform.position = spawnPos;
        player.transform.rotation = Quaternion.identity;

        yield return null;

        if (rb != null)
            rb.isKinematic = false;

        if (controller != null)
            controller.enabled = true;

        FloorExitUp.BlockTriggers(2.5f);
        FloorExitDown.BlockTriggers(2.5f);
        StairsInstance.BlockTriggers(2.5f);
    }

    void RestorePlayerStats()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return;

        Character character = player.GetComponentInParent<Character>();
        if (character == null || character.stats == null)
            return;

        character.stats.health = character.stats.maxHealth;
        character.stats.mana = character.stats.maxMana;
        character.stats.stamina = character.stats.maxStamina;
    }
}