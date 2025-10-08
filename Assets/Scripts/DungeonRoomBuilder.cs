using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoomBuilder : MonoBehaviour
{
    public GameObject blockPrefab;

    public void BuildRoom(Node node, float rotationDegrees = 0f)
    {
        if (node == null) return;

        //Node is passed from DungeonGenerator
        float width = node.width;
        float length = node.length;

        int roomWidth = Mathf.FloorToInt(width);
        int roomLength = Mathf.FloorToInt(length);

        int floorLevel = -1;
        int ceilingLevel = 5; //Change it later to roomHeight+1

        Quaternion rotation = Quaternion.Euler(0, rotationDegrees, 0);


        for (int i = 0; i < roomWidth; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                Vector3 offsetA = new Vector3((-roomWidth / 2f) + i + 0.5f, j, roomLength / 2f - 0.5f);
                Vector3 offsetB = new Vector3((-roomWidth / 2f) + i + 0.5f, j, -roomLength / 2f + 0.5f);

                Vector3 posWallA = node.center + rotation * offsetA;
                Vector3 posWallB = node.center + rotation * offsetB;

                Instantiate(blockPrefab, posWallA, rotation);
                Instantiate(blockPrefab, posWallB, rotation);
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

                Instantiate(blockPrefab, posWallC, rotation);
                Instantiate(blockPrefab, posWallD, rotation);
            }
        }

        // === Floor ===
        for (int i = 0; i < roomWidth; i++)
        {
            for (int k = 0; k < roomLength; k++)
            {
                Vector3 offset = new Vector3((-roomWidth / 2f) + i + 0.5f, -floorLevel, (-roomLength / 2f) + k + 0.5f);
                Vector3 posFloor = node.center + rotation * offset;
                Instantiate(blockPrefab, posFloor, rotation);
            }
        }

            // === Ceiling ===
        for (int i = 0; i < roomWidth; i++)
        {
            for (int k = 0; k < roomLength; k++)
            {
                Vector3 offset = new Vector3((-roomWidth / 2f) + i + 0.5f, ceilingLevel, (-roomLength / 2f) + k + 0.5f);
                Vector3 posFloor = node.center + rotation * offset;
                Instantiate(blockPrefab, posFloor, rotation);
            }
        }
    }
}