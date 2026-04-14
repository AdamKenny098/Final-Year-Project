using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlannedDecorPlacement
{
    public string itemName;
    public string itemTag;
    public string clusterId;

    public DecorSlotTier tier;
    public Vector2Int anchorGridPos;

    public int width;
    public int length;

    public GameObject prefab;
    public Vector3 worldPosition;
    public Quaternion localRotation;

    public List<Vector2Int> occupiedCells = new List<Vector2Int>();
    public GameObject spawnedInstance;

    public PlannedDecorPlacement(
        string itemName,
        string itemTag,
        string clusterId,
        DecorSlotTier tier,
        Vector2Int anchorGridPos,
        int width,
        int length,
        GameObject prefab,
        Vector3 worldPosition,
        Quaternion localRotation,
        List<Vector2Int> occupiedCells)
    {
        this.itemName = itemName;
        this.itemTag = itemTag;
        this.clusterId = clusterId;
        this.tier = tier;
        this.anchorGridPos = anchorGridPos;
        this.width = width;
        this.length = length;
        this.prefab = prefab;
        this.worldPosition = worldPosition;
        this.localRotation = localRotation;
        this.occupiedCells = occupiedCells;
    }
}