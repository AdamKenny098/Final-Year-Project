using UnityEngine;

[System.Serializable]
public class RoomTile
{
    public Vector2Int gridPos;
    public UnityEngine.Vector3 worldPos;

    public bool blocked;
    public bool reserved;
    public bool isDoorway;
    public bool isDoorBuffer;

    public float wallScore;
    public float centerScore;
    public float doorDistance;

    public bool IsFree => !blocked && !reserved;
}