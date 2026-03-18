using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomItem
{
    public string name;
    public GameObject prefab;
    public Priority priority = Priority.Secondary;

    public int gridWidth = 1;
    public int gridLength = 1;

    public int minCount = 0;
    public int maxCount = 1;
    public bool allowRotation = true;

    public float preferWall = 0f;
    public float preferCenter = 0f;
    public float avoidCenter = 0f;
    public float avoidDoors = 3f;
    public float preferNearSameTag = 0f;
    public float preferNearAnchor = 0f;

    public string itemTag = "";
    public string anchorTag = "";
    public int minAnchorDistance = 1;
    public int maxAnchorDistance = 4;

    public int doorwayBufferPenaltyRadius = 2;
    public int extraClearance = 0;

    public enum Priority
    {
        Primary,
        Secondary,
        Tertiary
    }
}