using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{

    public enum RoomType
    {
        Default,
        Tomb,
        Forge,
        Library,
        Armory,
        Barracks,
        Altar,
        Treasury,
        DiningHall,
        Warehouse,
        ShopKeeper,
        Kitchen,
        Tavern,
        Prison,

    }

    public Node node;
    public bool isCorridor = false;
    public RoomType roomType = RoomType.Default;
    public float roomArea;
    public float availableArea;
}
