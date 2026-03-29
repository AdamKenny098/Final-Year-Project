using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class QuestGeneratorWindow : EditorWindow
{
    [Header("Folders")]
    string questFolder = "Assets/Scriptable Objects/Quests";
    string objectiveFolder = "Assets/Scriptable Objects/Quests/Objectives";

    [Header("Quest Defaults")]
    string questIdPrefix = "quest";

    [Header("Generation Toggles")]
    bool generateReachFloor = true;
    bool generateMapFloor = true;
    bool generateSlayEnemy = true;
    bool generateCollectItems = true;

    [Header("Floor Generation")]
    int floorStart = 1;
    int floorEnd = 10;
    int mapFloorAreasOverride = 0;

    [Header("Enemy Generation")]
    string killCountsCsv = "1,3,5,10";

    [Header("Item Generation")]
    string itemIdsMultiline = "potion_health\npotion_mana\niron_ore\nwood_log";
    string collectAmountsCsv = "1,3,5,10";

    [Header("Reward Scaling - Reach Floor")]
    int reachFloorBaseXP = 25;
    int reachFloorXPPerFloor = 15;
    int reachFloorBaseGold = 10;
    int reachFloorGoldPerFloor = 5;

    [Header("Reward Scaling - Map Floor")]
    int mapFloorBaseXP = 40;
    int mapFloorXPPerFloor = 20;
    int mapFloorBaseGold = 15;
    int mapFloorGoldPerFloor = 6;
    int mapFloorAreaBonusXP = 2;
    int mapFloorAreaBonusGold = 1;

    [Header("Reward Scaling - Slay Enemy")]
    int slayEnemyBaseXP = 15;
    int slayEnemyXPPerKill = 8;
    int slayEnemyBaseGold = 8;
    int slayEnemyGoldPerKill = 3;

    [Header("Reward Scaling - Collect Items")]
    int collectBaseXP = 10;
    int collectXPPerItem = 5;
    int collectBaseGold = 5;
    int collectGoldPerItem = 2;

    Vector2 scroll;

    [MenuItem("Tools/Quest Generator")]
    public static void ShowWindow()
    {
        GetWindow<QuestGeneratorWindow>("Quest Generator");
    }

    void OnGUI()
    {
        scroll = EditorGUILayout.BeginScrollView(scroll);

        GUILayout.Label("Output Folders", EditorStyles.boldLabel);
        questFolder = EditorGUILayout.TextField("Quest Folder", questFolder);
        objectiveFolder = EditorGUILayout.TextField("Objective Folder", objectiveFolder);

        GUILayout.Space(8);
        GUILayout.Label("Quest Defaults", EditorStyles.boldLabel);
        questIdPrefix = EditorGUILayout.TextField("Quest ID Prefix", questIdPrefix);

        GUILayout.Space(8);
        GUILayout.Label("What To Generate", EditorStyles.boldLabel);
        generateReachFloor = EditorGUILayout.Toggle("Reach Floor Quests", generateReachFloor);
        generateMapFloor = EditorGUILayout.Toggle("Map Floor Quests", generateMapFloor);
        generateSlayEnemy = EditorGUILayout.Toggle("Slay Enemy Quests", generateSlayEnemy);
        generateCollectItems = EditorGUILayout.Toggle("Collect Item Quests", generateCollectItems);

        GUILayout.Space(8);
        GUILayout.Label("Floor Generation", EditorStyles.boldLabel);
        floorStart = EditorGUILayout.IntField("Floor Start", floorStart);
        floorEnd = EditorGUILayout.IntField("Floor End", floorEnd);
        mapFloorAreasOverride = EditorGUILayout.IntField("Map Areas Override", mapFloorAreasOverride);

        GUILayout.Space(8);
        GUILayout.Label("Enemy Generation", EditorStyles.boldLabel);
        killCountsCsv = EditorGUILayout.TextField("Kill Counts CSV", killCountsCsv);

        GUILayout.Space(8);
        GUILayout.Label("Collect Item Generation", EditorStyles.boldLabel);
        GUILayout.Label("Item IDs (one per line)", EditorStyles.miniBoldLabel);
        itemIdsMultiline = EditorGUILayout.TextArea(itemIdsMultiline, GUILayout.MinHeight(90));
        collectAmountsCsv = EditorGUILayout.TextField("Collect Amounts CSV", collectAmountsCsv);

        GUILayout.Space(12);
        GUILayout.Label("Reward Scaling - Reach Floor", EditorStyles.boldLabel);
        reachFloorBaseXP = EditorGUILayout.IntField("Base XP", reachFloorBaseXP);
        reachFloorXPPerFloor = EditorGUILayout.IntField("XP Per Floor", reachFloorXPPerFloor);
        reachFloorBaseGold = EditorGUILayout.IntField("Base Gold", reachFloorBaseGold);
        reachFloorGoldPerFloor = EditorGUILayout.IntField("Gold Per Floor", reachFloorGoldPerFloor);

        GUILayout.Space(8);
        GUILayout.Label("Reward Scaling - Map Floor", EditorStyles.boldLabel);
        mapFloorBaseXP = EditorGUILayout.IntField("Base XP", mapFloorBaseXP);
        mapFloorXPPerFloor = EditorGUILayout.IntField("XP Per Floor", mapFloorXPPerFloor);
        mapFloorBaseGold = EditorGUILayout.IntField("Base Gold", mapFloorBaseGold);
        mapFloorGoldPerFloor = EditorGUILayout.IntField("Gold Per Floor", mapFloorGoldPerFloor);
        mapFloorAreaBonusXP = EditorGUILayout.IntField("Area Bonus XP", mapFloorAreaBonusXP);
        mapFloorAreaBonusGold = EditorGUILayout.IntField("Area Bonus Gold", mapFloorAreaBonusGold);

        GUILayout.Space(8);
        GUILayout.Label("Reward Scaling - Slay Enemy", EditorStyles.boldLabel);
        slayEnemyBaseXP = EditorGUILayout.IntField("Base XP", slayEnemyBaseXP);
        slayEnemyXPPerKill = EditorGUILayout.IntField("XP Per Kill", slayEnemyXPPerKill);
        slayEnemyBaseGold = EditorGUILayout.IntField("Base Gold", slayEnemyBaseGold);
        slayEnemyGoldPerKill = EditorGUILayout.IntField("Gold Per Kill", slayEnemyGoldPerKill);

        GUILayout.Space(8);
        GUILayout.Label("Reward Scaling - Collect Items", EditorStyles.boldLabel);
        collectBaseXP = EditorGUILayout.IntField("Base XP", collectBaseXP);
        collectXPPerItem = EditorGUILayout.IntField("XP Per Item", collectXPPerItem);
        collectBaseGold = EditorGUILayout.IntField("Base Gold", collectBaseGold);
        collectGoldPerItem = EditorGUILayout.IntField("Gold Per Item", collectGoldPerItem);

        GUILayout.Space(14);

        if (GUILayout.Button("Generate Quest Pack", GUILayout.Height(36)))
        {
            GenerateQuestPack();
        }

        EditorGUILayout.EndScrollView();
    }

    void GenerateQuestPack()
    {
        if (floorEnd < floorStart)
        {
            Debug.LogError("Floor End must be >= Floor Start.");
            return;
        }

        EnsureFolderExists(questFolder);
        EnsureFolderExists(objectiveFolder);
        EnsureFolderExists(objectiveFolder + "/ReachFloor");
        EnsureFolderExists(objectiveFolder + "/MapFloor");
        EnsureFolderExists(objectiveFolder + "/SlayEnemy");
        EnsureFolderExists(objectiveFolder + "/CollectItems");

        int createdObjectives = 0;
        int createdQuests = 0;

        if (generateReachFloor)
            GenerateReachFloorSet(ref createdObjectives, ref createdQuests);

        if (generateMapFloor)
            GenerateMapFloorSet(ref createdObjectives, ref createdQuests);

        if (generateSlayEnemy)
            GenerateSlayEnemySet(ref createdObjectives, ref createdQuests);

        if (generateCollectItems)
            GenerateCollectItemSet(ref createdObjectives, ref createdQuests);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Quest generation complete. Created {createdObjectives} objectives and {createdQuests} quests.");
    }

    void GenerateReachFloorSet(ref int createdObjectives, ref int createdQuests)
    {
        for (int floor = floorStart; floor <= floorEnd; floor++)
        {
            var objective = ScriptableObject.CreateInstance<ObjectiveReachFloorSO>();
            objective.targetFloor = floor;
            objective.name = $"OBJ_ReachFloor_{floor}";

            string objectivePath = CreateAsset(
                objective,
                objectiveFolder + "/ReachFloor",
                objective.name
            );

            int xp = Mathf.Max(1, reachFloorBaseXP + (floor * reachFloorXPPerFloor));
            int gold = Mathf.Max(0, reachFloorBaseGold + (floor * reachFloorGoldPerFloor));

            CreateQuestAsset(
                questId: $"{questIdPrefix}_reach_floor_{floor}",
                title: $"Reach Floor {floor}",
                description: $"Descend until you reach Floor {floor}.",
                objective: AssetDatabase.LoadAssetAtPath<ObjectiveReachFloorSO>(objectivePath),
                questFileName: $"QUEST_ReachFloor_{floor}",
                experience: xp,
                gold: gold
            );

            createdObjectives++;
            createdQuests++;
        }
    }

    void GenerateMapFloorSet(ref int createdObjectives, ref int createdQuests)
    {
        for (int floor = floorStart; floor <= floorEnd; floor++)
        {
            var objective = ScriptableObject.CreateInstance<ObjectiveMapFloorSO>();
            objective.targetFloor = floor;
            objective.totalAreasOverride = mapFloorAreasOverride;
            objective.name = $"OBJ_MapFloor_{floor}";

            string objectivePath = CreateAsset(
                objective,
                objectiveFolder + "/MapFloor",
                objective.name
            );

            int areaCount = Mathf.Max(0, mapFloorAreasOverride);
            int xp = Mathf.Max(1, mapFloorBaseXP + (floor * mapFloorXPPerFloor) + (areaCount * mapFloorAreaBonusXP));
            int gold = Mathf.Max(0, mapFloorBaseGold + (floor * mapFloorGoldPerFloor) + (areaCount * mapFloorAreaBonusGold));

            CreateQuestAsset(
                questId: $"{questIdPrefix}_map_floor_{floor}",
                title: $"Map Floor {floor}",
                description: $"Discover all reachable areas on Floor {floor}.",
                objective: AssetDatabase.LoadAssetAtPath<ObjectiveMapFloorSO>(objectivePath),
                questFileName: $"QUEST_MapFloor_{floor}",
                experience: xp,
                gold: gold
            );

            createdObjectives++;
            createdQuests++;
        }
    }

    void GenerateSlayEnemySet(ref int createdObjectives, ref int createdQuests)
    {
        int[] killCounts = ParseIntCsv(killCountsCsv);
        Array enemyValues = Enum.GetValues(typeof(EnemyType));

        foreach (EnemyType enemyType in enemyValues)
        {
            for (int i = 0; i < killCounts.Length; i++)
            {
                int requiredKills = Mathf.Max(1, killCounts[i]);

                var objective = ScriptableObject.CreateInstance<ObjectiveSlayEnemySO>();
                objective.targetEnemy = enemyType;
                objective.requiredKills = requiredKills;
                objective.name = $"OBJ_Slay_{enemyType}_{requiredKills}";

                string objectivePath = CreateAsset(
                    objective,
                    objectiveFolder + "/SlayEnemy",
                    objective.name
                );

                int xp = Mathf.Max(1, slayEnemyBaseXP + (requiredKills * slayEnemyXPPerKill));
                int gold = Mathf.Max(0, slayEnemyBaseGold + (requiredKills * slayEnemyGoldPerKill));

                CreateQuestAsset(
                    questId: $"{questIdPrefix}_slay_{Sanitize(enemyType.ToString()).ToLower()}_{requiredKills}",
                    title: $"Slay {requiredKills} {enemyType}",
                    description: $"Eliminate {requiredKills} {enemyType}.",
                    objective: AssetDatabase.LoadAssetAtPath<ObjectiveSlayEnemySO>(objectivePath),
                    questFileName: $"QUEST_Slay_{enemyType}_{requiredKills}",
                    experience: xp,
                    gold: gold
                );

                createdObjectives++;
                createdQuests++;
            }
        }
    }

    void GenerateCollectItemSet(ref int createdObjectives, ref int createdQuests)
    {
        string[] itemIds = ParseLines(itemIdsMultiline);
        int[] amounts = ParseIntCsv(collectAmountsCsv);

        for (int i = 0; i < itemIds.Length; i++)
        {
            string itemId = itemIds[i];
            if (string.IsNullOrWhiteSpace(itemId))
                continue;

            for (int j = 0; j < amounts.Length; j++)
            {
                int requiredAmount = Mathf.Max(1, amounts[j]);

                var objective = ScriptableObject.CreateInstance<ObjectiveCollectItemsSO>();
                objective.itemId = itemId;
                objective.requiredAmount = requiredAmount;
                objective.name = $"OBJ_Collect_{Sanitize(itemId)}_{requiredAmount}";

                string objectivePath = CreateAsset(
                    objective,
                    objectiveFolder + "/CollectItems",
                    objective.name
                );

                int xp = Mathf.Max(1, collectBaseXP + (requiredAmount * collectXPPerItem));
                int gold = Mathf.Max(0, collectBaseGold + (requiredAmount * collectGoldPerItem));

                CreateQuestAsset(
                    questId: $"{questIdPrefix}_collect_{Sanitize(itemId).ToLower()}_{requiredAmount}",
                    title: $"Collect {requiredAmount} {itemId}",
                    description: $"Bring back {requiredAmount} {itemId}.",
                    objective: AssetDatabase.LoadAssetAtPath<ObjectiveCollectItemsSO>(objectivePath),
                    questFileName: $"QUEST_Collect_{Sanitize(itemId)}_{requiredAmount}",
                    experience: xp,
                    gold: gold
                );

                createdObjectives++;
                createdQuests++;
            }
        }
    }

    void CreateQuestAsset(
        string questId,
        string title,
        string description,
        QuestObjectiveSO objective,
        string questFileName,
        int experience,
        int gold)
    {
        QuestData quest = ScriptableObject.CreateInstance<QuestData>();

        quest.questId = questId;
        quest.title = title;
        quest.description = description;
        quest.objectives = new List<QuestObjectiveSO>();

        if (objective != null)
            quest.objectives.Add(objective);

        quest.reward = new QuestReward
        {
            experience = experience,
            gold = gold
        };

        quest.name = title;

        CreateAsset(quest, questFolder, questFileName);
    }

    string CreateAsset(UnityEngine.Object asset, string folder, string fileName)
    {
        EnsureFolderExists(folder);

        string path = Path.Combine(folder, fileName + ".asset");
        path = AssetDatabase.GenerateUniqueAssetPath(path);
        AssetDatabase.CreateAsset(asset, path);
        return path;
    }

    void EnsureFolderExists(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath))
            return;

        string[] parts = folderPath.Split('/');
        if (parts.Length == 0 || parts[0] != "Assets")
        {
            Debug.LogError("Folder path must start with Assets");
            return;
        }

        string current = parts[0];

        for (int i = 1; i < parts.Length; i++)
        {
            string next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);

            current = next;
        }
    }

    int[] ParseIntCsv(string csv)
    {
        if (string.IsNullOrWhiteSpace(csv))
            return new[] { 1 };

        string[] split = csv.Split(',');
        List<int> values = new List<int>();

        for (int i = 0; i < split.Length; i++)
        {
            string s = split[i].Trim();
            if (int.TryParse(s, out int value))
                values.Add(value);
        }

        if (values.Count == 0)
            values.Add(1);

        return values.ToArray();
    }

    string[] ParseLines(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return Array.Empty<string>();

        string[] raw = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        List<string> clean = new List<string>();

        for (int i = 0; i < raw.Length; i++)
        {
            string line = raw[i].Trim();
            if (!string.IsNullOrWhiteSpace(line))
                clean.Add(line);
        }

        return clean.ToArray();
    }

    string Sanitize(string input)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
            input = input.Replace(c.ToString(), "");

        return input.Replace(" ", "_");
    }
}