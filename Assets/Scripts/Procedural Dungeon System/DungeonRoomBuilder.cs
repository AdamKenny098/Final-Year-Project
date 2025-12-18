using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class DungeonRoomBuilder : MonoBehaviour
{
    public GameObject blockPrefab;
    public DunegonRoomDecorator decorator;

    public List<Room> allRooms = new List<Room>();
    public List<GameObject> floors = new List<GameObject>();
    public static DungeonRoomBuilder Instance;

    public GameObject corridors;
    public GameObject rooms;
    public GameObject dungeonParent;

    public void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        dungeonParent = GameObject.Find("DungeonGenerator");
        corridors = new GameObject("Corridors");
        rooms = new GameObject("Rooms");

        corridors.transform.SetParent(dungeonParent.transform);
        rooms.transform.SetParent(dungeonParent.transform);
    }

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
        roomAsGameObject.transform.rotation = Quaternion.Euler(0, rotationDegrees, 0);

        if(roomAsGameObject.name == "Corridor")
        {
            roomAsGameObject.transform.SetParent(corridors.transform);
        }
        else
        {
            roomAsGameObject.transform.SetParent(rooms.transform);
        }

        Room roomComponent = roomAsGameObject.AddComponent<Room>();
        roomComponent.node = node;
        roomComponent.isCorridor = isCorridor;
        roomComponent.roomType = type;
        allRooms.Add(roomComponent);

        // room sub components
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

        float width = node.width;
        float length = node.length;

        int roomWidth = Mathf.FloorToInt(width);
        int roomLength = Mathf.FloorToInt(length);

        int floorLevel = -1;
        int ceilingLevel = 5;

        Quaternion rotation = Quaternion.Euler(0, rotationDegrees, 0);

        // === Walls ===
        for (int i = 0; i < roomWidth; i++)
        {
            for (int j = 2; j < 5; j++)
            {
                Vector3 offsetA = new Vector3((-roomWidth / 2f) + i + 0.5f, j, roomLength / 2f - 0.5f);
                Vector3 offsetB = new Vector3((-roomWidth / 2f) + i + 0.5f, j, -roomLength / 2f + 0.5f);

                SpawnBlock(wallsParent1, node, rotation, offsetA, "wallBlock");
                SpawnBlock(wallsParent2, node, rotation, offsetB, "wallBlock");
            }
        }

        for (int k = 0; k < roomLength; k++)
        {
            for (int j = 2; j < 5; j++)
            {
                Vector3 offsetC = new Vector3(roomWidth / 2f - 0.5f, j, (-roomLength / 2f) + k + 0.5f);
                Vector3 offsetD = new Vector3(-roomWidth / 2f + 0.5f, j, (-roomLength / 2f) + k + 0.5f);

                SpawnBlock(wallsParent3, node, rotation, offsetC, "wallBlock");
                SpawnBlock(wallsParent4, node, rotation, offsetD, "wallBlock");
            }
        }

        // === Floor ===
        for (int i = 0; i < roomWidth; i++)
        {
            for (int k = 0; k < roomLength; k++)
            {
                Vector3 offset = new Vector3((-roomWidth / 2f) + i + 0.5f, -floorLevel, (-roomLength / 2f) + k + 0.5f);

                SpawnBlock(floorParent, node, rotation, offset, "floorBlock");
            }
        }

        floors.Add(floorParent);

        // === Ceiling ===
        for (int i = 0; i < roomWidth; i++)
        {
            for (int k = 0; k < roomLength; k++)
            {
                Vector3 offset = new Vector3((-roomWidth / 2f) + i + 0.5f, ceilingLevel, (-roomLength / 2f) + k + 0.5f);

                SpawnBlock(ceilingParent, node, rotation, offset, "ceilingBlock");
            }
        }

        BoxCollider boxC = roomAsGameObject.AddComponent<BoxCollider>();
        boxC.center = new Vector3(0, (ceilingLevel - floorLevel) / 2f, 0);
        roomComponent.roomArea = roomWidth * roomLength;
        boxC.size = new Vector3(roomWidth - 2, ceilingLevel - 1.5f, roomLength - 2);
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

                // === CASE 1: Room deleting corridor overlap ===
                if (!room.isCorridor)
                {
                    if (col.CompareTag("wallBlock"))
                    {
                        Destroy(col.gameObject);
                        continue;
                    }
                }

                if (!room.isCorridor && col.CompareTag("floorBlock") && col.transform.parent.gameObject.transform.parent.name == "Corridor")
                {
                    Destroy(col.gameObject);
                    continue;
                }

                if (!room.isCorridor && col.CompareTag("ceilingBlock") && col.transform.parent.gameObject.transform.parent.name == "Corridor")
                {
                    Destroy(col.gameObject);
                    continue;
                }

                // === CASE 2: Corridor carving through walls ===
                if (room.isCorridor && col.CompareTag("wallBlock"))
                {
                    // if(col.gameObject.transform.position.y ==2f)
                    // {

                    // }
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

    
}