using UnityEngine;

[System.Serializable]
public class DecorSlot
{
    public Vector2Int gridPos;
    public DecorSlotTier tier;
    public bool occupied;
    public string ownerClusterId;
    public string occupantId;
    public bool enabled = true;

    public DecorSlot(Vector2Int gridPos, DecorSlotTier tier, string ownerClusterId)
    {
        this.gridPos = gridPos;
        this.tier = tier;
        this.ownerClusterId = ownerClusterId;
        occupied = false;
        occupantId = string.Empty;
    }

    public bool IsAvailable => !occupied;

    public void Occupy(string occupantId)
    {
        occupied = true;
        this.occupantId = occupantId;
    }

    public void Clear()
    {
        occupied = false;
        occupantId = string.Empty;
    }
}