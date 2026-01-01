using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoomDecorator : MonoBehaviour
{

    public GameObject pillarPrefab;
    public GameObject torchPrefab;
    public GameObject torchPillarPrefab;

    [Range(0, 5)] public float torchHeight = 3.5f;
    [Range(-3, 4)] public float wallInset = 1f;

    public List<GenericRoomItem> genericRoomItems = new List<GenericRoomItem>();
    public DungeonRoomBuilder dungeonRoomBuilder;
    [SerializeField] private RoomItemDatabase roomItemDatabase;

    public bool generatePillars = false;
    public bool replacePillarsWithTorchPillars = false;
    public bool generateTorches = false;
    public bool cleanGenericDecor = false;

    public List<Room> allWorkableRooms = new List<Room>();

    public static DungeonRoomDecorator Instance;


    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        genericRoomItems.Add(new GenericRoomItem()
        {
            name = "Torch",
            prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Generic/Torch"),
            type = GenericRoomItem.GenericType.Torch
        });

        genericRoomItems.Add(new GenericRoomItem()
        {
            name = "Pillar",
            prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Generic/Pillar"),
            type = GenericRoomItem.GenericType.Pillar
        });

        genericRoomItems.Add(new GenericRoomItem()
        {
            name = "TorchPillar",
            prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Generic/TorchPillar"),
            type = GenericRoomItem.GenericType.TorchPillar
        });

        genericRoomItems.Add(new GenericRoomItem()
        {
            name = "Banner",
            prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Generic/Banner"),
            type = GenericRoomItem.GenericType.Banner
        });
    }

    public void PopulateWorkableRooms()
    {
        allWorkableRooms.Clear();

        Transform rooms = GameObject.Find("DungeonGenerator").transform.Find("Rooms");

        foreach (Transform child in rooms)
        {
            Room roomComp = child.GetComponent<Room>();
            if (roomComp != null)
            {
                allWorkableRooms.Add(roomComp);
            }
        }
    }

    public void DecorateRoomsGenerically()
    {
        if (allWorkableRooms.Count == 0)
            {
                Debug.LogWarning("No rooms available for decoration.");
                return;
            }

        foreach (Room room in allWorkableRooms)
        {   
            
            if (room.isCorridor) continue;

            Vector3 roomCenter = room.transform.position;
            float roomWidth = room.node.width;
            float roomLength = room.node.length;
            Quaternion rot = room.transform.rotation;
            room.roomType = GetRandomRoomType();

            // Only perform what’s enabled this pass
            if (generatePillars)
                GeneratePillars(room, rot, roomCenter, roomWidth, roomLength);

            if (replacePillarsWithTorchPillars)
                ReplacePillarsWithTorchPillars(room);

            if (generateTorches)
                GenerateTorches(room, rot, roomCenter, roomWidth, roomLength);

            if (cleanGenericDecor)
            DeleteRandomGenericDecor(room);
        }

        // Reset flags automatically after all rooms are processed
        generatePillars = false;
        replacePillarsWithTorchPillars = false;
        generateTorches = false;
    }


    public void DecorateRooms()
    {
        foreach (Room room in allWorkableRooms)
        {
            float availableRoomArea = room.roomArea * Random.Range(0.3f, 0.6f);
            availableRoomArea = Mathf.Floor(availableRoomArea);
            room.availableArea = availableRoomArea;

            if (room.isCorridor) continue;
            if (availableRoomArea <= 0) continue;

            if (!roomItemDatabase.roomItems.TryGetValue(room.roomType, out List<RoomItem> items))
            {
                continue;
            }

            // Split items by priority
            List<RoomItem> primaryItems = items.FindAll(i => i.priority == RoomItem.Priority.Primary);
            List<RoomItem> secondaryItems = items.FindAll(i => i.priority == RoomItem.Priority.Secondary);
            List<RoomItem> tertiaryItems = items.FindAll(i => i.priority == RoomItem.Priority.Tertiary);

            room.EnsureDecorationRoots();
            GameObject roomItemParent = room.roomItemsRoot.gameObject;

            // === PRIMARY ===
            foreach (RoomItem item in primaryItems)
            {
                float itemArea = GetItemArea(item.prefab);
                if (room.availableArea - itemArea <= 0) continue;

                Collider col = item.prefab.GetComponent<Collider>();
                Bounds b = col.bounds;
                float itemHeight = b.size.y;

                // Always spawn once
                Vector3 position = RandomRoomPosition(room);
                position = new Vector3(
                            Mathf.Floor(position.x), 
                            position.y + b.size.y / 2f + 2f, 
                            Mathf.Floor(position.z)
                );
                Quaternion rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                Instantiate(item.prefab, position, rotation, roomItemParent.transform);

                room.availableArea -= itemArea;
            }

            // === SECONDARY ===
            if (secondaryItems.Count > 0 && room.availableArea > 0)
            {
                // Decide count dynamically based on remaining space
                int secondaryCount = Mathf.Clamp(
                    Mathf.FloorToInt(room.availableArea / 20f),
                    1,
                    Random.Range(2, 6)
                );

                for (int i = 0; i < secondaryCount; i++)
                {
                    RoomItem item = secondaryItems[Random.Range(0, secondaryItems.Count)];
                    float itemArea = GetItemArea(item.prefab);
                    if (room.availableArea - itemArea <= 0) break;

                    Collider col = item.prefab.GetComponent<Collider>();
                    Bounds b = col.bounds;
                    float itemHeight = b.size.y;

                    Vector3 position = RandomRoomPosition(room);
                    position = new Vector3(
                            Mathf.Floor(position.x), 
                            position.y + b.size.y / 2f + 2f, 
                            Mathf.Floor(position.z)
                    );
                    Quaternion rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                    Instantiate(item.prefab, position, rotation, roomItemParent.transform);

                    room.availableArea -= itemArea;
                }
            }

            // === TERTIARY ===
            if (tertiaryItems.Count > 0 && room.availableArea > 0)
            {
                int tertiaryCount = Mathf.Clamp(
                    Mathf.FloorToInt(room.availableArea / 10f),
                    1,
                    Random.Range(3, 8)
                );

                for (int i = 0; i < tertiaryCount; i++)
                {
                    RoomItem item = tertiaryItems[Random.Range(0, tertiaryItems.Count)];
                    float itemArea = GetItemArea(item.prefab);
                    if (room.availableArea - itemArea <= 0) break;

                    Collider col = item.prefab.GetComponent<Collider>();
                    Bounds b = col.bounds;
                    float itemHeight = b.size.y;

                    Vector3 position = RandomRoomPosition(room);
                    position = new Vector3(
                            Mathf.Floor(position.x), 
                            position.y + b.size.y / 2f + 2f, 
                            Mathf.Floor(position.z)
                    );
                    Quaternion rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                    Instantiate(item.prefab, position, rotation, roomItemParent.transform);

                    room.availableArea -= itemArea;
                }
            }
        }
    }

    public Vector3 RandomRoomPosition(Room room)
    {
        Vector3 roomCenter = room.node.center;
        float roomWidth = room.node.width;
        float roomLength = room.node.length;

        float randomX = Random.Range(roomCenter.x - roomWidth / 2f + 1f, roomCenter.x + roomWidth / 2f - 1f);
        float randomZ = Random.Range(roomCenter.z - roomLength / 2f + 1f, roomCenter.z + roomLength / 2f - 1f);

        return new Vector3(randomX, roomCenter.y, randomZ);
    }

    void GeneratePillars(Room room, Quaternion rot, Vector3 roomCenter, float roomWidth, float roomLength)
    {
        float pillarSpacing = Random.Range(5f, 10f);
        pillarSpacing = Mathf.Floor(pillarSpacing);

        room.EnsureDecorationRoots();
        Transform pillarParent = room.pillarsRoot;

        pillarParent.localPosition = new Vector3(0, 0, 0);
        pillarParent.localRotation = Quaternion.identity;

        Transform floor = room.transform.Find("Floor");
        Renderer floorRenderer = floor.GetComponent<Renderer>();
        float floorTopY = floorRenderer.bounds.max.y;

        float pillarHeight = 3f;
        float pillarY = floorTopY + (pillarHeight * 0.5f);

        for (float i = 0; i < roomWidth - pillarSpacing; i += pillarSpacing)
        {
            Vector3 offsetA = new Vector3((-roomWidth / 2f) + i + 0.5f + pillarSpacing, pillarY - roomCenter.y, roomLength / 2f - 0.5f - pillarSpacing);
            Vector3 offsetB = new Vector3((-roomWidth / 2f) + i + 0.5f + pillarSpacing, pillarY - roomCenter.y, -roomLength / 2f + 0.5f + pillarSpacing);

            offsetA.x = Mathf.Floor(offsetA.x);
            offsetA.z = Mathf.Floor(offsetA.z);
            offsetB.x = Mathf.Floor(offsetB.x);
            offsetB.z = Mathf.Floor(offsetB.z);

            Vector3 posA = roomCenter + rot * offsetA;
            Vector3 posB = roomCenter + rot * offsetB;

            Instantiate(pillarPrefab, posA, rot, pillarParent);
            Instantiate(pillarPrefab, posB, rot, pillarParent);
        }

        for (float k = 0; k < roomLength - pillarSpacing; k += pillarSpacing)
        {
            Vector3 offsetC = new Vector3(roomWidth / 2f - 0.5f - pillarSpacing, pillarY - roomCenter.y, (-roomLength / 2f) + k + 0.5f + pillarSpacing);
            Vector3 offsetD = new Vector3(-roomWidth / 2f + 0.5f + pillarSpacing, pillarY - roomCenter.y, (-roomLength / 2f) + k + 0.5f + pillarSpacing);

            offsetC.x = Mathf.Floor(offsetC.x);
            offsetC.z = Mathf.Floor(offsetC.z);
            offsetD.x = Mathf.Floor(offsetD.x);
            offsetD.z = Mathf.Floor(offsetD.z);

            Vector3 posC = roomCenter + rot * offsetC;
            Vector3 posD = roomCenter + rot * offsetD;

            Instantiate(pillarPrefab, posC, rot, pillarParent);
            Instantiate(pillarPrefab, posD, rot, pillarParent);
        }
    }

    void GenerateTorches(Room room, Quaternion rot, Vector3 roomCenter, float roomWidth, float roomLength)
    {
        if (room.isCorridor) return;

        room.EnsureDecorationRoots();
        Transform torchParent = room.torchesRoot;
        torchParent.localPosition = Vector3.zero;
        torchParent.localRotation = Quaternion.identity;

        // === Torch placement parameters ===

        float spacing = Mathf.Clamp(Random.Range(6f, 10f), 4f, 10f); // room-dependent spacing

        // === North/South walls ===
        for (float i = spacing / 2; i < roomWidth - spacing / 2; i += spacing)
        {
            Vector3 northOffset = new Vector3(-roomWidth / 2f + i, torchHeight, roomLength / 2f - wallInset);
            Vector3 southOffset = new Vector3(-roomWidth / 2f + i, torchHeight, -roomLength / 2f + wallInset);

            Vector3 northPos = roomCenter + rot * northOffset;
            Vector3 southPos = roomCenter + rot * southOffset;

            Quaternion northRot = rot * Quaternion.Euler(0, 180, 0);
            Quaternion southRot = rot;

            Instantiate(torchPrefab, northPos, northRot, torchParent);
            Instantiate(torchPrefab, southPos, southRot, torchParent);
        }

        // === East/West walls ===
        for (float k = spacing / 2; k < roomLength - spacing / 2; k += spacing)
        {
            Vector3 eastOffset = new Vector3(roomWidth / 2f - wallInset, torchHeight, -roomLength / 2f + k);
            Vector3 westOffset = new Vector3(-roomWidth / 2f + wallInset, torchHeight, -roomLength / 2f + k);

            Vector3 eastPos = roomCenter + rot * eastOffset;
            Vector3 westPos = roomCenter + rot * westOffset;

            Quaternion eastRot = rot * Quaternion.Euler(0, 270, 0);
            Quaternion westRot = rot * Quaternion.Euler(0, 90, 0);

            Instantiate(torchPrefab, eastPos, eastRot, torchParent);
            Instantiate(torchPrefab, westPos, westRot, torchParent);
        }
    }

    void ReplacePillarsWithTorchPillars(Room room)
    {
        room.EnsureDecorationRoots();

        Transform pillarParent = room.pillarsRoot;

        List<Transform> pillarsToReplace = new List<Transform>();

        foreach (Transform pillar in pillarParent)
        {
            float replaceChance = Random.Range(0f, 1f);
            if (replaceChance <= 0.3f) // 30% chance to replace pillar with torch pillar
            {
                pillarsToReplace.Add(pillar);
            }
        }

        foreach (Transform pillar in pillarsToReplace)
        {
            Vector3 position = pillar.position;
            Quaternion rotation = pillar.rotation;
            Destroy(pillar.gameObject);
            Instantiate(torchPillarPrefab, position, rotation, pillarParent);
        }
    }

    void DeleteRandomGenericDecor(Room room)
    {
        room.EnsureDecorationRoots();

        Transform pillars = room.pillarsRoot;
        Transform torches = room.torchesRoot;

        float deleteChance = Random.Range(0.3f, 0.6f);
        for(int i = 0; i < pillars.transform.childCount; i++)
        {
            Transform child = pillars.transform.GetChild(i);
            float chance = Random.Range(0.3f, 1f);
            if (chance < deleteChance)
            {
                Destroy(child.gameObject);
            }
        }
        
        for(int i = 0; i < torches.transform.childCount; i++)
        {
            Transform child = torches.transform.GetChild(i);
            float chance = Random.Range(0.3f, 1f);
            if (chance < deleteChance)
            {
                Destroy(child.gameObject);
            }
        }
    }

    float GetItemArea(GameObject prefab)
    {
        Collider col = prefab.GetComponent<Collider>();
        if (col == null) return 1f;
        Bounds b = col.bounds;
        return b.size.x * b.size.z;
    }


    Room.RoomType GetRandomRoomType()
    {
        var values = System.Enum.GetValues(typeof(Room.RoomType));
        return (Room.RoomType)values.GetValue(Random.Range(0, values.Length));
    }

    public void FinalizeDecor()
    {
        foreach (Room room in allWorkableRooms)
        {
            room.FinalizeDecorations();
        }
    }

    public void ClearDoorways()
    {
        foreach (Room room in DungeonRoomBuilder.Instance.allRooms)
        {
            foreach (Transform door in room.doorways)
            {   
                Vector3 size = new Vector3(5f, 3f, 5f);
                Bounds doorBounds = new Bounds(door.position + Vector3.up * 1.5f, size);

                GameObject[] decorObjects = GameObject.FindGameObjectsWithTag("Decor");
                foreach (GameObject decor in decorObjects)
                {
                    Collider col = decor.GetComponentInChildren<Collider>();
                    if (col == null) continue;

                    if (doorBounds.Intersects(col.bounds))
                    {
                        Destroy(decor);
                    }
                }
            }
        }
    }

 
}
