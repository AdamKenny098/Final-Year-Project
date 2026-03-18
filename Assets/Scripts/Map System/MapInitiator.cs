using System.Collections;
using UnityEngine;

public class MapInitiator : MonoBehaviour
{
    public static MapInitiator Instance { get; private set; }

    public DungeonMapUI minimapUI;
    public DungeonMapUI fullMapUI;

    [Header("Search")]
    public float searchInterval = 0.25f;
    public float maxSearchTime = 10f;

    public bool initialized;
    public Coroutine bootstrapRoutine;

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
        StartBootstrap();
    }

    public void StartBootstrap()
    {
        if (bootstrapRoutine != null)
            StopCoroutine(bootstrapRoutine);

        bootstrapRoutine = StartCoroutine(BootstrapRoutine());
    }

    public IEnumerator BootstrapRoutine()
    {
        float elapsed = 0f;
        initialized = false;

        while (!initialized && elapsed < maxSearchTime)
        {
            DungeonRoomBuilder builder = FindFirstObjectByType<DungeonRoomBuilder>();

            if (builder != null && builder.allRooms != null && builder.allRooms.Count > 0)
            {
                ApplyBuilder(builder);
                initialized = true;
                bootstrapRoutine = null;
                yield break;
            }

            elapsed += searchInterval;
            yield return new WaitForSeconds(searchInterval);
        }

        bootstrapRoutine = null;

        if (!initialized)
            Debug.LogWarning("MapInitiator: Could not find a ready DungeonRoomBuilder in time.");
    }

    public void ApplyBuilder(DungeonRoomBuilder builder)
    {
        if (minimapUI != null)
        {
            minimapUI.SetRoomBuilder(builder);
            minimapUI.BuildMap();
        }

        if (fullMapUI != null)
        {
            fullMapUI.SetRoomBuilder(builder);
            fullMapUI.BuildMap();
        }
    }

    public void RebuildMaps()
    {
        DungeonRoomBuilder builder = FindFirstObjectByType<DungeonRoomBuilder>();
        if (builder == null) return;
        if (builder.allRooms == null || builder.allRooms.Count == 0) return;

        ApplyBuilder(builder);
        initialized = true;
    }

    public void RestartSearch()
    {
        StartBootstrap();
    }
}