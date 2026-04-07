using System.IO;
using UnityEditor;
using UnityEngine;

public class WeaponSetGeneratorWindow : EditorWindow
{
    string saveFolder = "Assets/Scriptable Objects/Items/Weapons";
    string baseItemName = "Iron Sword";
    string itemIdPrefix = "iron_sword";

    WeaponItem.WeaponType weaponType = WeaponItem.WeaponType.Sword;
    WeaponItem.ClassType classType = WeaponItem.ClassType.Warrior;
    DamageType damageType = DamageType.Slashing;

    Sprite icon;
    bool isSellable = true;
    int maxStack = 1;

    int commonValue = 12;
    int uncommonValue = 22;
    int rareValue = 36;
    int epicValue = 56;
    int legendaryValue = 84;

    int commonDamage = 3;
    int uncommonDamage = 5;
    int rareDamage = 7;
    int epicDamage = 10;
    int legendaryDamage = 14;

    float commonAttackSpeed = 1f;
    float uncommonAttackSpeed = 1f;
    float rareAttackSpeed = 1f;
    float epicAttackSpeed = 1f;
    float legendaryAttackSpeed = 1f;

    int uncommonBonus = 1;
    int rareBonus = 2;
    int epicBonus = 3;
    int legendaryBonus = 5;

    [MenuItem("Tools/Weapon Set Generator")]
    public static void ShowWindow()
    {
        GetWindow<WeaponSetGeneratorWindow>("Weapon Set Generator");
    }

    void OnGUI()
    {
        GUILayout.Label("Output", EditorStyles.boldLabel);
        saveFolder = EditorGUILayout.TextField("Save Folder", saveFolder);

        GUILayout.Space(8);
        GUILayout.Label("Base Item Setup", EditorStyles.boldLabel);
        baseItemName = EditorGUILayout.TextField("Base Item Name", baseItemName);
        itemIdPrefix = EditorGUILayout.TextField("Item ID Prefix", itemIdPrefix);
        weaponType = (WeaponItem.WeaponType)EditorGUILayout.EnumPopup("Weapon Type", weaponType);
        classType = (WeaponItem.ClassType)EditorGUILayout.EnumPopup("Class Type", classType);
        damageType = (DamageType)EditorGUILayout.EnumPopup("Damage Type", damageType);
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
        GUILayout.Label("Base Damage", EditorStyles.boldLabel);
        commonDamage = EditorGUILayout.IntField("Common Damage", commonDamage);
        uncommonDamage = EditorGUILayout.IntField("Uncommon Damage", uncommonDamage);
        rareDamage = EditorGUILayout.IntField("Rare Damage", rareDamage);
        epicDamage = EditorGUILayout.IntField("Epic Damage", epicDamage);
        legendaryDamage = EditorGUILayout.IntField("Legendary Damage", legendaryDamage);

        GUILayout.Space(8);
        GUILayout.Label("Attack Speed", EditorStyles.boldLabel);
        commonAttackSpeed = EditorGUILayout.FloatField("Common Attack Speed", commonAttackSpeed);
        uncommonAttackSpeed = EditorGUILayout.FloatField("Uncommon Attack Speed", uncommonAttackSpeed);
        rareAttackSpeed = EditorGUILayout.FloatField("Rare Attack Speed", rareAttackSpeed);
        epicAttackSpeed = EditorGUILayout.FloatField("Epic Attack Speed", epicAttackSpeed);
        legendaryAttackSpeed = EditorGUILayout.FloatField("Legendary Attack Speed", legendaryAttackSpeed);

        GUILayout.Space(8);
        GUILayout.Label("Bonus Amounts", EditorStyles.boldLabel);
        uncommonBonus = EditorGUILayout.IntField("Uncommon Bonus", uncommonBonus);
        rareBonus = EditorGUILayout.IntField("Rare Bonus", rareBonus);
        epicBonus = EditorGUILayout.IntField("Epic Bonus", epicBonus);
        legendaryBonus = EditorGUILayout.IntField("Legendary Bonus", legendaryBonus);

        GUILayout.Space(12);

        string bonusStatName = GetBonusStatName(weaponType);
        EditorGUILayout.HelpBox("This weapon type will grant bonus " + bonusStatName + ".", MessageType.Info);

        if (GUILayout.Button("Generate Weapon Set", GUILayout.Height(35)))
        {
            GenerateWeaponSet();
        }
    }

