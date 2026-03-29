using System.IO;
using UnityEditor;
using UnityEngine;

public class ArmorSetGeneratorWindow : EditorWindow
{
    string saveFolder = "Assets/Scriptable Objects/Items/Armor";
    string baseItemName = "Iron Helmet";
    string itemIdPrefix = "iron_helmet";
    ArmorItem.ArmorType armorType = ArmorItem.ArmorType.Helmet;
    Sprite icon;
    bool isSellable = true;
    int maxStack = 1;

    int commonValue = 10;
    int uncommonValue = 20;
    int rareValue = 35;
    int epicValue = 55;
    int legendaryValue = 80;

    int commonDefense = 2;
    int uncommonDefense = 4;
    int rareDefense = 6;
    int epicDefense = 9;
    int legendaryDefense = 13;

    int uncommonBonus = 1;
    int rareBonus = 2;
    int epicBonus = 3;
    int legendaryBonus = 5;

    [MenuItem("Tools/Armor Set Generator")]
    public static void ShowWindow()
    {
        GetWindow<ArmorSetGeneratorWindow>("Armor Set Generator");
    }

    void OnGUI()
    {
        GUILayout.Label("Output", EditorStyles.boldLabel);
        saveFolder = EditorGUILayout.TextField("Save Folder", saveFolder);

        GUILayout.Space(8);
        GUILayout.Label("Base Item Setup", EditorStyles.boldLabel);
        baseItemName = EditorGUILayout.TextField("Base Item Name", baseItemName);
        itemIdPrefix = EditorGUILayout.TextField("Item ID Prefix", itemIdPrefix);
        armorType = (ArmorItem.ArmorType)EditorGUILayout.EnumPopup("Armor Type", armorType);
        icon = (Sprite)EditorGUILayout.ObjectField("Icon", icon, typeof(Sprite), false);
        isSellable = EditorGUILayout.Toggle("Is Sellable", isSellable);
        maxStack = EditorGUILayout.IntField("Max Stack", maxStack);

        GUILayout.Space(8);
        GUILayout.Label("Base Values", EditorStyles.boldLabel);
        commonValue = EditorGUILayout.IntField("Common Value", commonValue);
        uncommonValue = EditorGUILayout.IntField("Uncommon Value", uncommonValue);
        rareValue = EditorGUILayout.IntField("Rare Value", rareValue);
        epicValue = EditorGUILayout.IntField("Epic Value", epicValue);
        legendaryValue = EditorGUILayout.IntField("Legendary Value", legendaryValue);

        GUILayout.Space(8);
        GUILayout.Label("Base Defense", EditorStyles.boldLabel);
        commonDefense = EditorGUILayout.IntField("Common Defense", commonDefense);
        uncommonDefense = EditorGUILayout.IntField("Uncommon Defense", uncommonDefense);
        rareDefense = EditorGUILayout.IntField("Rare Defense", rareDefense);
        epicDefense = EditorGUILayout.IntField("Epic Defense", epicDefense);
        legendaryDefense = EditorGUILayout.IntField("Legendary Defense", legendaryDefense);

        GUILayout.Space(8);
        GUILayout.Label("Bonus Amounts", EditorStyles.boldLabel);
        uncommonBonus = EditorGUILayout.IntField("Uncommon Bonus", uncommonBonus);
        rareBonus = EditorGUILayout.IntField("Rare Bonus", rareBonus);
        epicBonus = EditorGUILayout.IntField("Epic Bonus", epicBonus);
        legendaryBonus = EditorGUILayout.IntField("Legendary Bonus", legendaryBonus);

        GUILayout.Space(12);

        if (GUILayout.Button("Generate Armor Set", GUILayout.Height(35)))
        {
            GenerateArmorSet();
        }
    }

    void GenerateArmorSet()
    {
        EnsureFolderExists(saveFolder);

        CreateArmorAsset(
            rarity: 1,
            itemName: baseItemName,
            assetFileName: $"Common_{Sanitize(baseItemName)}",
            value: commonValue,
            defense: commonDefense,
            str: 0,
            dex: 0,
            intel: 0,
            cha: 0,
            itemId: $"{itemIdPrefix}_common"
        );

        CreateRaritySet("Uncommon", 2, uncommonValue, uncommonDefense, uncommonBonus);
        CreateRaritySet("Rare", 3, rareValue, rareDefense, rareBonus);
        CreateRaritySet("Epic", 4, epicValue, epicDefense, epicBonus);
        CreateRaritySet("Legendary", 5, legendaryValue, legendaryDefense, legendaryBonus);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Armor set generated in: {saveFolder}");
    }

