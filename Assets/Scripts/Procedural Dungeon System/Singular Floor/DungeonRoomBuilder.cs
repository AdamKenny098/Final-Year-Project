using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class DungeonRoomBuilder : MonoBehaviour
{
    public GameObject blockPrefab;
    public DungeonRoomDecorator decorator;
    public List<Room> allRooms = new List<Room>();
    public List<GameObject> floors = new List<GameObject>();
    public GameObject corridors;
    public GameObject rooms;
    public GameObject dungeonParent;
    Transform floorRoot;

    [Header("Floor Mapping")]
    int roomCounter;
    int corridorCounter;

    public void StartBuildProcess()
    {
        DeleteExcessBlocks();
    }

public void BuildRoom(Node node, float rotationDegrees = 0f, bool isCorridor = false, Room.RoomType type = Room.RoomType.Default)
{
    if (node == null) return;

    string wallTag = isCorridor ? "CorridorBlock" : "wallBlock";
    string floorTag = isCorridor ? "CorridorBlock" : "floorBlock";
    string ceilingTag = isCorridor ? "CorridorBlock" : "ceilingBlock";

    GameObject roomAsGameObject = new GameObject(isCorridor ? "Corridor" : "Room");
    roomAsGameObject.transform.position = node.center;
    roomAsGameObject.transform.rotation = Quaternion.Euler(0f, rotationDegrees, 0f);

    if (isCorridor)
        roomAsGameObject.transform.SetParent(corridors.transform);
    else
        roomAsGameObject.transform.SetParent(rooms.transform);

    Room roomComponent = roomAsGameObject.AddComponent<Room>();
    roomComponent.node = node;
    roomComponent.isCorridor = isCorridor;
    roomComponent.roomType = type;
    roomComponent.floorIndex = LabyrinthManager.Instance != null ? LabyrinthManager.Instance.currentFloorIndex : 0;

    if (isCorridor)
        roomComponent.areaId = "C" + corridorCounter++;
    else
        roomComponent.areaId = "R" + roomCounter++;

    roomComponent.visited = false;
    allRooms.Add(roomComponent);

    GameObject wallsParent1 = new GameObject("Walls");
    GameObject wallsParent2 = new GameObject("Walls");
    GameObject wallsParent3 = new GameObject("Walls");
    GameObject wallsParent4 = new GameObject("Walls");

    GameObject floorParent = new GameObject("Floor");
    GameObject ceilingParent = new GameObject("Ceiling");

    wallsParent1.tag = "dungeonWall";
    wallsParent2.tag = "dungeonWall";
    wallsParent3.tag = "dungeonWall";
    wallsParent4.tag = "dungeonWall";

    floorParent.tag = "dungeonFloor";
    ceilingParent.tag = "dungeonCeiling";

    wallsParent1.transform.SetParent(roomAsGameObject.transform);
    wallsParent2.transform.SetParent(roomAsGameObject.transform);
    wallsParent3.transform.SetParent(roomAsGameObject.transform);
    wallsParent4.transform.SetParent(roomAsGameObject.transform);

    floorParent.transform.SetParent(roomAsGameObject.transform);
    ceilingParent.transform.SetParent(roomAsGameObject.transform);

    int roomWidth = Mathf.FloorToInt(node.width);
    int roomLength = Mathf.FloorToInt(node.length);

    int floorLevel = -1;
    int ceilingLevel = 5;

    Quaternion rotation = Quaternion.Euler(0f, rotationDegrees, 0f);

    // In this builder, length is always local Z before rotation.
    // So:
    // - first wall loop = end caps
    // - second wall loop = long side walls
    //
    // For corridors:
    // - skip end caps
    // - keep side walls
    bool spawnEndCaps = !isCorridor;
    bool spawnSideWalls = true;

    // End caps
    if (spawnEndCaps)
    {
        for (int i = 0; i < roomWidth; i++)
        {
            for (int j = 2; j < 5; j++)
            {
                Vector3 offsetA = new Vector3((-roomWidth / 2f) + i + 0.5f, j, roomLength / 2f - 0.5f);
                Vector3 offsetB = new Vector3((-roomWidth / 2f) + i + 0.5f, j, -roomLength / 2f + 0.5f);

                SpawnBlock(wallsParent1, node, rotation, offsetA, wallTag);
                SpawnBlock(wallsParent2, node, rotation, offsetB, wallTag);
            }
        }
    }

    // Long side walls
    if (spawnSideWalls)
    {
        for (int k = 0; k < roomLength; k++)
        {
            for (int j = 2; j < 5; j++)
            {
                Vector3 offsetC = new Vector3(roomWidth / 2f - 0.5f, j, (-roomLength / 2f) + k + 0.5f);
                Vector3 offsetD = new Vector3(-roomWidth / 2f + 0.5f, j, (-roomLength / 2f) + k + 0.5f);

                SpawnBlock(wallsParent3, node, rotation, offsetC, wallTag);
                SpawnBlock(wallsParent4, node, rotation, offsetD, wallTag);
            }
        }
    }

    // Floor
    for (int i = 0; i < roomWidth; i++)
    {
        for (int k = 0; k < roomLength; k++)
        {
            Vector3 offset = new Vector3((-roomWidth / 2f) + i + 0.5f, -floorLevel, (-roomLength / 2f) + k + 0.5f);
            SpawnBlock(floorParent, node, rotation, offset, floorTag);
        }
    }

    floors.Add(floorParent);

    // Ceiling
    for (int i = 0; i < roomWidth; i++)
    {
        for (int k = 0; k < roomLength; k++)
        {
            Vector3 offset = new Vector3((-roomWidth / 2f) + i + 0.5f, ceilingLevel, (-roomLength / 2f) + k + 0.5f);
            SpawnBlock(ceilingParent, node, rotation, offset, ceilingTag);
        }
    }

    BoxCollider boxC = roomAsGameObject.AddComponent<BoxCollider>();
    boxC.center = new Vector3(0f, (ceilingLevel - floorLevel) / 2f, 0f);
    roomComponent.roomArea = (roomWidth - 2) * (roomLength - 2);
    boxC.size = new Vector3(roomWidth - 2, ceilingLevel - 1.5f, roomLength - 2);
    boxC.isTrigger = true;
}
    public void SpawnBlock(GameObject parentofBlock, Node node, Quaternion rotation, Vector3 offset, string tag)
    {
        Vector3 pos = node.center + rotation * offset;
        GameObject block = Instantiate(blockPrefab, pos, rotation, parentofBlock.transform);
        block.tag = tag;
        block.isStatic = true;
    }

    public void DeleteExcessBlocks()
{
    Room[] everyRoom = FindObjectsOfType<Room>();

    foreach (Room room in everyRoom)
    {
        BoxCollider boxC = room.GetComponent<BoxCollider>();
        if (boxC == null) continue;

        Vector3 worldCenter = boxC.transform.TransformPoint(boxC.center);
        Vector3 halfExtents = boxC.size * 0.5f * 0.97f;

        Collider[] overlaps = Physics.OverlapBox(worldCenter, halfExtents, boxC.transform.rotation);

        foreach (Collider col in overlaps)
        {
            if (col == null) continue;

            if (!room.isCorridor &&
                col.CompareTag("floorBlock") &&
                col.transform.parent != null &&
                col.transform.parent.parent != null &&
                col.transform.parent.parent.name == "Corridor")
            {
                Destroy(col.gameObject);
                continue;
            }

            if (!room.isCorridor &&
                col.CompareTag("ceilingBlock") &&
                col.transform.parent != null &&
                col.transform.parent.parent != null &&
                col.transform.parent.parent.name == "Corridor")
            {
                Destroy(col.gameObject);
                continue;
            }
        }
    }
}

    public void AddNavMeshSurface()
    {
        foreach (GameObject floor in floors)
        {
            NavMeshSurface navMeshSurface = floor.AddComponent<NavMeshSurface>();
        }
    }

    public void ParentObjects(Transform floorRoot)
    {
        roomCounter = 0;
        corridorCounter = 0;

        this.floorRoot = floorRoot;

        corridors = new GameObject("Corridors");
        rooms = new GameObject("Rooms");

        corridors.transform.SetParent(floorRoot, false);
        rooms.transform.SetParent(floorRoot, false);
    }

    public void CarveDoorways()
    {
        foreach (Room room in allRooms)
        {
            if (room.isCorridor)
                continue;

            foreach (Vector3 doorway in room.plannedDoorwayPositions)
            {
                CarveDoorway(room, doorway);
            }
        }
    }

    void CarveDoorway(Room room, Vector3 doorway)
    {
        int doorHeight = 3;
        int doorWidth = 3;

        float snappedDoorX = Mathf.Round(doorway.x);
        float snappedDoorZ = Mathf.Round(doorway.z);

        float minX = room.transform.position.x - Mathf.FloorToInt(room.node.width) / 2f;
        float maxX = room.transform.position.x + Mathf.FloorToInt(room.node.width) / 2f;
        float minZ = room.transform.position.z - Mathf.FloorToInt(room.node.length) / 2f;
        float maxZ = room.transform.position.z + Mathf.FloorToInt(room.node.length) / 2f;

        // Distance checks with a small tolerance to account for floating point imprecision
        bool onWestWall = Mathf.Abs(snappedDoorX - (minX + 0.5f)) < 0.1f;
        bool onEastWall = Mathf.Abs(snappedDoorX - (maxX - 0.5f)) < 0.1f;
        bool onSouthWall = Mathf.Abs(snappedDoorZ - (minZ + 0.5f)) < 0.1f;
        bool onNorthWall = Mathf.Abs(snappedDoorZ - (maxZ - 0.5f)) < 0.1f;

        foreach (Transform group in room.transform)
        {
            if (group.name != "Walls")
                continue;

            foreach (Transform block in group)
            {
                Vector3 blockPosition = block.position;

                bool correctHeight = blockPosition.y >= 2 && blockPosition.y < 2 + doorHeight;
                if (!correctHeight)
                    continue;

                bool shouldDestroy = false;

                // doorway on left/right wall = widen across Z
                if (onWestWall || onEastWall)
                {
                    shouldDestroy = Mathf.Abs(blockPosition.x - snappedDoorX) < 0.01f && Mathf.Abs(blockPosition.z - snappedDoorZ) <= 1.01f;
                }

                // doorway on top/bottom wall = widen across X
                else if (onNorthWall || onSouthWall)
                {
                    shouldDestroy = Mathf.Abs(blockPosition.z - snappedDoorZ) < 0.01f && Mathf.Abs(blockPosition.x - snappedDoorX) <= 1.01f;
                }

                if (shouldDestroy)
                {
                    Destroy(block.gameObject);
                }
            }
        }

        GameObject doorwayObj = new GameObject("Doorway");
        doorwayObj.transform.position = new Vector3(snappedDoorX, 2f, snappedDoorZ);
        doorwayObj.transform.SetParent(room.transform);
        room.doorways.Add(doorwayObj.transform);
    }   
}