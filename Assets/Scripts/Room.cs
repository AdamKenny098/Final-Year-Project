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

    public List<Bounds> occupiedAreas = new List<Bounds>();
    public Transform pillarsRoot;
    public Transform torchesRoot;
    public Transform roomItemsRoot;
    public Transform DecorRoot;

    public void EnsureDecorationRoots()
    {
        if (isCorridor) return;

        if (pillarsRoot == null)
            pillarsRoot = new GameObject("Pillars").transform;

        if (torchesRoot == null)
            torchesRoot = new GameObject("Torches").transform;

        if (roomItemsRoot == null)
            roomItemsRoot = new GameObject("Room Items").transform;

        pillarsRoot.SetParent(transform, false);
        torchesRoot.SetParent(transform, false);
        roomItemsRoot.SetParent(transform, false);
    }

    public void FinalizeDecorations()
    {
        if (isCorridor) return;

        EnsureDecorationRoots();

        if (DecorRoot != null) return;

        DecorRoot = new GameObject("Room Decorations").transform;
        DecorRoot.SetParent(transform, false);

        pillarsRoot.SetParent(DecorRoot, true);
        torchesRoot.SetParent(DecorRoot, true);
        roomItemsRoot.SetParent(DecorRoot, true);
    }
}