    void CreateRaritySet(string rarityName, int rarity, int value, int defense, int bonusAmount)
    {
        string safeBaseName = Sanitize(baseItemName);

        CreateArmorAsset(
            rarity: rarity,
            itemName: baseItemName,
            assetFileName: $"{rarityName}_{safeBaseName}",
            value: value,
            defense: defense,
            str: 0,
            dex: 0,
            intel: 0,
            cha: 0,
            itemId: $"{itemIdPrefix}_{rarityName.ToLower()}"
        );

        CreateArmorAsset(
            rarity: rarity,
            itemName: $"{baseItemName} of Strength",
            assetFileName: $"{rarityName}_{safeBaseName}_Strength",
            value: value,
            defense: defense,
            str: bonusAmount,
            dex: 0,
            intel: 0,
            cha: 0,
            itemId: $"{itemIdPrefix}_{rarityName.ToLower()}_strength"
        );

        CreateArmorAsset(
            rarity: rarity,
            itemName: $"{baseItemName} of Dexterity",
            assetFileName: $"{rarityName}_{safeBaseName}_Dexterity",
            value: value,
            defense: defense,
            str: 0,
            dex: bonusAmount,
            intel: 0,
            cha: 0,
            itemId: $"{itemIdPrefix}_{rarityName.ToLower()}_dexterity"
        );

        CreateArmorAsset(
            rarity: rarity,
            itemName: $"{baseItemName} of Intelligence",
            assetFileName: $"{rarityName}_{safeBaseName}_Intelligence",
            value: value,
            defense: defense,
            str: 0,
            dex: 0,
            intel: bonusAmount,
            cha: 0,
            itemId: $"{itemIdPrefix}_{rarityName.ToLower()}_intelligence"
        );

        CreateArmorAsset(
            rarity: rarity,
            itemName: $"{baseItemName} of Charisma",
            assetFileName: $"{rarityName}_{safeBaseName}_Charisma",
            value: value,
            defense: defense,
            str: 0,
            dex: 0,
            intel: 0,
            cha: bonusAmount,
            itemId: $"{itemIdPrefix}_{rarityName.ToLower()}_charisma"
        );
    }

    void CreateArmorAsset(
        int rarity,
        string itemName,
        string assetFileName,
        int value,
        int defense,
        int str,
        int dex,
        int intel,
        int cha,
        string itemId)
    {
        ArmorItem asset = ScriptableObject.CreateInstance<ArmorItem>();

        asset.name = itemName;
        asset.maxStack = maxStack;
        asset.icon = icon;
        asset.rarity = rarity;
        asset.value = value;
        asset.isSellable = isSellable;
        asset.itemId = itemId;

        asset.armorType = armorType;
        asset.defenseValue = defense;

        asset.strength = str;
        asset.dexterity = dex;
        asset.intelligence = intel;
        asset.charisma = cha;

        string path = Path.Combine(saveFolder, assetFileName + ".asset");
        path = AssetDatabase.GenerateUniqueAssetPath(path);

        AssetDatabase.CreateAsset(asset, path);

        Debug.Log($"Created: {itemName} | Rarity {rarity} | DEF {defense} | STR {str} DEX {dex} INT {intel} CHA {cha}");
    }

    void EnsureFolderExists(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath))
            return;

        string[] parts = folderPath.Split('/');
        if (parts.Length == 0 || parts[0] != "Assets")
        {
            Debug.LogError("Save folder must start with 'Assets'");
            return;
        }

        string current = parts[0];

        for (int i = 1; i < parts.Length; i++)
        {
            string next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
            {
                AssetDatabase.CreateFolder(current, parts[i]);
            }
            current = next;
        }
    }

    string Sanitize(string input)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            input = input.Replace(c.ToString(), "");
        }

        return input.Replace(" ", "_");
    }
}