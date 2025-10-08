using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoomBuilder : MonoBehaviour
{
    public GameObject blockPrefab;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BuildRoom(Node node)
    {
        if (node == null) return;

        //Node is passed from DungeonGenerator
        float width = node.width;
        float length = node.length;

        int roomWidth = Mathf.FloorToInt(width);
        int roomLength = Mathf.FloorToInt(length);

        for (int i = 0; i < roomWidth; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Vector3 posWallA = node.center + new Vector3((-roomWidth / 2f) + i + 0.5f, j, roomLength / 2f - 0.5f);
                Instantiate(blockPrefab, posWallA, Quaternion.identity);

                Vector3 posWallB = node.center + new Vector3((-roomWidth / 2f) + i + 0.5f, j, -roomLength / 2f + 0.5f);
                Instantiate(blockPrefab, posWallB, Quaternion.identity);
            }
        }

        for (int k = 0; k < roomLength; k++)
        {
            for (int j = 0; j < 4; j++)
            {
                Vector3 posWallC = node.center + new Vector3(roomWidth / 2f - 0.5f, j, (-roomLength / 2f) + k + 0.5f);
                Instantiate(blockPrefab, posWallC, Quaternion.identity);

                Vector3 posWallD = node.center + new Vector3(-roomWidth / 2f + 0.5f, j,(-roomLength / 2f) + k + 0.5f);
                Instantiate(blockPrefab, posWallD, Quaternion.identity);
            }
        }

    }
}