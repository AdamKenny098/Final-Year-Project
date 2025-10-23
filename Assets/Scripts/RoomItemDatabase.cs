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
                Room.RoomType.Tomb,
                new List<RoomItem>()
                {
                    new RoomItem() { name = "Tomb", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Tomb"), priority = RoomItem.Priority.Primary },
                    new RoomItem() { name = "Bones", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Bones"), priority = RoomItem.Priority.Secondary },
                    new RoomItem() { name = "Cobwebs", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Cobwebs"), priority = RoomItem.Priority.Tertiary }
                }
            },
            {
                Room.RoomType.Forge,
                new List<RoomItem>()
                {
                    new RoomItem() { name = "Anvil", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Anvil"), priority = RoomItem.Priority.Primary },
                    new RoomItem() { name = "Forge", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Forge"), priority = RoomItem.Priority.Primary },
                    new RoomItem() { name = "ToolRack", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/ToolRack"), priority = RoomItem.Priority.Secondary }
                }
            },
            {
                Room.RoomType.Library,
                new List<RoomItem>()
                {
                    new RoomItem() { name = "Bookshelf", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Bookshelf"), priority = RoomItem.Priority.Primary },
                    new RoomItem() { name = "ReadingTable", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/ReadingTable"), priority = RoomItem.Priority.Secondary },
                    new RoomItem() { name = "Chair", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Chair"), priority = RoomItem.Priority.Tertiary }
                }
            },
            {
                Room.RoomType.Armory,
                new List<RoomItem>()
                {
                    new RoomItem() { name = "WeaponRack", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/WeaponRack"), priority = RoomItem.Priority.Primary },
                    new RoomItem() { name = "ArmorStand", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/ArmorStand"), priority = RoomItem.Priority.Secondary }
                }
            },
            {
                Room.RoomType.Barracks,
                new List<RoomItem>()
                {
                    new RoomItem() { name = "Bed", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Bed"), priority = RoomItem.Priority.Primary },
                    new RoomItem() { name = "Locker", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Locker"), priority = RoomItem.Priority.Secondary },
                    new RoomItem() { name = "Table", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Table"), priority = RoomItem.Priority.Tertiary },
                    new RoomItem() { name = "Chair", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Chair"), priority = RoomItem.Priority.Tertiary }
                }
            },
            {
                Room.RoomType.Altar,
                new List<RoomItem>()
                {
                    new RoomItem() { name = "Altar", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Altar"), priority = RoomItem.Priority.Primary },
                    new RoomItem() { name = "Candles", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Candles"), priority = RoomItem.Priority.Secondary },
                    new RoomItem() { name = "ReligiousStatue", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/ReligiousStatue"), priority = RoomItem.Priority.Tertiary }
                }
            },
            {
                Room.RoomType.Treasury,
                new List<RoomItem>()
                {
                    new RoomItem() { name = "TreasureChest", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/TreasureChest"), priority = RoomItem.Priority.Primary },
                    new RoomItem() { name = "GoldPile", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/GoldPile"), priority = RoomItem.Priority.Secondary },
                    new RoomItem() { name = "GemDisplay", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/GemDisplay"), priority = RoomItem.Priority.Tertiary }
                }
            },
            {
                Room.RoomType.DiningHall,
                new List<RoomItem>()
                {
                    new RoomItem() { name = "Table", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Table"), priority = RoomItem.Priority.Primary },
                    new RoomItem() { name = "Chair", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Chair"), priority = RoomItem.Priority.Secondary },
                    new RoomItem() { name = "Chandelier", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Chandelier"), priority = RoomItem.Priority.Tertiary }
                }
            },
            {
                Room.RoomType.Warehouse,
                new List<RoomItem>()
                {
                    new RoomItem() { name = "Crate", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Crate"), priority = RoomItem.Priority.Primary },
                    new RoomItem() { name = "Barrel", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Barrel"), priority = RoomItem.Priority.Secondary },
                    new RoomItem() { name = "Shelf", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Shelf"), priority = RoomItem.Priority.Tertiary }
                }
            },
            {
                Room.RoomType.ShopKeeper,
                new List<RoomItem>()
                {
                    new RoomItem() { name = "Counter", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Counter"), priority = RoomItem.Priority.Primary },
                    new RoomItem() { name = "DisplayCase", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/DisplayCase"), priority = RoomItem.Priority.Secondary }
                }
            },
            {
                Room.RoomType.Kitchen,
                new List<RoomItem>()
                {
                    new RoomItem() { name = "Stove", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Stove"), priority = RoomItem.Priority.Primary },
                    new RoomItem() { name = "Table", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Table"), priority = RoomItem.Priority.Secondary },
                    new RoomItem() { name = "Chair", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Chair"), priority = RoomItem.Priority.Tertiary }
                }
            },
            {
                Room.RoomType.Tavern,
                new List<RoomItem>()
                {
                    new RoomItem() { name = "BarCounter", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/BarCounter"), priority = RoomItem.Priority.Primary },
                    new RoomItem() { name = "Stool", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Stool"), priority = RoomItem.Priority.Secondary },
                    new RoomItem() { name = "Keg", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Keg"), priority = RoomItem.Priority.Tertiary }
                }
            },
            {
                Room.RoomType.Prison,
                new List<RoomItem>()
                {
                    new RoomItem() { name = "Cell", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Cell"), priority = RoomItem.Priority.Primary },
                    new RoomItem() { name = "GuardPost", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/GuardPost"), priority = RoomItem.Priority.Secondary },
                    new RoomItem() { name = "Crates", prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Crates"), priority = RoomItem.Priority.Tertiary }
                }
            }

        };
    }
}
