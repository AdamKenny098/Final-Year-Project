using System.Collections.Generic;
using UnityEngine;

public class RoomEnemyActivityManager : MonoBehaviour
{
    public static RoomEnemyActivityManager Instance;

    private readonly List<Room> trackedRooms = new List<Room>();
    private Room currentRoom;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void RegisterRooms(List<Room> rooms)
    {
        trackedRooms.Clear();

        if (rooms == null)
            return;

        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i] != null && !trackedRooms.Contains(rooms[i]))
                trackedRooms.Add(rooms[i]);
        }

        RefreshEnemyActivity();
    }

    public void SetCurrentRoom(Room room)
    {
        currentRoom = room;
        RefreshEnemyActivity();
    }

    void RefreshEnemyActivity()
    {
        for (int i = 0; i < trackedRooms.Count; i++)
        {
            Room room = trackedRooms[i];
            if (room == null)
                continue;

            bool shouldBeActive = room == currentRoom;

            for (int j = 0; j < room.spawnedEnemies.Count; j++)
            {
                GameObject enemy = room.spawnedEnemies[j];
                if (enemy == null)
                    continue;

                EnemyPerformanceController perf = enemy.GetComponent<EnemyPerformanceController>();
                if (perf != null)
                    perf.SetActiveState(shouldBeActive);
            }
        }
    }
}