    void GenerateWeaponSet()
    {
        EnsureFolderExists(saveFolder);

        CreateWeaponAsset(
            rarity: 1,
            itemName: baseItemName,
            assetFileName: "Common_" + Sanitize(baseItemName),
            value: commonValue,
            damage: commonDamage,
            attackSpeed: commonAttackSpeed,
            bonusAmount: 0,
            itemId: itemIdPrefix + "_common"
        );

        CreateRaritySet("Uncommon", 2, uncommonValue, uncommonDamage, uncommonAttackSpeed, uncommonBonus);
        CreateRaritySet("Rare", 3, rareValue, rareDamage, rareAttackSpeed, rareBonus);
        CreateRaritySet("Epic", 4, epicValue, epicDamage, epicAttackSpeed, epicBonus);
        CreateRaritySet("Legendary", 5, legendaryValue, legendaryDamage, legendaryAttackSpeed, legendaryBonus);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Weapon set generated in: " + saveFolder);
    }

    void CreateRaritySet(string rarityName, int rarity, int value, int damage, float attackSpeed, int bonusAmount)
    {
        string safeBaseName = Sanitize(baseItemName);
        string suffix = GetBonusSuffix(weaponType);

        CreateWeaponAsset(
            rarity: rarity,
            itemName: baseItemName + " of " + suffix,
            assetFileName: rarityName + "_" + safeBaseName,
            value: value,
            damage: damage,
            attackSpeed: attackSpeed,
            bonusAmount: bonusAmount,
            itemId: itemIdPrefix + "_" + rarityName.ToLower()
        );
    }

    void CreateWeaponAsset(
        int rarity,
        string itemName,
        string assetFileName,
        int value,
        int damage,
        float attackSpeed,
        int bonusAmount,
        string itemId)
    {
        WeaponItem asset = ScriptableObject.CreateInstance<WeaponItem>();

        asset.name = itemName;
        asset.maxStack = maxStack;
        asset.icon = icon;
        asset.rarity = rarity;
        asset.value = value;
        asset.isSellable = isSellable;
        asset.itemId = itemId;

        asset.weaponType = weaponType;
        asset.classType = classType;
        asset.damageType = damageType;
        asset.damage = damage;
        asset.attackSpeed = attackSpeed;

        asset.strength = 0;
        asset.dexterity = 0;
        asset.intelligence = 0;
        asset.charisma = 0;

        ApplyWeaponStatBonus(asset, weaponType, bonusAmount);

        string path = Path.Combine(saveFolder, assetFileName + ".asset");
        path = AssetDatabase.GenerateUniqueAssetPath(path);

        AssetDatabase.CreateAsset(asset, path);

        Debug.Log(
            "Created: " + itemName +
            " | Rarity " + rarity +
            " | DMG " + damage +
            " | STR " + asset.strength +
            " DEX " + asset.dexterity +
            " INT " + asset.intelligence +
            " CHA " + asset.charisma
        );
    }

    void ApplyWeaponStatBonus(WeaponItem asset, WeaponItem.WeaponType type, int bonusAmount)
    {
        switch (type)
        {
            case WeaponItem.WeaponType.Sword:
            case WeaponItem.WeaponType.Axe:
            case WeaponItem.WeaponType.Mace:
                asset.strength = bonusAmount;
                break;

            case WeaponItem.WeaponType.Bow:
                asset.dexterity = bonusAmount;
                break;

            case WeaponItem.WeaponType.Staff:
            case WeaponItem.WeaponType.Wand:
                asset.intelligence = bonusAmount;
                break;

            case WeaponItem.WeaponType.Dagger:
                asset.charisma = bonusAmount;
                break;
        }
    }

    string GetBonusStatName(WeaponItem.WeaponType type)
    {
        switch (type)
        {
            case WeaponItem.WeaponType.Sword:
            case WeaponItem.WeaponType.Axe:
            case WeaponItem.WeaponType.Mace:
                return "Strength";

            case WeaponItem.WeaponType.Bow:
                return "Dexterity";

            case WeaponItem.WeaponType.Staff:
            case WeaponItem.WeaponType.Wand:
                return "Intelligence";

            case WeaponItem.WeaponType.Dagger:
                return "Charisma";
        }

        return "None";
    }

    string GetBonusSuffix(WeaponItem.WeaponType type)
    {
        switch (type)
        {
            case WeaponItem.WeaponType.Sword:
            case WeaponItem.WeaponType.Axe:
            case WeaponItem.WeaponType.Mace:
                return "Strength";

            case WeaponItem.WeaponType.Bow:
                return "Dexterity";

            case WeaponItem.WeaponType.Staff:
            case WeaponItem.WeaponType.Wand:
                return "Intelligence";

            case WeaponItem.WeaponType.Dagger:
                return "Charisma";
        }

        return "Power";
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