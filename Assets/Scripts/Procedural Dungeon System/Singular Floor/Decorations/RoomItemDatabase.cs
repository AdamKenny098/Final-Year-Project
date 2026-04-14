using System.Collections.Generic;
using UnityEngine;

public class RoomItemDatabase : MonoBehaviour
{
    [Header("Resources")]
    [SerializeField] string roomSetPath = "RoomItems/RoomSets/Default_Set";

    [Header("Behavior")]
    [SerializeField] bool useDefaultForAllRooms = true;

    public List<RoomItem> emptyItems = new List<RoomItem>();

    public Dictionary<Room.RoomType, List<RoomItem>> roomItems = new Dictionary<Room.RoomType, List<RoomItem>>();

    void Awake()
    {
        BuildDatabase();
        ValidateDatabase();
    }

    void BuildDatabase()
    {
        roomItems.Clear();

        RoomTypeSet set = Resources.Load<RoomTypeSet>(roomSetPath);
        if (set == null)
        {
            Debug.LogError($"[RoomItemDatabase] Failed to load RoomTypeSet at Resources path: {roomSetPath}");
            return;
        }

        List<RoomItem> runtimeItems = new List<RoomItem>();

        if (set.items != null)
        {
            for (int i = 0; i < set.items.Count; i++)
            {
                RoomItemDefinition def = set.items[i];
                if (def == null)
                    continue;

                runtimeItems.Add(def.ToRuntimeItem());
            }
        }

        roomItems[Room.RoomType.Default] = runtimeItems;
    }

    void ValidateDatabase()
    {
        foreach (KeyValuePair<Room.RoomType, List<RoomItem>> pair in roomItems) // KeyValuePair is like a single entry in the dictionary, with pair.Key and pair.Value
        {
            List<RoomItem> items = pair.Value;
            if (items == null)
                continue;

            for (int i = 0; i < items.Count; i++)
            {
                RoomItem item = items[i];
                if (item == null)
                    continue;

                if (item.prefab == null)
                    Debug.LogWarning($"[RoomItemDatabase] Missing prefab for {pair.Key} -> {item.name}");

                if (item.gridWidth <= 0)
                    item.gridWidth = 1;

                if (item.gridLength <= 0)
                    item.gridLength = 1;

                if (item.maxCount < item.minCount)
                    item.maxCount = item.minCount;
            }
        }
    }

    public List<RoomItem> GetItemsForRoom(Room.RoomType roomType)
    {
        if (roomItems.TryGetValue(roomType, out List<RoomItem> exactItems))
            return exactItems;

        if (useDefaultForAllRooms && roomItems.TryGetValue(Room.RoomType.Default, out List<RoomItem> defaultItems))
            return defaultItems;

        return emptyItems;
    }

    public bool HasItemsForRoom(Room.RoomType roomType)
    {
        if (roomItems.ContainsKey(roomType))
            return true;

        return useDefaultForAllRooms && roomItems.ContainsKey(Room.RoomType.Default);
    }

    public void Rebuild()
    {
        BuildDatabase();
        ValidateDatabase();
    }
}