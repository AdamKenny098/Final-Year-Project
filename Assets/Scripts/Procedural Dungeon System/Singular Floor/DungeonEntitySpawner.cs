using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonEntitySpawner : MonoBehaviour
{
    public GameObject hitboxTesterPrefab;
    public List<Room> allRooms = new List<Room>();

    [Header("Enemy Spawning")]
    public GameObject enemyPrefab;
    public int minEnemiesPerRoom = 1;
    public int maxEnemiesPerRoom = 3;

    public float enemyHeight = 2f;
    public float enemyRadius = 0.5f;
    
    private Transform dungeonRoot;
    private Transform enemyRoot;
    private Transform npcRoot;

    private DungeonRoomBuilder builder;
    private DungeonRoomDecorator decorator;
    private Transform floorRoot;

    public void FillReferences(DungeonRoomBuilder builder,DungeonRoomDecorator decorator,Transform floorRoot)
    {
        this.builder = builder;
        this.decorator = decorator;
        this.floorRoot = floorRoot;

        SetupRoots();
    }

    void SetupRoots()
    {
        dungeonRoot = floorRoot;

        enemyRoot = new GameObject("Enemies").transform;
        enemyRoot.SetParent(dungeonRoot, false);

        npcRoot = new GameObject("NPCs").transform;
        npcRoot.SetParent(dungeonRoot, false);
    }

    public void SpawnAll()
    {
        allRooms = decorator.allWorkableRooms;
        SpawnPlayer();
        SpawnEntitiesInRooms();
    }

    public void SpawnPlayer()
    {
        int maxSpawnAttempts = 20;

        float playerHeight = 1.5f;
        float playerRadius = 0.5f;

        for(int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            Room playerRoom = allRooms[Random.Range(0, allRooms.Count)];
            Vector3 spawnPosition = decorator.RandomRoomPosition(playerRoom);

            Vector3 finalPos = spawnPosition;
            finalPos.y = 1.55f;

            if (IsValidSpawn(playerRoom, finalPos, playerHeight, playerRadius))
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");

                if (player == null)
                {
                    Debug.LogError("Player not found in scene.");
                    return;
                }

                player.transform.position = finalPos;
                player.transform.rotation = Quaternion.identity;

                playerRoom.preventSpawning = true;
                playerRoom.preventEnemySpawning = true;

                DungeonManager.Instance.SpawnLastFloorExit(playerRoom);
                return;
            }
        }
    }

    bool IsValidSpawn(Room room, Vector3 position, float height, float radius)
{
    float clearanceX = 2f;
    float clearanceZ = 2f;

    Bounds spawnBounds = new Bounds(
        position + Vector3.up * (height * 0.5f),
        new Vector3(clearanceX, height, clearanceZ)
    );

    foreach (Bounds b in room.occupiedAreas)
    {
        if (b.Intersects(spawnBounds))
            return false;
    }

    return true;
}


    public void SpawnEntitiesInRooms()
    {
        int maxSpawnAttempts = 15;

        foreach (Room room in allRooms)
        {
            if (room.preventSpawning || room.preventEnemySpawning)
                continue;
            
            int enemiesToSpawn = Random.Range(minEnemiesPerRoom, maxEnemiesPerRoom + 1);

            for (int i = 0; i < enemiesToSpawn; i++)
            {
                bool spawned = false;

                for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
                {
                    Vector3 spawnPos = decorator.RandomRoomPosition(room);
                    spawnPos.y = 2.5f;

                    if (IsValidSpawn(room, spawnPos, enemyHeight, enemyRadius))
                    {
                        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity, enemyRoot);

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

}
