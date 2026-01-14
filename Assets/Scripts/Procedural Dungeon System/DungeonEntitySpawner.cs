using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonEntitySpawner : MonoBehaviour
{
    public GameObject hitboxTesterPrefab;
    public GameObject playerPrefab;
    public List<Room> allRooms = new List<Room>();

    [Header("Enemy Spawning")]
    public GameObject enemyPrefab;
    public int minEnemiesPerRoom = 1;
    public int maxEnemiesPerRoom = 3;

    public float enemyHeight = 2f;
    public float enemyRadius = 0.5f;

    [Header("NPC Spawning")]
    public GameObject merchantPrefab;

    public void SpawnAll()
    {
        allRooms = DungeonRoomDecorator.Instance.allWorkableRooms;
        SpawnPlayer();
        SpawnEntitiesInRooms();
        SpawnMerchantInRoom();
    }

    public void SpawnPlayer()
    {
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
                

                playerRoom.preventSpawning = true;
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
        int maxSpawnAttempts = 15;

        foreach (Room room in allRooms)
        {
            if (room.roomType == Room.RoomType.ShopKeeper)
            {   
                room.preventEnemySpawning = true;
                continue;
            }

            if (room.preventEnemySpawning) continue;
            
            int enemiesToSpawn = Random.Range(minEnemiesPerRoom, maxEnemiesPerRoom + 1);

            for (int i = 0; i < enemiesToSpawn; i++)
            {
                bool spawned = false;

                for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
                {
                    Vector3 spawnPos = DungeonRoomDecorator.Instance.RandomRoomPosition(room);
                    spawnPos.y = 2.5f;

                    if (IsValidSpawn(room, spawnPos, enemyHeight, enemyRadius))
                    {
                        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

                        // Register occupied space so future spawns respect it
                        RegisterOccupiedArea(room, spawnPos, enemyHeight, enemyRadius);

                        spawned = true;
                        break;
                    }
                }

                if (!spawned)
                {
                    continue;
                }
            }
        }
    }

    public void RegisterOccupiedArea(Room room, Vector3 position, float height, float radius)
    {
        Bounds b = new Bounds(
            position + Vector3.up * (height / 2f),
            new Vector3(radius * 2f, height, radius * 2f)
        );

        room.occupiedAreas.Add(b);
    }

    void SpawnMerchantInRoom()
    {
        int maxAttempts = 10;

        foreach (Room room in allRooms)
        {
            if (room.roomType != Room.RoomType.ShopKeeper)
                continue;

            if (room.preventNPCSpawning)
                continue;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                Vector3 spawnPos = DungeonRoomDecorator.Instance.RandomRoomPosition(room);
                spawnPos.y = 2.5f;

                if (IsValidSpawn(room, spawnPos, enemyHeight, enemyRadius))
                {
                    Instantiate(
                        merchantPrefab,
                        spawnPos,
                        Quaternion.identity,
                        room.transform
                    );

                    RegisterOccupiedArea(room, spawnPos, enemyHeight, enemyRadius);
                    room.preventNPCSpawning = true;

                    break;
                }
            }
        }
    }





    
}
