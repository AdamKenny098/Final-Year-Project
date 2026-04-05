using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SingleRoomDatabaseBuilder : EditorWindow
{
    const string DefinitionsRoot = "Assets/Resources/RoomItems/Definitions";
    const string RoomSetsRoot = "Assets/Resources/RoomItems/RoomSets";
    const string TargetRoomFolder = "Assets/Resources/RoomItems/Definitions/Default";
    const string TargetSetPath = "Assets/Resources/RoomItems/RoomSets/Default_Set.asset";

    bool overwriteExistingDefinitions = true;
    bool clearRoomSetBeforeBuild = true;

    [MenuItem("Tools/Dungeon/Build Single Room Database")]
    static void Open()
    {
        GetWindow<SingleRoomDatabaseBuilder>("Single Room DB");
    }

    void OnGUI()
    {
        GUILayout.Label("Build Single Room Database", EditorStyles.boldLabel);

        EditorGUILayout.HelpBox(
            "Creates a single Default room set using the selected kept prefabs only.",
            MessageType.Info);

        overwriteExistingDefinitions = EditorGUILayout.Toggle("Overwrite Existing Definitions", overwriteExistingDefinitions);
        clearRoomSetBeforeBuild = EditorGUILayout.Toggle("Clear Room Set Before Build", clearRoomSetBeforeBuild);

        EditorGUILayout.Space();

        if (GUILayout.Button("Build"))
            Build();
    }

    void Build()
    {
        EnsureFolder("Assets/Resources");
        EnsureFolder("Assets/Resources/RoomItems");
        EnsureFolder(DefinitionsRoot);
        EnsureFolder(RoomSetsRoot);
        EnsureFolder(TargetRoomFolder);

        RoomTypeSet set = AssetDatabase.LoadAssetAtPath<RoomTypeSet>(TargetSetPath);

        if (set == null)
        {
            set = CreateInstance<RoomTypeSet>();
            set.roomType = Room.RoomType.Default;
            set.items = new List<RoomItemDefinition>();
            AssetDatabase.CreateAsset(set, TargetSetPath);
        }

        if (set.items == null || clearRoomSetBeforeBuild)
            set.items = new List<RoomItemDefinition>();

        List<SeedItem> seeds = BuildSeeds();

        int created = 0;
        int updated = 0;
        int addedToSet = 0;

        for (int i = 0; i < seeds.Count; i++)
        {
            SeedItem seed = seeds[i];
            string safeName = MakeSafeFileName(seed.itemName);
            string assetPath = $"{TargetRoomFolder}/{safeName}.asset";

            RoomItemDefinition def = AssetDatabase.LoadAssetAtPath<RoomItemDefinition>(assetPath);

            if (def == null)
            {
                def = CreateInstance<RoomItemDefinition>();
                ApplySeed(def, seed);
                AssetDatabase.CreateAsset(def, assetPath);
                created++;
            }
            else if (overwriteExistingDefinitions)
            {
                ApplySeed(def, seed);
                EditorUtility.SetDirty(def);
                updated++;
            }

            if (def != null && !set.items.Contains(def))
            {
                set.items.Add(def);
                addedToSet++;
            }
        }

        EditorUtility.SetDirty(set);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[SingleRoomDatabaseBuilder] Done. Created: {created}, Updated: {updated}, Added to set: {addedToSet}");
    }

    void ApplySeed(RoomItemDefinition def, SeedItem seed)
    {
        def.itemName = seed.itemName;
        def.itemTag = seed.itemTag;
        def.anchorTag = seed.anchorTag;
        def.priority = seed.priority;
        def.gridWidth = Mathf.Max(1, seed.gridWidth);
        def.gridLength = Mathf.Max(1, seed.gridLength);
        def.allowRotation = seed.allowRotation;
        def.minCount = Mathf.Max(0, seed.minCount);
        def.maxCount = seed.priority == RoomItem.Priority.Primary
            ? Mathf.Max(def.minCount, seed.maxCount)
            : 999;

        def.preferWall = seed.preferWall;
        def.preferCenter = seed.preferCenter;
        def.avoidCenter = seed.avoidCenter;
        def.avoidDoors = seed.avoidDoors;
        def.preferNearAnchor = seed.preferNearAnchor;
        def.preferNearSameTag = seed.preferNearSameTag;
        def.minAnchorDistance = Mathf.Max(0, seed.minAnchorDistance);
        def.maxAnchorDistance = Mathf.Max(def.minAnchorDistance, seed.maxAnchorDistance);

        def.prefab = string.IsNullOrWhiteSpace(seed.prefabPath)
            ? null
            : Resources.Load<GameObject>(seed.prefabPath);

        if (def.prefab == null && !string.IsNullOrWhiteSpace(seed.prefabPath))
            Debug.LogWarning($"[SingleRoomDatabaseBuilder] Missing prefab at Resources path: {seed.prefabPath} for {seed.itemName}");
    }

    List<SeedItem> BuildSeeds()
    {
        return new List<SeedItem>()
        {
            Item(
                "Bed",
                "Prefabs/RoomItems/Barracks/Bed",
                RoomItem.Priority.Primary,
                2, 1, 1, 3, true,
                preferWall: 5f,
                avoidDoors: 4f,
                avoidCenter: 2f,
                itemTag: "Bed",
                preferNearSameTag: 1f
            ),
            Item(
                "WeaponRack",
                "Prefabs/RoomItems/Armory/WeaponRack",
                RoomItem.Priority.Primary,
                1, 2, 1, 3, true,
                preferWall: 5f,
                avoidDoors: 5f,
                avoidCenter: 2f,
                itemTag: "WeaponRack",
                preferNearSameTag: 1f
            ),
            Item(
                "ShieldRack",
                "Prefabs/RoomItems/Armory/ShieldRack",
                RoomItem.Priority.Primary,
                1, 1, 1, 2, true,
                preferWall: 4f,
                avoidDoors: 4f,
                itemTag: "ShieldRack"
            ),
            Item(
                "WeaponTable",
                "Prefabs/RoomItems/Armory/WeaponTable",
                RoomItem.Priority.Primary,
                2, 1, 1, 2, true,
                preferCenter: 2f,
                avoidDoors: 4f,
                itemTag: "WeaponTable"
            ),
            Item(
                "Crate",
                "Prefabs/RoomItems/Warehouse/Crate",
                RoomItem.Priority.Primary,
                1, 1, 2, 5, true,
                preferWall: 4f,
                avoidDoors: 4f,
                avoidCenter: 1f,
                itemTag: "Crate",
                preferNearSameTag: 2f
            ),

            Item(
                "Footlocker",
                "Prefabs/RoomItems/Barracks/Footlocker",
                RoomItem.Priority.Secondary,
                1, 1, 2, 5, true,
                preferNearAnchor: 4f,
                anchorTag: "Bed",
                itemTag: "Footlocker",
                minAnchorDistance: 1,
                maxAnchorDistance: 2,
                avoidDoors: 3f
            ),
            Item(
                "WeaponStand",
                "Prefabs/RoomItems/Barracks/WeaponStand",
                RoomItem.Priority.Secondary,
                1, 1, 1, 3, true,
                preferWall: 3f,
                avoidDoors: 4f,
                itemTag: "WeaponStand"
            ),
            Item(
                "Barrel",
                "Prefabs/RoomItems/Warehouse/Barrel",
                RoomItem.Priority.Secondary,
                1, 1, 2, 5, true,
                preferNearAnchor: 3f,
                anchorTag: "Crate",
                itemTag: "Barrel",
                minAnchorDistance: 1,
                maxAnchorDistance: 3,
                avoidDoors: 4f
            ),
            Item(
                "ShieldPile",
                "Prefabs/RoomItems/Armory/ShieldPile",
                RoomItem.Priority.Secondary,
                1, 1, 2, 5, true,
                preferWall: 3f,
                avoidDoors: 3f,
                itemTag: "ShieldPile"
            ),
            Item(
                "WeaponCrate",
                "Prefabs/RoomItems/Armory/WeaponCrate",
                RoomItem.Priority.Secondary,
                1, 1, 2, 5, true,
                preferWall: 3f,
                avoidDoors: 4f,
                itemTag: "WeaponCrate"
            ),
            Item(
                "GenericTable",
                "Prefabs/RoomItems/Default/GenericTable",
                RoomItem.Priority.Secondary,
                1, 1, 1, 3, true,
                preferCenter: 2f,
                avoidDoors: 4f,
                itemTag: "GenericTable"
            ),
            Item(
                "Shelf",
                "Prefabs/RoomItems/Generic/Shelf",
                RoomItem.Priority.Secondary,
                1, 2, 2, 4, true,
                preferWall: 5f,
                avoidDoors: 4f,
                itemTag: "Shelf"
            ),

            Item(
                "Chair",
                "Prefabs/RoomItems/Generic/Chair",
                RoomItem.Priority.Tertiary,
                1, 1, 3, 8, true,
                preferNearAnchor: 3f,
                anchorTag: "GenericTable",
                itemTag: "Chair",
                minAnchorDistance: 1,
                maxAnchorDistance: 2,
                avoidDoors: 3f
            ),
            Item(
                "BrokenFurniture",
                "Prefabs/RoomItems/Default/BrokenFurniture",
                RoomItem.Priority.Tertiary,
                1, 1, 3, 8, true,
                preferWall: 2f,
                avoidDoors: 3f,
                avoidCenter: 1f,
                itemTag: "BrokenFurniture"
            ),
            Item(
                "CrateSmall",
                "Prefabs/RoomItems/Default/CrateSmall",
                RoomItem.Priority.Tertiary,
                1, 1, 3, 8, true,
                preferWall: 2f,
                avoidDoors: 3f,
                itemTag: "CrateSmall"
            ),
            Item(
                "Debris",
                "Prefabs/RoomItems/Default/Debris",
                RoomItem.Priority.Tertiary,
                1, 1, 4, 10, true,
                avoidDoors: 2f,
                avoidCenter: 1f,
                itemTag: "Debris"
            ),
            Item(
                "TorchStand",
                "Prefabs/RoomItems/Default/TorchStand",
                RoomItem.Priority.Tertiary,
                1, 1, 2, 6, true,
                preferWall: 3f,
                avoidDoors: 3f,
                itemTag: "TorchStand"
            )
        };
    }

    static SeedItem Item(
        string itemName,
        string prefabPath,
        RoomItem.Priority priority,
        int gridWidth,
        int gridLength,
        int minCount,
        int maxCount,
        bool allowRotation,
        float preferWall = 0f,
        float preferCenter = 0f,
        float avoidCenter = 0f,
        float avoidDoors = 0f,
        float preferNearAnchor = 0f,
        float preferNearSameTag = 0f,
        string itemTag = "",
        string anchorTag = "",
        int minAnchorDistance = 1,
        int maxAnchorDistance = 4)
    {
        return new SeedItem
        {
            itemName = itemName,
            prefabPath = prefabPath,
            priority = priority,
            gridWidth = gridWidth,
            gridLength = gridLength,
            minCount = minCount,
            maxCount = maxCount,
            allowRotation = allowRotation,
            preferWall = preferWall,
            preferCenter = preferCenter,
            avoidCenter = avoidCenter,
            avoidDoors = avoidDoors,
            preferNearAnchor = preferNearAnchor,
            preferNearSameTag = preferNearSameTag,
            itemTag = itemTag,
            anchorTag = anchorTag,
            minAnchorDistance = minAnchorDistance,
            maxAnchorDistance = maxAnchorDistance
        };
    }

    void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
            return;

        string parent = Path.GetDirectoryName(path)?.Replace("\\", "/");
        string folder = Path.GetFileName(path);

        if (!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent))
            EnsureFolder(parent);

        if (!string.IsNullOrEmpty(parent) && !string.IsNullOrEmpty(folder))
            AssetDatabase.CreateFolder(parent, folder);
    }

    string MakeSafeFileName(string value)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
            value = value.Replace(c.ToString(), "");

        value = value.Replace("/", "_");
        value = value.Replace("\\", "_");
        value = value.Replace(":", "_");

        return string.IsNullOrWhiteSpace(value) ? "RoomItem" : value;
    }

    class SeedItem
    {
        public string itemName;
        public string prefabPath;
        public RoomItem.Priority priority;
        public int gridWidth;
        public int gridLength;
        public int minCount;
        public int maxCount;
        public bool allowRotation;

        public float preferWall;
        public float preferCenter;
        public float avoidCenter;
        public float avoidDoors;
        public float preferNearAnchor;
        public float preferNearSameTag;

        public string itemTag;
        public string anchorTag;
        public int minAnchorDistance;
        public int maxAnchorDistance;
    }
}