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
                    new RoomItem
                    {
                        name = "GenericTable",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Chair"),
                        priority = RoomItem.Priority.Primary,
                        gridWidth = 1,
                        gridLength = 1,
                        minCount = 1,
                        maxCount = 2,
                        allowRotation = true,
                        preferCenter = 2f,
                        avoidDoors = 4f,
                        itemTag = "Generic"
                    }
                }
            },
            {
                Room.RoomType.Tomb,
                new List<RoomItem>()
                {
                    new RoomItem
                    {
                        name = "Tomb",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Tomb"),
                        priority = RoomItem.Priority.Primary,
                        gridWidth = 2,
                        gridLength = 1,
                        minCount = 1,
                        maxCount = 2,
                        allowRotation = true,
                        preferWall = 4f,
                        avoidDoors = 5f,
                        avoidCenter = 1f,
                        itemTag = "Tomb"
                    },
                    new RoomItem
                    {
                        name = "Bones",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Bones"),
                        priority = RoomItem.Priority.Secondary,
                        gridWidth = 1,
                        gridLength = 1,
                        minCount = 1,
                        maxCount = 3,
                        allowRotation = true,
                        avoidDoors = 4f,
                        preferNearAnchor = 4f,
                        anchorTag = "Tomb",
                        itemTag = "Bones",
                        minAnchorDistance = 1,
                        maxAnchorDistance = 2
                    },
                    new RoomItem
                    {
                        name = "Cobwebs",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Cobwebs"),
                        priority = RoomItem.Priority.Tertiary,
                        gridWidth = 1,
                        gridLength = 1,
                        minCount = 1,
                        maxCount = 4,
                        allowRotation = true,
                        preferWall = 3f,
                        avoidDoors = 2f,
                        avoidCenter = 2f,
                        itemTag = "Cobwebs",
                        preferNearSameTag = 1f
                    }
                }
            },
            {
                Room.RoomType.Forge,
                new List<RoomItem>()
                {
                    new RoomItem
                    {
                        name = "Forge",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Forge"),
                        priority = RoomItem.Priority.Primary,
                        gridWidth = 2,
                        gridLength = 2,
                        minCount = 1,
                        maxCount = 1,
                        allowRotation = false,
                        preferWall = 5f,
                        avoidDoors = 6f,
                        avoidCenter = 3f,
                        itemTag = "Furnace"
                    },
                    new RoomItem
                    {
                        name = "Anvil",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Anvil"),
                        priority = RoomItem.Priority.Secondary,
                        gridWidth = 1,
                        gridLength = 1,
                        minCount = 1,
                        maxCount = 2,
                        allowRotation = true,
                        preferWall = 1f,
                        avoidDoors = 4f,
                        preferNearAnchor = 10f,
                        anchorTag = "Furnace",
                        itemTag = "Anvil",
                        minAnchorDistance = 1,
                        maxAnchorDistance = 2
                    },
                    new RoomItem
                    {
                        name = "ToolRack",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/ToolRack"),
                        priority = RoomItem.Priority.Tertiary,
                        gridWidth = 1,
                        gridLength = 1,
                        minCount = 1,
                        maxCount = 1,
                        allowRotation = true,
                        preferWall = 1f,
                        avoidDoors = 3f,
                        preferNearAnchor = 5f,
                        anchorTag = "Furnace",
                        itemTag = "ToolRack",
                        minAnchorDistance = 1,
                        maxAnchorDistance = 3
                    }
                }
            },
            {
                Room.RoomType.Library,
                new List<RoomItem>()
                {
                    new RoomItem
                    {
                        name = "Bookshelf",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Bookshelf"),
                        priority = RoomItem.Priority.Primary,
                        gridWidth = 1,
                        gridLength = 2,
                        minCount = 2,
                        maxCount = 4,
                        allowRotation = true,
                        preferWall = 5f,
                        avoidDoors = 5f,
                        avoidCenter = 3f,
                        itemTag = "Bookshelf",
                        preferNearSameTag = 2f
                    },
                    new RoomItem
                    {
                        name = "ReadingTable",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/ReadingTable"),
                        priority = RoomItem.Priority.Secondary,
                        gridWidth = 2,
                        gridLength = 2,
                        minCount = 1,
                        maxCount = 1,
                        allowRotation = true,
                        preferCenter = 4f,
                        avoidDoors = 4f,
                        itemTag = "ReadingTable"
                    },
                    new RoomItem
                    {
                        name = "Chair",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Chair"),
                        priority = RoomItem.Priority.Tertiary,
                        gridWidth = 1,
                        gridLength = 1,
                        minCount = 2,
                        maxCount = 4,
                        allowRotation = true,
                        preferNearAnchor = 5f,
                        anchorTag = "ReadingTable",
                        itemTag = "Chair",
                        minAnchorDistance = 1,
                        maxAnchorDistance = 2,
                        avoidDoors = 3f
                    }
                }
            },
            {
                Room.RoomType.Armory,
                new List<RoomItem>()
                {
                    new RoomItem
                    {
                        name = "WeaponRack",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/WeaponRack"),
                        priority = RoomItem.Priority.Primary,
                        gridWidth = 1,
                        gridLength = 2,
                        minCount = 2,
                        maxCount = 4,
                        allowRotation = true,
                        preferWall = 5f,
                        avoidDoors = 5f,
                        avoidCenter = 3f,
                        itemTag = "WeaponRack",
                        preferNearSameTag = 2f
                    },
                    new RoomItem
                    {
                        name = "ArmorStand",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/ArmorStand"),
                        priority = RoomItem.Priority.Secondary,
                        gridWidth = 1,
                        gridLength = 1,
                        minCount = 1,
                        maxCount = 3,
                        allowRotation = true,
                        preferWall = 3f,
                        avoidDoors = 4f,
                        preferNearAnchor = 4f,
                        anchorTag = "WeaponRack",
                        itemTag = "ArmorStand",
                        minAnchorDistance = 1,
                        maxAnchorDistance = 3
                    }
                }
            },
            {
                Room.RoomType.Barracks,
                new List<RoomItem>()
                {
                    new RoomItem
                    {
                        name = "Bed",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Bed"),
                        priority = RoomItem.Priority.Primary,
                        gridWidth = 2,
                        gridLength = 1,
                        minCount = 2,
                        maxCount = 5,
                        allowRotation = true,
                        preferWall = 5f,
                        avoidDoors = 4f,
                        avoidCenter = 3f,
                        itemTag = "Bed",
                        preferNearSameTag = 2f
                    },
                    new RoomItem
                    {
                        name = "Locker",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Locker"),
                        priority = RoomItem.Priority.Secondary,
                        gridWidth = 1,
                        gridLength = 1,
                        minCount = 1,
                        maxCount = 3,
                        allowRotation = true,
                        preferNearAnchor = 4f,
                        anchorTag = "Bed",
                        itemTag = "Locker",
                        minAnchorDistance = 1,
                        maxAnchorDistance = 2,
                        avoidDoors = 3f
                    },
                    new RoomItem
                    {
                        name = "Table",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Table"),
                        priority = RoomItem.Priority.Tertiary,
                        gridWidth = 2,
                        gridLength = 2,
                        minCount = 0,
                        maxCount = 1,
                        allowRotation = true,
                        preferCenter = 3f,
                        avoidDoors = 4f,
                        itemTag = "BarracksTable"
                    },
                    new RoomItem
                    {
                        name = "Chair",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Chair"),
                        priority = RoomItem.Priority.Tertiary,
                        gridWidth = 1,
                        gridLength = 1,
                        minCount = 0,
                        maxCount = 2,
                        allowRotation = true,
                        preferNearAnchor = 4f,
                        anchorTag = "BarracksTable",
                        itemTag = "Chair",
                        minAnchorDistance = 1,
                        maxAnchorDistance = 2,
                        avoidDoors = 3f
                    }
                }
            },
            {
                Room.RoomType.Altar,
                new List<RoomItem>()
                {
                    new RoomItem
                    {
                        name = "Altar",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Altar"),
                        priority = RoomItem.Priority.Primary,
                        gridWidth = 2,
                        gridLength = 2,
                        minCount = 1,
                        maxCount = 1,
                        allowRotation = true,
                        preferCenter = 5f,
                        avoidDoors = 5f,
                        itemTag = "Altar"
                    },
                    new RoomItem
                    {
                        name = "Candles",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Candles"),
                        priority = RoomItem.Priority.Secondary,
                        gridWidth = 1,
                        gridLength = 1,
                        minCount = 2,
                        maxCount = 4,
                        allowRotation = true,
                        preferNearAnchor = 6f,
                        anchorTag = "Altar",
                        itemTag = "Candles",
                        minAnchorDistance = 1,
                        maxAnchorDistance = 2,
                        avoidDoors = 3f
                    },
                    new RoomItem
                    {
                        name = "ReligiousStatue",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/ReligiousStatue"),
                        priority = RoomItem.Priority.Tertiary,
                        gridWidth = 1,
                        gridLength = 1,
                        minCount = 0,
                        maxCount = 2,
                        allowRotation = true,
                        preferWall = 3f,
                        avoidDoors = 4f,
                        itemTag = "Statue"
                    }
                }
            },
            {
                Room.RoomType.Treasury,
                new List<RoomItem>()
                {
                    new RoomItem
                    {
                        name = "TreasureChest",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/TreasureChest"),
                        priority = RoomItem.Priority.Primary,
                        gridWidth = 1,
                        gridLength = 1,
                        minCount = 1,
                        maxCount = 3,
                        allowRotation = true,
                        preferWall = 4f,
                        avoidDoors = 5f,
                        itemTag = "TreasureChest",
                        preferNearSameTag = 2f
                    },
                    new RoomItem
                    {
                        name = "GoldPile",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/GoldPile"),
                        priority = RoomItem.Priority.Secondary,
                        gridWidth = 1,
                        gridLength = 1,
                        minCount = 1,
                        maxCount = 4,
                        allowRotation = true,
                        preferNearAnchor = 5f,
                        anchorTag = "TreasureChest",
                        itemTag = "GoldPile",
                        minAnchorDistance = 1,
                        maxAnchorDistance = 2,
                        avoidDoors = 4f
                    },
                    new RoomItem
                    {
                        name = "GemDisplay",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/GemDisplay"),
                        priority = RoomItem.Priority.Tertiary,
                        gridWidth = 1,
                        gridLength = 1,
                        minCount = 0,
                        maxCount = 2,
                        allowRotation = true,
                        preferWall = 2f,
                        avoidDoors = 4f,
                        itemTag = "GemDisplay"
                    }
                }
            },
            {
                Room.RoomType.DiningHall,
                new List<RoomItem>()
                {
                    new RoomItem
                    {
                        name = "Table",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Table"),
                        priority = RoomItem.Priority.Primary,
                        gridWidth = 2,
                        gridLength = 3,
                        minCount = 1,
                        maxCount = 2,
                        allowRotation = true,
                        preferCenter = 5f,
                        avoidDoors = 5f,
                        itemTag = "DiningTable"
                    },
                    new RoomItem
                    {
                        name = "Chair",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Chair"),
                        priority = RoomItem.Priority.Secondary,
                        gridWidth = 1,
                        gridLength = 1,
                        minCount = 2,
                        maxCount = 6,
                        allowRotation = true,
                        preferNearAnchor = 6f,
                        anchorTag = "DiningTable",
                        itemTag = "Chair",
                        minAnchorDistance = 1,
                        maxAnchorDistance = 2,
                        avoidDoors = 3f
                    },
                    new RoomItem
                    {
                        name = "Chandelier",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Chandelier"),
                        priority = RoomItem.Priority.Tertiary,
                        gridWidth = 1,
                        gridLength = 1,
                        minCount = 0,
                        maxCount = 1,
                        allowRotation = false,
                        preferCenter = 3f,
                        avoidDoors = 3f,
                        itemTag = "Chandelier"
                    }
                }
            },
            {
                Room.RoomType.Warehouse,
                new List<RoomItem>()
                {
                    new RoomItem
                    {
                        name = "Crate",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Crate"),
                        priority = RoomItem.Priority.Primary,
                        gridWidth = 1,
                        gridLength = 1,
                        minCount = 3,
                        maxCount = 7,
                        allowRotation = true,
                        preferWall = 4f,
                        avoidDoors = 5f,
                        avoidCenter = 2f,
                        itemTag = "Crate",
                        preferNearSameTag = 3f
                    },
                    new RoomItem
                    {
                        name = "Barrel",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Barrel"),
                        priority = RoomItem.Priority.Secondary,
                        gridWidth = 1,
                        gridLength = 1,
                        minCount = 2,
                        maxCount = 5,
                        allowRotation = true,
                        preferWall = 3f,
                        avoidDoors = 4f,
                        preferNearAnchor = 4f,
                        anchorTag = "Crate",
                        itemTag = "Barrel",
                        minAnchorDistance = 1,
                        maxAnchorDistance = 3
                    },
                    new RoomItem
                    {
                        name = "Shelf",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Shelf"),
                        priority = RoomItem.Priority.Tertiary,
                        gridWidth = 1,
                        gridLength = 2,
                        minCount = 1,
                        maxCount = 3,
                        allowRotation = true,
                        preferWall = 5f,
                        avoidDoors = 5f,
                        itemTag = "Shelf"
                    }
                }
            },
            {
                Room.RoomType.ShopKeeper,
                new List<RoomItem>()
                {
                    new RoomItem
                    {
                        name = "Counter",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Counter"),
                        priority = RoomItem.Priority.Primary,
                        gridWidth = 2,
                        gridLength = 1,
                        minCount = 1,
                        maxCount = 1,
                        allowRotation = true,
                        preferWall = 4f,
                        avoidDoors = 5f,
                        itemTag = "Counter"
                    },
                    new RoomItem
                    {
                        name = "DisplayCase",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/DisplayCase"),
                        priority = RoomItem.Priority.Secondary,
                        gridWidth = 1,
                        gridLength = 1,
                        minCount = 1,
                        maxCount = 3,
                        allowRotation = true,
                        preferNearAnchor = 5f,
                        anchorTag = "Counter",
                        itemTag = "DisplayCase",
                        minAnchorDistance = 1,
                        maxAnchorDistance = 2,
                        avoidDoors = 4f
                    }
                }
            },
            {
                Room.RoomType.Kitchen,
                new List<RoomItem>()
                {
                    new RoomItem
                    {
                        name = "Stove",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Stove"),
                        priority = RoomItem.Priority.Primary,
                        gridWidth = 2,
                        gridLength = 1,
                        minCount = 1,
                        maxCount = 2,
                        allowRotation = true,
                        preferWall = 5f,
                        avoidDoors = 5f,
                        itemTag = "Stove"
                    },
                    new RoomItem
                    {
                        name = "Table",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Table"),
                        priority = RoomItem.Priority.Secondary,
                        gridWidth = 2,
                        gridLength = 2,
                        minCount = 1,
                        maxCount = 1,
                        allowRotation = true,
                        preferCenter = 4f,
                        avoidDoors = 4f,
                        itemTag = "KitchenTable"
                    },
                    new RoomItem
                    {
                        name = "Chair",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Chair"),
                        priority = RoomItem.Priority.Tertiary,
                        gridWidth = 1,
                        gridLength = 1,
                        minCount = 1,
                        maxCount = 4,
                        allowRotation = true,
                        preferNearAnchor = 4f,
                        anchorTag = "KitchenTable",
                        itemTag = "Chair",
                        minAnchorDistance = 1,
                        maxAnchorDistance = 2,
                        avoidDoors = 3f
                    }
                }
            },
            {
                Room.RoomType.Tavern,
                new List<RoomItem>()
                {
                    new RoomItem
                    {
                        name = "BarCounter",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/BarCounter"),
                        priority = RoomItem.Priority.Primary,
                        gridWidth = 3,
                        gridLength = 1,
                        minCount = 1,
                        maxCount = 1,
                        allowRotation = true,
                        preferWall = 5f,
                        avoidDoors = 5f,
                        itemTag = "BarCounter"
                    },
                    new RoomItem
                    {
                        name = "Stool",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Stool"),
                        priority = RoomItem.Priority.Secondary,
                        gridWidth = 1,
                        gridLength = 1,
                        minCount = 2,
                        maxCount = 5,
                        allowRotation = true,
                        preferNearAnchor = 6f,
                        anchorTag = "BarCounter",
                        itemTag = "Stool",
                        minAnchorDistance = 1,
                        maxAnchorDistance = 2,
                        avoidDoors = 3f
                    },
                    new RoomItem
                    {
                        name = "Keg",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Keg"),
                        priority = RoomItem.Priority.Tertiary,
                        gridWidth = 1,
                        gridLength = 1,
                        minCount = 1,
                        maxCount = 3,
                        allowRotation = true,
                        preferWall = 3f,
                        avoidDoors = 4f,
                        preferNearSameTag = 2f,
                        itemTag = "Keg"
                    }
                }
            },
            {
                Room.RoomType.Prison,
                new List<RoomItem>()
                {
                    new RoomItem
                    {
                        name = "Cell",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Cell"),
                        priority = RoomItem.Priority.Primary,
                        gridWidth = 2,
                        gridLength = 2,
                        minCount = 1,
                        maxCount = 2,
                        allowRotation = true,
                        preferWall = 5f,
                        avoidDoors = 5f,
                        avoidCenter = 3f,
                        itemTag = "Cell"
                    },
                    new RoomItem
                    {
                        name = "GuardPost",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/GuardPost"),
                        priority = RoomItem.Priority.Secondary,
                        gridWidth = 1,
                        gridLength = 1,
                        minCount = 1,
                        maxCount = 2,
                        allowRotation = true,
                        preferNearAnchor = 4f,
                        anchorTag = "Cell",
                        itemTag = "GuardPost",
                        minAnchorDistance = 1,
                        maxAnchorDistance = 3,
                        avoidDoors = 4f
                    },
                    new RoomItem
                    {
                        name = "Crates",
                        prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Crates"),
                        priority = RoomItem.Priority.Tertiary,
                        gridWidth = 1,
                        gridLength = 1,
                        minCount = 1,
                        maxCount = 3,
                        allowRotation = true,
                        preferWall = 3f,
                        avoidDoors = 4f,
                        preferNearSameTag = 2f,
                        itemTag = "PrisonCrates"
                    }
                }
            }
        };

        ValidateDatabase();
    }

    void ValidateDatabase()
    {
        foreach (var pair in roomItems)
        {
            foreach (RoomItem item in pair.Value)
            {
                if (item.prefab == null)
                {
                    Debug.LogWarning($"[RoomItemDatabase] Missing prefab for {pair.Key} -> {item.name}");
                }

                if (item.gridWidth <= 0) item.gridWidth = 1;
                if (item.gridLength <= 0) item.gridLength = 1;
                if (item.maxCount < item.minCount) item.maxCount = item.minCount;
            }
        }
    }
}