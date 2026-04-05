using System.Collections.Generic;
using UnityEngine;

public class DungeonRoomDecorator : MonoBehaviour
{
    [Header("Generic Prefabs")]
    public GameObject pillarPrefab;
    public GameObject torchPrefab;
    public GameObject torchPillarPrefab;

    [Header("Generic Settings")]
    [Range(0, 5)] public float torchHeight = 3.5f;
    [Range(-3, 4)] public float wallInset = 1f;

    [Header("References")]
    public DungeonRoomBuilder dungeonRoomBuilder;
    [SerializeField] RoomItemDatabase roomItemDatabase;

    [Header("Legacy Pass Toggles")]
    public bool generatePillars = true;
    public bool replacePillarsWithTorchPillars = true;
    public bool generateTorches = true;
    public bool cleanGenericDecor = false;

    public List<Room> allWorkableRooms = new List<Room>();

    DungeonRoomBuilder builder;
    Transform floorRoot;

    public Dictionary<Room, RoomDecorPlan> roomPlans = new Dictionary<Room, RoomDecorPlan>();

    public RoomDecorPlanner roomDecorPlanner = new RoomDecorPlanner();
    public RoomSpecificDecorPlacer roomSpecificDecorPlacer = new RoomSpecificDecorPlacer();
    public RoomDecorInstantiator roomDecorInstantiator = new RoomDecorInstantiator();

    public void FillReferences(DungeonRoomBuilder builder, Transform floorRoot)
    {
        this.builder = builder;
        this.floorRoot = floorRoot;
    }

    void Awake()
    {
        if (pillarPrefab == null)
            pillarPrefab = Resources.Load<GameObject>("Prefabs/RoomItems/Generic/Pillar");

        if (torchPrefab == null)
            torchPrefab = Resources.Load<GameObject>("Prefabs/RoomItems/Generic/Torch");

        if (torchPillarPrefab == null)
            torchPillarPrefab = Resources.Load<GameObject>("Prefabs/RoomItems/Generic/TorchPillar");
    }

    public void PopulateWorkableRooms()
    {
        allWorkableRooms.Clear();

        if (DungeonManager.Instance == null || DungeonManager.Instance.activeFloorRoot == null)
            return;

        Transform rooms = DungeonManager.Instance.activeFloorRoot.Find("Rooms");
        if (rooms == null)
            return;

        foreach (Transform child in rooms)
        {
            Room roomComp = child.GetComponent<Room>();
            if (roomComp != null)
                allWorkableRooms.Add(roomComp);
        }
    }

    RoomDecorPlan GetOrCreatePlan(Room room)
    {
        if (room == null)
            return null;

        if (roomPlans.TryGetValue(room, out RoomDecorPlan existingPlan) && existingPlan != null)
            return existingPlan;

        RoomDecorPlan newPlan = roomDecorPlanner.BuildPlan(room);
        if (newPlan != null)
            roomPlans[room] = newPlan;

        return newPlan;
    }

    void RebuildAllRoomPlans()
    {
        roomPlans.Clear();

        for (int i = 0; i < allWorkableRooms.Count; i++)
        {
            Room room = allWorkableRooms[i];
            if (room == null || room.isCorridor)
                continue;

            RoomDecorPlan plan = roomDecorPlanner.BuildPlan(room);
            if (plan != null)
                roomPlans[room] = plan;
        }
    }

    public Dictionary<Room, RoomDecorPlan> GetRoomPlans()
    {
        return roomPlans;
    }

    public void BuildAllRoomPlans()
    {
        RebuildAllRoomPlans();
    }

    public void PlanSpecificDecorFromPlans()
    {
        if (roomItemDatabase == null)
        {
            Debug.LogWarning("RoomItemDatabase missing on DungeonRoomDecorator.");
            return;
        }

        for (int i = 0; i < allWorkableRooms.Count; i++)
        {
            Room room = allWorkableRooms[i];
            if (room == null || room.isCorridor)
                continue;

            if (!roomItemDatabase.HasItemsForRoom(room.roomType))
                continue;

            RoomDecorPlan plan = GetOrCreatePlan(room);
            if (plan == null)
                continue;

            roomSpecificDecorPlacer.PlanRoom(plan, roomItemDatabase);
        }
    }

    public void PlanGenericDecorFromPlans()
    {
        for (int i = 0; i < allWorkableRooms.Count; i++)
        {
            Room room = allWorkableRooms[i];
            if (room == null || room.isCorridor)
                continue;

            room.EnsureDecorationRoots();
            ClearExistingGenericDecor(room);

            GeneratePillars(room);
            ReplacePillarsWithTorchPillars(room);
            GenerateTorches(room);

            if (cleanGenericDecor)
                DeleteRandomGenericDecor(room);
        }
    }

