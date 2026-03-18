using System.Collections;
using System.Collections.Generic;
using System.Numerics;
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

    private DungeonRoomBuilder builder;
    private Transform floorRoot;

    public void FillReferences(DungeonRoomBuilder builder, Transform floorRoot)
    {
        this.builder = builder;
        this.floorRoot = floorRoot;
    }


    void Awake()
    {
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

        Transform rooms = DungeonManager.Instance.activeFloorRoot.Find("Rooms");

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

            UnityEngine.Vector3 roomCenter = room.transform.position;
            float roomWidth = room.node.width;
            float roomLength = room.node.length;
            UnityEngine.Quaternion rot = room.transform.rotation;
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
            if (room == null) continue;
            if (room.isCorridor) continue;

            if (!roomItemDatabase.roomItems.TryGetValue(room.roomType, out List<RoomItem> items))
                continue;

            room.EnsureDecorationRoots();
            Transform parent = room.roomItemsRoot;

            RoomGrid grid = new RoomGrid(room);

            List<RoomItem> primary = items.FindAll(i => i.priority == RoomItem.Priority.Primary);
            List<RoomItem> secondary = items.FindAll(i => i.priority == RoomItem.Priority.Secondary);
            List<RoomItem> tertiary = items.FindAll(i => i.priority == RoomItem.Priority.Tertiary);

            PlaceItemGroup(grid, room, primary, parent);
            PlaceItemGroup(grid, room, secondary, parent);
            PlaceItemGroup(grid, room, tertiary, parent);
        }
    }
    public UnityEngine.Vector3 RandomRoomPosition(Room room)
    {
        UnityEngine.Vector3 roomCenter = room.node.center;
        float roomWidth = room.node.width;
        float roomLength = room.node.length;

        float randomX = Random.Range(roomCenter.x - roomWidth / 2f + 1f, roomCenter.x + roomWidth / 2f - 1f);
        float randomZ = Random.Range(roomCenter.z - roomLength / 2f + 1f, roomCenter.z + roomLength / 2f - 1f);

        return new UnityEngine.Vector3(randomX, roomCenter.y, randomZ);
    }

    void GeneratePillars(Room room, UnityEngine.Quaternion rot, UnityEngine.Vector3 roomCenter, float roomWidth, float roomLength)
    {
        if (room == null) return;
        if (pillarPrefab == null) return;
        if (room.isCorridor) return;

        room.EnsureDecorationRoots();
        if (room.pillarsRoot == null) return;

        Transform pillarParent = room.pillarsRoot;
        pillarParent.localPosition = new UnityEngine.Vector3(0f, 1.5f, 0f);
        pillarParent.localRotation = UnityEngine.Quaternion.identity;

        int width = Mathf.FloorToInt(roomWidth);
        int length = Mathf.FloorToInt(roomLength);

        if (width < 8 || length < 8)
            return;

        float pillarY = 1.5f;

        float edgeInset = 2f;
        float midInsetX = Mathf.Max(2f, Mathf.Floor(width * 0.25f));
        float midInsetZ = Mathf.Max(2f, Mathf.Floor(length * 0.25f));

        List<UnityEngine.Vector3> candidateOffsets = new List<UnityEngine.Vector3>();

        bool veryLargeRoom = width >= 14 && length >= 14;
        bool longRoom = length >= width + 4;
        bool wideRoom = width >= length + 4;

        if (veryLargeRoom)
        {
            candidateOffsets.Add(new UnityEngine.Vector3(-midInsetX, pillarY, -midInsetZ));
            candidateOffsets.Add(new UnityEngine.Vector3(-midInsetX, pillarY, midInsetZ));
            candidateOffsets.Add(new UnityEngine.Vector3(midInsetX, pillarY, -midInsetZ));
            candidateOffsets.Add(new UnityEngine.Vector3(midInsetX, pillarY, midInsetZ));

            if (width >= 18)
            {
                candidateOffsets.Add(new UnityEngine.Vector3(0f, pillarY, -midInsetZ));
                candidateOffsets.Add(new UnityEngine.Vector3(0f, pillarY, midInsetZ));
            }

            if (length >= 18)
            {
                candidateOffsets.Add(new UnityEngine.Vector3(-midInsetX, pillarY, 0f));
                candidateOffsets.Add(new UnityEngine.Vector3(midInsetX, pillarY, 0f));
            }
        }
        else if (longRoom)
        {
            candidateOffsets.Add(new UnityEngine.Vector3(0f, pillarY, -midInsetZ));
            candidateOffsets.Add(new UnityEngine.Vector3(0f, pillarY, midInsetZ));

            if (length >= 12)
            {
                candidateOffsets.Add(new UnityEngine.Vector3(-2f, pillarY, 0f));
                candidateOffsets.Add(new UnityEngine.Vector3(2f, pillarY, 0f));
            }
        }
        else if (wideRoom)
        {
            candidateOffsets.Add(new UnityEngine.Vector3(-midInsetX, pillarY, 0f));
            candidateOffsets.Add(new UnityEngine.Vector3(midInsetX, pillarY, 0f));

            if (width >= 12)
            {
                candidateOffsets.Add(new UnityEngine.Vector3(0f, pillarY, -2f));
                candidateOffsets.Add(new UnityEngine.Vector3(0f, pillarY, 2f));
            }
        }
        else
        {
            candidateOffsets.Add(new UnityEngine.Vector3(-edgeInset, pillarY, -edgeInset));
            candidateOffsets.Add(new UnityEngine.Vector3(-edgeInset, pillarY, edgeInset));
            candidateOffsets.Add(new UnityEngine.Vector3(edgeInset, pillarY, -edgeInset));
            candidateOffsets.Add(new UnityEngine.Vector3(edgeInset, pillarY, edgeInset));
        }

        for (int i = 0; i < candidateOffsets.Count; i++)
        {
            UnityEngine.Vector3 localOffset = candidateOffsets[i];
            localOffset.x = Mathf.Round(localOffset.x);
            localOffset.z = Mathf.Round(localOffset.z);

            UnityEngine.Vector3 worldPos = roomCenter + rot * localOffset;

            TryPlaceGenericObject(room, pillarPrefab, worldPos, rot, pillarParent, 0.35f, 0.45f);
        }
    }
    void GenerateTorches(Room room, UnityEngine.Quaternion rot, UnityEngine.Vector3 roomCenter, float roomWidth, float roomLength)
    {
        if (room == null) return;
        if (torchPrefab == null) return;
        if (room.isCorridor) return;

        room.EnsureDecorationRoots();
        if (room.torchesRoot == null) return;

        Transform torchParent = room.torchesRoot;
        torchParent.localPosition = UnityEngine.Vector3.zero;
        torchParent.localRotation = UnityEngine.Quaternion.identity;

        int width = Mathf.FloorToInt(roomWidth);
        int length = Mathf.FloorToInt(roomLength);

        if (width < 5 || length < 5)
            return;

        float inwardOffset = 0.2f;
        float verticalOffset = -0.5f;
        float torchY = torchHeight + verticalOffset;

        int northSouthCount = Mathf.Clamp(width / 4, 1, 4);
        int eastWestCount = Mathf.Clamp(length / 4, 1, 4);

        if (width <= 6) northSouthCount = 1;
        if (length <= 6) eastWestCount = 1;

        for (int i = 0; i < northSouthCount; i++)
        {
            float t = northSouthCount == 1 ? 0.5f : (float)i / (northSouthCount - 1);

            float x = Mathf.Lerp(
                -roomWidth / 2f + 1.5f,
                roomWidth / 2f - 1.5f,
                t
            );

            UnityEngine.Vector3 northOffset = new UnityEngine.Vector3(
                x,
                torchY,
                roomLength / 2f - wallInset - inwardOffset
            );

            UnityEngine.Vector3 southOffset = new UnityEngine.Vector3(
                x,
                torchY,
                -roomLength / 2f + wallInset + inwardOffset
            );

            UnityEngine.Vector3 northPos = roomCenter + rot * northOffset;
            UnityEngine.Vector3 southPos = roomCenter + rot * southOffset;

            UnityEngine.Quaternion northRot = rot * UnityEngine.Quaternion.Euler(0f, 180f, 0f);
            UnityEngine.Quaternion southRot = rot;

            TryPlaceGenericObject(
                room,
                torchPrefab,
                northPos,
                northRot,
                torchParent,
                0.05f,
                0.4f
            );

            TryPlaceGenericObject(
                room,
                torchPrefab,
                southPos,
                southRot,
                torchParent,
                0.05f,
                0.4f
            );
        }

        for (int i = 0; i < eastWestCount; i++)
        {
            float t = eastWestCount == 1 ? 0.5f : (float)i / (eastWestCount - 1);

            float z = Mathf.Lerp(
                -roomLength / 2f + 1.5f,
                roomLength / 2f - 1.5f,
                t
            );

            UnityEngine.Vector3 eastOffset = new UnityEngine.Vector3(
                roomWidth / 2f - wallInset - inwardOffset,
                torchY,
                z
            );

            UnityEngine.Vector3 westOffset = new UnityEngine.Vector3(
                -roomWidth / 2f + wallInset + inwardOffset,
                torchY,
                z
            );

            UnityEngine.Vector3 eastPos = roomCenter + rot * eastOffset;
            UnityEngine.Vector3 westPos = roomCenter + rot * westOffset;

            UnityEngine.Quaternion eastRot = rot * UnityEngine.Quaternion.Euler(0f, 270f, 0f);
            UnityEngine.Quaternion westRot = rot * UnityEngine.Quaternion.Euler(0f, 90f, 0f);

            TryPlaceGenericObject(
                room,
                torchPrefab,
                eastPos,
                eastRot,
                torchParent,
                0.05f,
                0.4f
            );

            TryPlaceGenericObject(
                room,
                torchPrefab,
                westPos,
                westRot,
                torchParent,
                0.05f,
                0.4f
            );
        }
    }

    void ReplacePillarsWithTorchPillars(Room room)
    {
        if (room == null)
        {
            Debug.LogWarning("ReplacePillarsWithTorchPillars called with null room.");
            return;
        }

        if (torchPillarPrefab == null)
        {
            Debug.LogWarning("Torch pillar prefab is missing on DungeonRoomDecorator.");
            return;
        }

        room.EnsureDecorationRoots();

        if (room.pillarsRoot == null)
        {
            Debug.LogWarning("Room pillarsRoot is still null after EnsureDecorationRoots on room: " + room.name);
            return;
        }

        Transform pillarParent = room.pillarsRoot;

        List<Transform> pillarsToReplace = new List<Transform>();

        for (int i = 0; i < pillarParent.childCount; i++)
        {
            Transform pillar = pillarParent.GetChild(i);
            if (pillar == null) continue;

            float replaceChance = Random.Range(0f, 1f);
            if (replaceChance <= 0.3f)
            {
                pillarsToReplace.Add(pillar);
            }
        }

        for (int i = 0; i < pillarsToReplace.Count; i++)
        {
            Transform pillar = pillarsToReplace[i];
            if (pillar == null) continue;

            UnityEngine.Vector3 position = pillar.position;
            UnityEngine.Quaternion rotation = pillar.rotation;

            Destroy(pillar.gameObject);

            TryPlaceGenericObject(room, torchPillarPrefab, position, rotation, pillarParent, 0.3f, 0.4f);
        }
    }

    void DeleteRandomGenericDecor(Room room)
    {
        room.EnsureDecorationRoots();

        Transform pillars = room.pillarsRoot;
        Transform torches = room.torchesRoot;

        float deleteChance = Random.Range(0.1f, 0.4f);
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
        foreach (Room room in builder.allRooms)
        {
            foreach (Transform door in room.doorways)
            {   
                UnityEngine.Vector3 size = new UnityEngine.Vector3(5f, 3f, 5f);
                Bounds doorBounds = new Bounds(door.position + UnityEngine.Vector3.up * 1.5f, size);

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

    public void CullDecor()
    {
        foreach (Room room in builder.allRooms)
        {
            GameObject roomDecorRoot = room.roomItemsRoot.gameObject;
            foreach (Transform decor in roomDecorRoot.transform)
            {
                Collider col = decor.GetComponentInChildren<Collider>();
                if (col == null) continue;

                UnityEngine.Vector3 roomCenter = room.node.center;
                float roomWidth = room.node.width;
                float roomLength = room.node.length;

                Bounds roomBounds = new Bounds(roomCenter, new UnityEngine.Vector3(roomWidth, 10f, roomLength));

                if (!roomBounds.Intersects(col.bounds))
                {
                    Destroy(decor.gameObject);
                }
            }
        }
    }

    void PlaceItemGroup(RoomGrid grid, Room room, List<RoomItem> items, Transform parent)
    {
        foreach (RoomItem item in items)
        {
            if (item == null || item.prefab == null) continue;

            int spawnCount = Random.Range(item.minCount, item.maxCount + 1);

            for (int i = 0; i < spawnCount; i++)
            {
                TryPlaceItem(grid, room, item, parent);
            }
        }
    }

    bool TryPlaceItem(RoomGrid grid, Room room, RoomItem item, Transform parent)
    {
        float bestScore = float.NegativeInfinity;
        Vector2Int bestCell = new Vector2Int(-1, -1);
        int bestWidth = item.gridWidth;
        int bestLength = item.gridLength;
        UnityEngine.Quaternion bestRotation = UnityEngine.Quaternion.identity;

        TryEvaluateOrientation(grid, room, item, item.gridWidth, item.gridLength, UnityEngine.Quaternion.identity, ref bestScore, ref bestCell, ref bestWidth, ref bestLength, ref bestRotation);

        if (item.allowRotation && item.gridWidth != item.gridLength)
        {
            TryEvaluateOrientation(grid, room, item, item.gridLength, item.gridWidth, UnityEngine.Quaternion.Euler(0f, 90f, 0f), ref bestScore, ref bestCell, ref bestWidth, ref bestLength, ref bestRotation);
        }

        if (bestCell.x < 0 || bestCell.y < 0)
            return false;

        UnityEngine.Vector3 spawnPos = grid.GetPlacementWorldCenter(bestCell.x, bestCell.y, bestWidth, bestLength);
        GameObject instance = Instantiate(item.prefab, spawnPos, room.transform.rotation * bestRotation, parent);

        grid.Reserve(bestCell.x, bestCell.y, bestWidth, bestLength);

        string tagToUse = string.IsNullOrEmpty(item.itemTag) ? item.name : item.itemTag;
        grid.anchors.Add(new DecorAnchor(tagToUse, bestCell, new Vector2Int(bestWidth, bestLength), instance.transform));

        return true;
    }

    void TryEvaluateOrientation(
        RoomGrid grid,
        Room room,
        RoomItem item,
        int itemWidth,
        int itemLength,
        UnityEngine.Quaternion rotation,
        ref float bestScore,
        ref Vector2Int bestCell,
        ref int bestWidth,
        ref int bestLength,
        ref UnityEngine.Quaternion bestRotation)
    {
        for (int x = 0; x <= grid.width - itemWidth; x++)
        {
            for (int z = 0; z <= grid.length - itemLength; z++)
            {
                if (!grid.CanPlace(x, z, itemWidth, itemLength, item.extraClearance))
                    continue;

                float score = grid.ScorePlacement(item, x, z, itemWidth, itemLength);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestCell = new Vector2Int(x, z);
                    bestWidth = itemWidth;
                    bestLength = itemLength;
                    bestRotation = rotation;
                }
            }
        }
    }
    bool IsSelfOrChild(Transform root, Transform other)
    {
        if (root == null || other == null) return false;
        return other == root || other.IsChildOf(root);
    }

    bool TryGetCombinedColliderBounds(GameObject obj, out Bounds combinedBounds)
    {
        combinedBounds = new Bounds();
        Collider[] colliders = obj.GetComponentsInChildren<Collider>();

        bool found = false;

        for (int i = 0; i < colliders.Length; i++)
        {
            Collider col = colliders[i];
            if (col == null || !col.enabled) continue;

            if (!found)
            {
                combinedBounds = col.bounds;
                found = true;
            }
            else
            {
                combinedBounds.Encapsulate(col.bounds);
            }
        }

        return found;
    }

    bool IsWithinRoomBounds(Room room, Bounds objectBounds, float inset)
    {
        UnityEngine.Vector3 roomCenter = room.transform.position;

        float halfWidth = Mathf.FloorToInt(room.node.width) * 0.5f;
        float halfLength = Mathf.FloorToInt(room.node.length) * 0.5f;

        float minX = roomCenter.x - halfWidth + inset;
        float maxX = roomCenter.x + halfWidth - inset;
        float minZ = roomCenter.z - halfLength + inset;
        float maxZ = roomCenter.z + halfLength - inset;

        if (objectBounds.min.x < minX) return false;
        if (objectBounds.max.x > maxX) return false;
        if (objectBounds.min.z < minZ) return false;
        if (objectBounds.max.z > maxZ) return false;

        return true;
    }

    bool IntersectsDoorwayBounds(Room room, Bounds objectBounds, float padding)
    {
        for (int i = 0; i < room.doorways.Count; i++)
        {
            Transform door = room.doorways[i];
            if (door == null) continue;

            Bounds doorBounds = new Bounds(
                door.position + UnityEngine.Vector3.up * 1.5f,
                new UnityEngine.Vector3(3f + padding, 3f, 3f + padding)
            );

            if (doorBounds.Intersects(objectBounds))
                return true;
        }

        return false;
    }

    bool IntersectsExistingDecor(Room room, GameObject instance, Bounds objectBounds)
    {
        Collider[] ownColliders = instance.GetComponentsInChildren<Collider>();

        room.EnsureDecorationRoots();

        Transform[] roots =
        {
            room.pillarsRoot,
            room.torchesRoot,
            room.roomItemsRoot
        };

        for (int r = 0; r < roots.Length; r++)
        {
            Transform root = roots[r];
            if (root == null) continue;

            Collider[] existing = root.GetComponentsInChildren<Collider>();

            for (int i = 0; i < existing.Length; i++)
            {
                Collider other = existing[i];
                if (other == null || !other.enabled) continue;

                if (IsSelfOrChild(instance.transform, other.transform))
                    continue;

                for (int j = 0; j < ownColliders.Length; j++)
                {
                    Collider own = ownColliders[j];
                    if (own == null || !own.enabled) continue;

                    if (own.bounds.Intersects(other.bounds))
                        return true;
                }
            }
        }

        return false;
    }

    void TagDecorRecursive(GameObject obj)
    {
        obj.tag = "Decor";

        for (int i = 0; i < obj.transform.childCount; i++)
        {
            TagDecorRecursive(obj.transform.GetChild(i).gameObject);
        }
    }

    GameObject TryPlaceGenericObject(
        Room room,
        GameObject prefab,
        UnityEngine.Vector3 position,
        UnityEngine.Quaternion rotation,
        Transform parent,
        float roomInset = 0.2f,
        float doorwayPadding = 0.25f)
    {
        if (prefab == null) return null;

        GameObject instance = Instantiate(prefab, position, rotation, parent);
        TagDecorRecursive(instance);

        Bounds objectBounds;
        if (!TryGetCombinedColliderBounds(instance, out objectBounds))
        {
            Destroy(instance);
            return null;
        }

        if (!IsWithinRoomBounds(room, objectBounds, roomInset))
        {
            Destroy(instance);
            return null;
        }

        if (IntersectsDoorwayBounds(room, objectBounds, doorwayPadding))
        {
            Destroy(instance);
            return null;
        }

        if (IntersectsExistingDecor(room, instance, objectBounds))
        {
            Destroy(instance);
            return null;
        }

        return instance;
    }
}
