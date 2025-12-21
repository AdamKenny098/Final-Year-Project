using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonEntitySpawner : MonoBehaviour
{
    public GameObject hitboxTesterPrefab;
    public GameObject playerPrefab;
    public List<Room> allRooms = new List<Room>();
    public void SpawnAll()
    {
        allRooms = DungeonRoomDecorator.Instance.allWorkableRooms;
        SpawnPlayer();
        SpawnEntitiesInRooms();
    }

    public void SpawnPlayer()
    {
        int spawnAttempts = 0;
        int maxSpawnAttempts = 20;

        float playerHeight = 1.5f;
        float playerRadius = 0.5f;

        for(int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            Room playerRoom = allRooms[Random.Range(0, allRooms.Count)];
            Vector3 spawnPosition = DungeonRoomDecorator.Instance.RandomRoomPosition(playerRoom);

            Vector3 finalPos = spawnPosition;
            finalPos.y = 1f;

            if (IsValidSpawn(playerRoom, finalPos, playerHeight, playerRadius))
            {
                Instantiate(playerPrefab, finalPos, Quaternion.identity);
                return;
            }
        }
    }

    bool IsValidSpawn(Room room, Vector3 position, float height, float radius)
    {
        GameObject hitbox = Instantiate(hitboxTesterPrefab, position, Quaternion.identity);

        CapsuleCollider capsule = hitbox.GetComponent<CapsuleCollider>();
        capsule.height = height;
        capsule.radius = radius;
        capsule.center = Vector3.up * (height / 2f);

        Bounds hitboxB = capsule.bounds;

        foreach (Bounds b in room.occupiedAreas)
        {
            if (b.Intersects(hitboxB))
            {
                Destroy(hitbox);
                return false;
            }
        }

        Destroy(hitbox);
        return true;
    }


    public void SpawnEntitiesInRooms()
    {
        
    }


    
}
