using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomItem
{
    public string name;
    public GameObject prefab;
    public Priority priority;

    public BoxCollider boxC;
    public float areaOccupied;

    public enum Priority
    {
        Primary,
        Secondary,
        Tertiary
    }

    void Awake()
    {
        boxC = prefab.GetComponent<BoxCollider>();
        areaOccupied = boxC.size.x * boxC.size.z;
    }
}
