using UnityEngine;

[CreateAssetMenu(menuName = "Dungeon/Room Item Definition", fileName = "RoomItem_")]
public class RoomItemDefinition : ScriptableObject
{
    [Header("Identity")]
    public string itemName;
    public string itemTag;
    public string anchorTag;

    [Header("Prefab")]
    public GameObject prefab;

    [Header("Classification")]
    public RoomItem.Priority priority;

    [Header("Grid Footprint")]
    public int gridWidth = 1;
    public int gridLength = 1;
    public bool allowRotation = true;

    [Header("Count")]
    public int minCount = 0;
    public int maxCount = 1;

    [Header("Placement Bias")]
    public float preferWall = 0f;
    public float preferCenter = 0f;
    public float avoidCenter = 0f;
    public float avoidDoors = 0f;
    public float preferNearAnchor = 0f;
    public float preferNearSameTag = 0f;

    [Header("Distance Rules")]
    public int minAnchorDistance = 0;
    public int maxAnchorDistance = 0;

    public RoomItem ToRuntimeItem()
    {
        return new RoomItem
        {
            name = string.IsNullOrWhiteSpace(itemName) ? name : itemName,
            prefab = prefab,
            priority = priority,
            gridWidth = Mathf.Max(1, gridWidth),
            gridLength = Mathf.Max(1, gridLength),
            minCount = Mathf.Max(0, minCount),
            maxCount = Mathf.Max(minCount, maxCount),
            allowRotation = allowRotation,
            preferWall = preferWall,
            preferCenter = preferCenter,
            avoidCenter = avoidCenter,
            avoidDoors = avoidDoors,
            preferNearAnchor = preferNearAnchor,
            preferNearSameTag = preferNearSameTag,
            anchorTag = anchorTag,
            itemTag = itemTag,
            minAnchorDistance = Mathf.Max(0, minAnchorDistance),
            maxAnchorDistance = Mathf.Max(0, maxAnchorDistance)
        };
    }
}