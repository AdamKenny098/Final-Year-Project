using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomItemDatabase : MonoBehaviour
{
    public Dictionary<Room.RoomType, List<RoomItem>> roomItems;

    void Awake()
    {
        roomItems = new Dictionary<Room.RoomType, List<RoomItem>>()
        {
            {
                Room.RoomType.Default,
                new List<RoomItem>()
                {
                    MakeItem("GenericTable", "Prefabs/RoomItems/Chair", RoomItem.Priority.Primary)
                }
            },
            {
                Room.RoomType.Tomb,
                new List<RoomItem>()
                {
                    MakeItem("Tomb", "Prefabs/RoomItems/Tomb", RoomItem.Priority.Primary),
                    MakeItem("Bones", "Prefabs/RoomItems/Bones", RoomItem.Priority.Secondary),
                    MakeItem("Cobwebs", "Prefabs/RoomItems/Cobwebs", RoomItem.Priority.Tertiary)
                }
            },
            {
                Room.RoomType.Forge,
                new List<RoomItem>()
                {
                    MakeItem("Anvil", "Prefabs/RoomItems/Anvil", RoomItem.Priority.Primary),
                    MakeItem("Forge", "Prefabs/RoomItems/Forge", RoomItem.Priority.Primary),
                    MakeItem("ToolRack", "Prefabs/RoomItems/ToolRack", RoomItem.Priority.Secondary)
                }
            },
            {
                Room.RoomType.Library,
                new List<RoomItem>()
                {
                    MakeItem("Bookshelf", "Prefabs/RoomItems/Bookshelf", RoomItem.Priority.Primary),
                    MakeItem("ReadingTable", "Prefabs/RoomItems/ReadingTable", RoomItem.Priority.Secondary),
                    MakeItem("Chair", "Prefabs/RoomItems/Chair", RoomItem.Priority.Tertiary)
                }
            },
            {
                Room.RoomType.Armory,
                new List<RoomItem>()
                {
                    MakeItem("WeaponRack", "Prefabs/RoomItems/WeaponRack", RoomItem.Priority.Primary),
                    MakeItem("ArmorStand", "Prefabs/RoomItems/ArmorStand", RoomItem.Priority.Secondary)
                }
            },
            {
                Room.RoomType.Barracks,
                new List<RoomItem>()
                {
                    MakeItem("Bed", "Prefabs/RoomItems/Bed", RoomItem.Priority.Primary),
                    MakeItem("Locker", "Prefabs/RoomItems/Locker", RoomItem.Priority.Secondary),
                    MakeItem("Table", "Prefabs/RoomItems/Table", RoomItem.Priority.Tertiary),
                    MakeItem("Chair", "Prefabs/RoomItems/Chair", RoomItem.Priority.Tertiary)
                }
            },
            {
                Room.RoomType.Altar,
                new List<RoomItem>()
                {
                    MakeItem("Altar", "Prefabs/RoomItems/Altar", RoomItem.Priority.Primary),
                    MakeItem("Candles", "Prefabs/RoomItems/Candles", RoomItem.Priority.Secondary),
                    MakeItem("ReligiousStatue", "Prefabs/RoomItems/ReligiousStatue", RoomItem.Priority.Tertiary)
                }
            },
            {
                Room.RoomType.Treasury,
                new List<RoomItem>()
                {
                    MakeItem("TreasureChest", "Prefabs/RoomItems/TreasureChest", RoomItem.Priority.Primary),
                    MakeItem("GoldPile", "Prefabs/RoomItems/GoldPile", RoomItem.Priority.Secondary),
                    MakeItem("GemDisplay", "Prefabs/RoomItems/GemDisplay", RoomItem.Priority.Tertiary)
                }
            },
            {
                Room.RoomType.DiningHall,
                new List<RoomItem>()
                {
                    MakeItem("Table", "Prefabs/RoomItems/Table", RoomItem.Priority.Primary),
                    MakeItem("Chair", "Prefabs/RoomItems/Chair", RoomItem.Priority.Secondary),
                    MakeItem("Chandelier", "Prefabs/RoomItems/Chandelier", RoomItem.Priority.Tertiary)
                }
            },
            {
                Room.RoomType.Warehouse,
                new List<RoomItem>()
                {
                    MakeItem("Crate", "Prefabs/RoomItems/Crate", RoomItem.Priority.Primary),
                    MakeItem("Barrel", "Prefabs/RoomItems/Barrel", RoomItem.Priority.Secondary),
                    MakeItem("Shelf", "Prefabs/RoomItems/Shelf", RoomItem.Priority.Tertiary)
                }
            },
            {
                Room.RoomType.ShopKeeper,
                new List<RoomItem>()
                {
                    MakeItem("Counter", "Prefabs/RoomItems/Counter", RoomItem.Priority.Primary),
                    MakeItem("DisplayCase", "Prefabs/RoomItems/DisplayCase", RoomItem.Priority.Secondary)
                }
            },
            {
                Room.RoomType.Kitchen,
                new List<RoomItem>()
                {
                    MakeItem("Stove", "Prefabs/RoomItems/Stove", RoomItem.Priority.Primary),
                    MakeItem("Table", "Prefabs/RoomItems/Table", RoomItem.Priority.Secondary),
                    MakeItem("Chair", "Prefabs/RoomItems/Chair", RoomItem.Priority.Tertiary)
                }
            },
            {
                Room.RoomType.Tavern,
                new List<RoomItem>()
                {
                    MakeItem("BarCounter", "Prefabs/RoomItems/BarCounter", RoomItem.Priority.Primary),
                    MakeItem("Stool", "Prefabs/RoomItems/Stool", RoomItem.Priority.Secondary),
                    MakeItem("Keg", "Prefabs/RoomItems/Keg", RoomItem.Priority.Tertiary)
                }
            },
            {
                Room.RoomType.Prison,
                new List<RoomItem>()
                {
                    MakeItem("Cell", "Prefabs/RoomItems/Cell", RoomItem.Priority.Primary),
                    MakeItem("GuardPost", "Prefabs/RoomItems/GuardPost", RoomItem.Priority.Secondary),
                    MakeItem("Crates", "Prefabs/RoomItems/Crates", RoomItem.Priority.Tertiary)
                }
            }
        };
    }

    private RoomItem MakeItem(string name, string resourcePath, RoomItem.Priority priority)
    {
        GameObject prefab = Resources.Load<GameObject>(resourcePath);
        float area = 1f;
        if (prefab != null)
        {
            BoxCollider col = prefab.GetComponent<BoxCollider>();
            if (col != null)
                area = col.size.x * col.size.z;
        }
        else
        {
            Debug.LogWarning($"[RoomItemDatabase] Missing prefab at path: {resourcePath}");
        }

        return new RoomItem
        {
            name = name,
            prefab = prefab,
            priority = priority,
            areaOccupied = area
        };
    }
}
