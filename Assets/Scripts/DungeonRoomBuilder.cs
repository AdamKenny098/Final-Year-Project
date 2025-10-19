using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoomBuilder : MonoBehaviour
{
    public GameObject blockPrefab;

    public void BuildRoom(Node node, float rotationDegrees = 0f, bool isCorridor = false)
    {
        if (node == null) return;

        string wallTag = isCorridor ? "CorridorBlock" : "wallBlock";
        string floorTag = isCorridor ? "CorridorBlock" : "floorBlock";
        string ceilingTag = isCorridor ? "CorridorBlock" : "ceilingBlock";

        GameObject roomAsGameObject = new GameObject(isCorridor ? "Corridor" : "Room");
        roomAsGameObject.transform.position = node.center;
        roomAsGameObject.transform.rotation = Quaternion.Euler(0, rotationDegrees, 0);

        Room roomComponent = roomAsGameObject.AddComponent<Room>();
        roomComponent.node = node;
        roomComponent.isCorridor = isCorridor;

        float width = node.width;
        float length = node.length;
        int roomWidth = Mathf.FloorToInt(width);
        int roomLength = Mathf.FloorToInt(length);

        int floorLevel = -1;
        int ceilingLevel = 5;

        Quaternion rotation = Quaternion.Euler(0, rotationDegrees, 0);

        // === Walls ===
        for (int i = 0; i < roomWidth; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                Vector3 offsetA = new Vector3((-roomWidth / 2f) + i + 0.5f, j, roomLength / 2f - 0.5f);
                Vector3 offsetB = new Vector3((-roomWidth / 2f) + i + 0.5f, j, -roomLength / 2f + 0.5f);

                Vector3 posWallA = node.center + rotation * offsetA;
                Vector3 posWallB = node.center + rotation * offsetB;

                GameObject blockA = Instantiate(blockPrefab, posWallA, rotation, roomAsGameObject.transform);
                GameObject blockB = Instantiate(blockPrefab, posWallB, rotation, roomAsGameObject.transform);
                blockA.tag = wallTag;
                blockB.tag = wallTag;
            }
        }

        for (int k = 0; k < roomLength; k++)
        {
            for (int j = 0; j < 5; j++)
            {
                Vector3 offsetC = new Vector3(roomWidth / 2f - 0.5f, j, (-roomLength / 2f) + k + 0.5f);
                Vector3 offsetD = new Vector3(-roomWidth / 2f + 0.5f, j, (-roomLength / 2f) + k + 0.5f);

                Vector3 posWallC = node.center + rotation * offsetC;
                Vector3 posWallD = node.center + rotation * offsetD;

                GameObject blockC = Instantiate(blockPrefab, posWallC, rotation, roomAsGameObject.transform);
                GameObject blockD = Instantiate(blockPrefab, posWallD, rotation, roomAsGameObject.transform);
                blockC.tag = wallTag;
                blockD.tag = wallTag;
            }
        }

        // === Floor ===
        for (int i = 0; i < roomWidth; i++)
        {
            for (int k = 0; k < roomLength; k++)
            {
                Vector3 offset = new Vector3((-roomWidth / 2f) + i + 0.5f, -floorLevel, (-roomLength / 2f) + k + 0.5f);
                Vector3 posFloor = node.center + rotation * offset;
                GameObject block = Instantiate(blockPrefab, posFloor, rotation, roomAsGameObject.transform);
                block.tag = floorTag;
            }
        }

        // === Ceiling ===
        for (int i = 0; i < roomWidth; i++)
        {
            for (int k = 0; k < roomLength; k++)
            {
                Vector3 offset = new Vector3((-roomWidth / 2f) + i + 0.5f, ceilingLevel, (-roomLength / 2f) + k + 0.5f);
                Vector3 posCeil = node.center + rotation * offset;
                GameObject block = Instantiate(blockPrefab, posCeil, rotation, roomAsGameObject.transform);
                block.tag = ceilingTag;
            }
        }

        BoxCollider boxC = roomAsGameObject.AddComponent<BoxCollider>();
        boxC.center = new Vector3(0, (ceilingLevel - floorLevel - 1) / 2f, 0);
        boxC.size = new Vector3(roomWidth - 2, ceilingLevel - 2, roomLength - 2);
    }
    
    public void DeleteExcessBlocks()
    {
        Room[] everyRoom = FindObjectsOfType<Room>();

        foreach (Room room in everyRoom)
        {
            BoxCollider boxC = room.GetComponent<BoxCollider>();
            if (boxC == null) continue;

            Vector3 worldCenter = boxC.transform.TransformPoint(boxC.center);
            Vector3 halfExtents = (boxC.size / 2f) * 0.965f;

            Collider[] overlaps = Physics.OverlapBox(worldCenter, halfExtents, boxC.transform.rotation);

            foreach (Collider col in overlaps)
            {
                if (col == null) continue;

                // === CASE 1: Room deleting corridor overlap ===
                if (!room.isCorridor && col.CompareTag("CorridorBlock"))
                {
                    Destroy(col.gameObject);
                    continue;
                }

                // === CASE 2: Corridor carving through walls ===
                if (room.isCorridor && col.CompareTag("wallBlock"))
                {
                    Destroy(col.gameObject);
                    continue;
                }
            }
        }
    }


}