    public void InstantiateAllDecorFromPlans()
    {
        for (int i = 0; i < allWorkableRooms.Count; i++)
        {
            Room room = allWorkableRooms[i];
            if (room == null || room.isCorridor)
                continue;

            RoomDecorPlan plan = GetOrCreatePlan(room);
            if (plan == null)
                continue;

            room.EnsureDecorationRoots();

            roomDecorInstantiator.InstantiatePlan(
                plan,
                room.roomItemsRoot,
                room.pillarsRoot,
                room.torchesRoot
            );
        }
    }

    public void AssignRoomTypesIfNeeded()
    {
        for (int i = 0; i < allWorkableRooms.Count; i++)
        {
            Room room = allWorkableRooms[i];
            if (room == null || room.isCorridor)
                continue;

            room.roomType = Room.RoomType.Default;
        }
    }

    public void ValidateAllDecor()
    {
        RoomDecorValidator validator = new RoomDecorValidator();

        for (int i = 0; i < allWorkableRooms.Count; i++)
        {
            Room room = allWorkableRooms[i];
            if (room == null || room.isCorridor)
                continue;

            RoomDecorPlan plan = GetOrCreatePlan(room);
            if (plan == null)
                continue;

            validator.ValidatePlan(plan);
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

    void GeneratePillars(Room room)
    {
        if (!generatePillars)
            return;

        if (room == null || room.isCorridor || pillarPrefab == null)
            return;

        int width = Mathf.FloorToInt(room.node.width);
        int length = Mathf.FloorToInt(room.node.length);

        if (width < 8 || length < 8)
            return;

        float floorTopY = GetRoomFloorTopY(room);
        List<Vector3> offsets = BuildPillarOffsets(width, length);

        for (int i = 0; i < offsets.Count; i++)
        {
            Vector3 worldPos = room.transform.position + room.transform.rotation * offsets[i];
            worldPos.y = floorTopY + 1.5f;

            GameObject placed = TryPlaceGenericObjectAtHeight(
                room,
                pillarPrefab,
                worldPos,
                room.transform.rotation,
                room.pillarsRoot,
                0.35f,
                0.45f,
                0.1f
            );

            if (placed != null)
                placed.transform.position += Vector3.up * 1.5f;
        }
    }

    List<Vector3> BuildPillarOffsets(int width, int length)
    {
        List<Vector3> offsets = new List<Vector3>();

        float midInsetX = Mathf.Max(2f, Mathf.Floor(width * 0.25f));
        float midInsetZ = Mathf.Max(2f, Mathf.Floor(length * 0.25f));
        float edgeInset = 2f;

        bool veryLargeRoom = width >= 14 && length >= 14;
        bool longRoom = length >= width + 4;
        bool wideRoom = width >= length + 4;

        if (veryLargeRoom)
        {
            offsets.Add(new Vector3(-midInsetX, 0f, -midInsetZ));
            offsets.Add(new Vector3(-midInsetX, 0f,  midInsetZ));
            offsets.Add(new Vector3( midInsetX, 0f, -midInsetZ));
            offsets.Add(new Vector3( midInsetX, 0f,  midInsetZ));

            offsets.Add(new Vector3(0f, 0f, -midInsetZ));
            offsets.Add(new Vector3(0f, 0f,  midInsetZ));
            offsets.Add(new Vector3(-midInsetX, 0f, 0f));
            offsets.Add(new Vector3( midInsetX, 0f, 0f));

            if (width >= 18 && length >= 18)
                offsets.Add(new Vector3(0f, 0f, 0f));
        }
        else if (longRoom)
        {
            offsets.Add(new Vector3(0f, 0f, -midInsetZ));
            offsets.Add(new Vector3(0f, 0f,  midInsetZ));

            if (length >= 12)
            {
                offsets.Add(new Vector3(-2f, 0f, 0f));
                offsets.Add(new Vector3( 2f, 0f, 0f));
            }
        }
        else if (wideRoom)
        {
            offsets.Add(new Vector3(-midInsetX, 0f, 0f));
            offsets.Add(new Vector3( midInsetX, 0f, 0f));

            if (width >= 12)
            {
                offsets.Add(new Vector3(0f, 0f, -2f));
                offsets.Add(new Vector3(0f, 0f,  2f));
            }
        }
        else
        {
            offsets.Add(new Vector3(-edgeInset, 0f, -edgeInset));
            offsets.Add(new Vector3(-edgeInset, 0f,  edgeInset));
            offsets.Add(new Vector3( edgeInset, 0f, -edgeInset));
            offsets.Add(new Vector3( edgeInset, 0f,  edgeInset));
        }

        for (int i = 0; i < offsets.Count; i++)
        {
            Vector3 o = offsets[i];
            o.x = Mathf.Round(o.x);
            o.z = Mathf.Round(o.z);
            offsets[i] = o;
        }

        return offsets;
    }

    void ReplacePillarsWithTorchPillars(Room room)
    {
        if (!replacePillarsWithTorchPillars)
            return;

        if (room == null || torchPillarPrefab == null || room.pillarsRoot == null)
            return;

        List<Transform> toReplace = new List<Transform>();

        for (int i = 0; i < room.pillarsRoot.childCount; i++)
        {
            Transform pillar = room.pillarsRoot.GetChild(i);
            if (pillar == null)
                continue;

            if (Random.value <= 0.3f)
                toReplace.Add(pillar);
        }

        float floorTopY = GetRoomFloorTopY(room);

        for (int i = 0; i < toReplace.Count; i++)
        {
            Transform pillar = toReplace[i];
            Vector3 pos = pillar.position;
            Quaternion rot = pillar.rotation;

            RemoveOccupiedAreaNearPoint(room, pos, 1.5f);
            Destroy(pillar.gameObject);

            pos.y = floorTopY + 1.5f;

            GameObject placed = TryPlaceGenericObjectAtHeight(
                room,
                torchPillarPrefab,
                pos,
                rot,
                room.pillarsRoot,
                0.3f,
                0.4f,
                0.1f
            );

            if (placed != null)
                placed.transform.position += Vector3.up * 1.5f;
        }
    }

    void GenerateTorches(Room room)
    {
        if (!generateTorches)
            return;

        if (room == null || room.isCorridor || torchPrefab == null)
            return;

        int width = Mathf.FloorToInt(room.node.width);
        int length = Mathf.FloorToInt(room.node.length);

        if (width < 5 || length < 5)
            return;

        float floorTopY = GetRoomFloorTopY(room);
        float inwardOffset = 0.2f;
        float torchWorldY = floorTopY + (torchHeight - 0.5f);

        int northSouthCount = Mathf.Clamp(width / 4, 1, 4);
        int eastWestCount = Mathf.Clamp(length / 4, 1, 4);

        if (width <= 6) northSouthCount = 1;
        if (length <= 6) eastWestCount = 1;

        for (int i = 0; i < northSouthCount; i++)
        {
            float t = northSouthCount == 1 ? 0.5f : (float)i / (northSouthCount - 1);
            float x = Mathf.Lerp(-room.node.width / 2f + 1.5f, room.node.width / 2f - 1.5f, t);

            Vector3 northOffset = new Vector3(x, 0f, room.node.length / 2f - wallInset - inwardOffset);
            Vector3 southOffset = new Vector3(x, 0f, -room.node.length / 2f + wallInset + inwardOffset);

            Vector3 northPos = room.transform.position + room.transform.rotation * northOffset;
            Vector3 southPos = room.transform.position + room.transform.rotation * southOffset;

            northPos.y = torchWorldY;
            southPos.y = torchWorldY;

            TryPlaceGenericObjectAtHeight(
                room,
                torchPrefab,
                northPos,
                room.transform.rotation * Quaternion.Euler(0f, 180f, 0f),
                room.torchesRoot,
                0.05f,
                0.4f,
                0.1f
            );

            TryPlaceGenericObjectAtHeight(
                room,
                torchPrefab,
                southPos,
                room.transform.rotation,
                room.torchesRoot,
                0.05f,
                0.4f,
                0.1f
            );
        }

        for (int i = 0; i < eastWestCount; i++)
        {
            float t = eastWestCount == 1 ? 0.5f : (float)i / (eastWestCount - 1);
            float z = Mathf.Lerp(-room.node.length / 2f + 1.5f, room.node.length / 2f - 1.5f, t);

            Vector3 eastOffset = new Vector3(room.node.width / 2f - wallInset - inwardOffset, 0f, z);
            Vector3 westOffset = new Vector3(-room.node.width / 2f + wallInset + inwardOffset, 0f, z);

            Vector3 eastPos = room.transform.position + room.transform.rotation * eastOffset;
            Vector3 westPos = room.transform.position + room.transform.rotation * westOffset;

            eastPos.y = torchWorldY;
            westPos.y = torchWorldY;

            TryPlaceGenericObjectAtHeight(
                room,
                torchPrefab,
                eastPos,
                room.transform.rotation * Quaternion.Euler(0f, 270f, 0f),
                room.torchesRoot,
                0.05f,
                0.4f,
                0.1f
            );

            TryPlaceGenericObjectAtHeight(
                room,
                torchPrefab,
                westPos,
                room.transform.rotation * Quaternion.Euler(0f, 90f, 0f),
                room.torchesRoot,
                0.05f,
                0.4f,
                0.1f
            );
        }
    }

    void DeleteRandomGenericDecor(Room room)
    {
        if (room == null)
            return;

        room.EnsureDecorationRoots();

        float pillarDeleteChance = 0.15f;
        float torchDeleteChance = 0.10f;

        for (int i = room.pillarsRoot.childCount - 1; i >= 0; i--)
        {
            Transform child = room.pillarsRoot.GetChild(i);
            if (child == null)
                continue;

            if (Random.value < pillarDeleteChance)
            {
                RemoveOccupiedAreaNearPoint(room, child.position, 1.5f);
                Destroy(child.gameObject);
            }
        }

        for (int i = room.torchesRoot.childCount - 1; i >= 0; i--)
        {
            Transform child = room.torchesRoot.GetChild(i);
            if (child == null)
                continue;

            if (Random.value < torchDeleteChance)
            {
                RemoveOccupiedAreaNearPoint(room, child.position, 1.5f);
                Destroy(child.gameObject);
            }
        }
    }

    float GetRoomFloorTopY(Room room)
    {
        if (room == null)
            return 0f;

        Transform floor = room.transform.Find("Floor");
        if (floor != null)
        {
            Collider col = floor.GetComponent<Collider>();
            if (col != null)
                return col.bounds.max.y;
        }

        return room.transform.position.y + 1.5f;
    }

    GameObject TryPlaceGenericObjectAtHeight(Room room, GameObject prefab, Vector3 position, Quaternion rotation, Transform parent, float roomInset = 0.2f, float doorwayPadding = 0.25f, float occupiedPadding = 0.1f)
    {
        if (room == null || prefab == null)
            return null;

        GameObject instance = Instantiate(prefab, position, rotation, parent);
        TagDecorRecursive(instance);

        if (!TryGetCombinedColliderBounds(instance, out Bounds objectBounds))
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

        if (IntersectsOccupiedArea(room, objectBounds, occupiedPadding))
        {
            Destroy(instance);
            return null;
        }

        room.occupiedAreas.Add(objectBounds);
        ReservePlacedGenericInPlan(room, objectBounds, prefab.name);
        return instance;
    }

    bool TryGetCombinedColliderBounds(GameObject obj, out Bounds combinedBounds)
    {
        combinedBounds = new Bounds();
        Collider[] colliders = obj.GetComponentsInChildren<Collider>();

        bool found = false;

        for (int i = 0; i < colliders.Length; i++)
        {
            Collider col = colliders[i];
            if (col == null || !col.enabled)
                continue;

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
        Vector3 center = room.transform.position;

        float halfWidth = Mathf.FloorToInt(room.node.width) * 0.5f;
        float halfLength = Mathf.FloorToInt(room.node.length) * 0.5f;

        float minX = center.x - halfWidth + inset;
        float maxX = center.x + halfWidth - inset;
        float minZ = center.z - halfLength + inset;
        float maxZ = center.z + halfLength - inset;

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
            if (door == null)
                continue;

            Bounds doorBounds = new Bounds(
                door.position + Vector3.up * 1.5f,
                new Vector3(3f + padding, 3f, 3f + padding)
            );

            if (doorBounds.Intersects(objectBounds))
                return true;
        }

        return false;
    }

    bool IntersectsOccupiedArea(Room room, Bounds bounds, float padding)
    {
        if (room == null)
            return false;

        Bounds expanded = bounds;
        expanded.Expand(padding * 2f);

        for (int i = 0; i < room.occupiedAreas.Count; i++)
        {
            Bounds existing = room.occupiedAreas[i];
            if (existing.Intersects(expanded))
                return true;
        }

        return false;
    }

    void RemoveOccupiedAreaNearPoint(Room room, Vector3 point, float radius)
    {
        if (room == null)
            return;

        for (int i = room.occupiedAreas.Count - 1; i >= 0; i--)
        {
            Bounds bounds = room.occupiedAreas[i];
            Vector3 closest = bounds.ClosestPoint(point);

            if (Vector3.Distance(closest, point) <= radius)
                room.occupiedAreas.RemoveAt(i);
        }
    }

    void ClearExistingGenericDecor(Room room)
    {
        if (room == null)
            return;

        room.occupiedAreas.Clear();

        room.EnsureDecorationRoots();

        for (int i = room.pillarsRoot.childCount - 1; i >= 0; i--)
            Destroy(room.pillarsRoot.GetChild(i).gameObject);

        for (int i = room.torchesRoot.childCount - 1; i >= 0; i--)
            Destroy(room.torchesRoot.GetChild(i).gameObject);
    }

    void TagDecorRecursive(GameObject obj)
    {
        obj.tag = "Decor";

        for (int i = 0; i < obj.transform.childCount; i++)
            TagDecorRecursive(obj.transform.GetChild(i).gameObject);
    }

    public void FinalizeDecor()
    {
        for (int i = 0; i < allWorkableRooms.Count; i++)
        {
            Room room = allWorkableRooms[i];
            if (room != null)
                room.FinalizeDecorations();
        }
    }

    void ReservePlacedGenericInPlan(Room room, Bounds bounds, string source)
    {
        if (room == null)
            return;

        RoomDecorPlan plan = GetOrCreatePlan(room);
        if (plan == null || plan.grid == null)
            return;

        Vector2Int min = plan.grid.WorldToGrid(bounds.min);
        Vector2Int max = plan.grid.WorldToGrid(bounds.max);

        int startX = Mathf.Min(min.x, max.x);
        int endX = Mathf.Max(min.x, max.x);
        int startZ = Mathf.Min(min.y, max.y);
        int endZ = Mathf.Max(min.y, max.y);

        for (int x = startX; x <= endX; x++)
        {
            for (int z = startZ; z <= endZ; z++)
            {
                if (!plan.grid.IsInside(x, z))
                    continue;

                plan.grid.TryReserveCell(
                    x,
                    z,
                    DecorReservationPriority.Protected,
                    DecorReservationType.Blocked,
                    source
                );
            }
        }
    }

    public bool TryGetExitPosition(Room room, out Vector3 position)
    {
        position = Vector3.zero;

        if (room == null)
            return false;

        float clearanceRadius = 1.5f;
        float clearanceHeight = 2.5f;
        float roomInset = 2f;
        float doorwayPadding = 0.5f;

        float halfRoomWidth = room.node.width * 0.5f;
        float halfRoomLength = room.node.length * 0.5f;

        float minLocalX = -halfRoomWidth + clearanceRadius + roomInset;
        float maxLocalX =  halfRoomWidth - clearanceRadius - roomInset;
        float minLocalZ = -halfRoomLength + clearanceRadius + roomInset;
        float maxLocalZ =  halfRoomLength - clearanceRadius - roomInset;

        if (minLocalX > maxLocalX || minLocalZ > maxLocalZ)
            return false;

        List<Vector3> candidates = new List<Vector3>();

        candidates.Add(Vector3.zero);
        candidates.Add(new Vector3(-2f, 0f, -2f));
        candidates.Add(new Vector3(-2f, 0f,  2f));
        candidates.Add(new Vector3( 2f, 0f, -2f));
        candidates.Add(new Vector3( 2f, 0f,  2f));
        candidates.Add(new Vector3(-3f, 0f,  0f));
        candidates.Add(new Vector3( 3f, 0f,  0f));
        candidates.Add(new Vector3( 0f, 0f, -3f));
        candidates.Add(new Vector3( 0f, 0f,  3f));

        for (int i = 0; i < candidates.Count; i++)
        {
            Vector3 local = candidates[i];
            local.x = Mathf.Clamp(local.x, minLocalX, maxLocalX);
            local.z = Mathf.Clamp(local.z, minLocalZ, maxLocalZ);

            Vector3 world = room.transform.position + room.transform.rotation * local;
            world.y = 1.5f;

            Bounds testBounds = new Bounds(
                world + Vector3.up * (clearanceHeight * 0.5f),
                new Vector3(clearanceRadius * 2f, clearanceHeight, clearanceRadius * 2f)
            );

            if (!IsWithinRoomBounds(room, testBounds, 0.1f))
                continue;

            if (IntersectsDoorwayBounds(room, testBounds, doorwayPadding))
                continue;

            if (IntersectsOccupiedArea(room, testBounds, 0.2f))
                continue;

            room.occupiedAreas.Add(testBounds);
            position = world;
            return true;
        }
        return false;
    }
}