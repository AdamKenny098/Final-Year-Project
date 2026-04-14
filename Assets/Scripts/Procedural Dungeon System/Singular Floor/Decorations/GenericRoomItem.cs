using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GenericRoomItem
{
    public string name;

    public GameObject prefab;
    public GenericType type;

    public float areaOccupied;
    public BoxCollider boxC;

public enum GenericType
{
    Torch,
    Pillar,
    TorchPillar
}

void Awake()
    {
        boxC = prefab.GetComponent<BoxCollider>();
        areaOccupied = boxC.size.x * boxC.size.z;
    }
}

