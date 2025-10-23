using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomItem
{
    public string name;
    public GameObject prefab;
    public Priority priority;

    public enum Priority
    {
        Primary,
        Secondary,
        Tertiary
    }
}
