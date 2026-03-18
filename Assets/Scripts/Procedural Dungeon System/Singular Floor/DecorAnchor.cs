using UnityEngine;

public class DecorAnchor
{
    public string tag;
    public Vector2Int gridPos;
    public Vector2Int size;
    public Transform instance;

    public DecorAnchor(string tag, Vector2Int gridPos, Vector2Int size, Transform instance)
    {
        this.tag = tag;
        this.gridPos = gridPos;
        this.size = size;
        this.instance = instance;
    }
}