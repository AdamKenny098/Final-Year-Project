using UnityEngine;

public class RoomMapTracker : MonoBehaviour
{
    public static RoomMapTracker Instance { get; private set; }

    public Room CurrentRoom { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SetCurrentRoom(Room room)
    {
        CurrentRoom = room;
    }

    public void ClearCurrentRoom(Room room)
    {
        if (CurrentRoom == room)
            CurrentRoom = null;
    }
}