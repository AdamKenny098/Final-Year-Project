using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public enum RoomType
    {
        Default,
        Armory,
        Barracks,
        Warehouse,
    }

    public Node node;
    public bool isCorridor = false;
    public RoomType roomType = RoomType.Default;
    public float roomArea;
    public float availableArea;

    public List<Bounds> occupiedAreas = new List<Bounds>();
    public List<Transform> doorways = new List<Transform>();
    public Transform pillarsRoot;
    public Transform torchesRoot;
    public Transform roomItemsRoot;
    public Transform DecorRoot;
    public bool preventEnemySpawning;
    public bool preventNPCSpawning; // optional but future-proof
    public bool preventSpawning;

    [Header("Floor Mapping")]
    public int floorIndex;
    public string areaId;
    public bool visited;

    public List<Vector3> plannedDoorwayPositions = new List<Vector3>();
    public List<GameObject> spawnedEnemies = new List<GameObject>();

    public void EnsureDecorationRoots()
    {
        if (isCorridor) return;

        if (pillarsRoot == null)
            pillarsRoot = new GameObject("Pillars").transform;

        if (torchesRoot == null)
            torchesRoot = new GameObject("Torches").transform;

        if (roomItemsRoot == null)
            roomItemsRoot = new GameObject("Room Items").transform;

        pillarsRoot.SetParent(transform, false);
        torchesRoot.SetParent(transform, false);
        roomItemsRoot.SetParent(transform, false);
    }

    public void FinalizeDecorations()
    {
        if (isCorridor) return;

        EnsureDecorationRoots();

        if (DecorRoot != null) return;

        DecorRoot = new GameObject("Room Decorations").transform;
        DecorRoot.SetParent(transform, false);

        pillarsRoot.SetParent(DecorRoot, true);
        torchesRoot.SetParent(DecorRoot, true);
        roomItemsRoot.SetParent(DecorRoot, true);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (!visited)
        {
            visited = true;

            if (QuestSystem.Instance != null)
                QuestSystem.Instance.NotifyAreaDiscovered(floorIndex, areaId);
        }

        if (RoomMapTracker.Instance != null)
            RoomMapTracker.Instance.SetCurrentRoom(this);

        if (RoomEnemyActivityManager.Instance != null)
            RoomEnemyActivityManager.Instance.SetCurrentRoom(this);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (RoomMapTracker.Instance != null)
            RoomMapTracker.Instance.ClearCurrentRoom(this);
    }
}