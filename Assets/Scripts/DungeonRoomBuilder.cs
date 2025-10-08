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

        float excessWidth = width - roomWidth;
        float excessLength = length - roomLength;

        Vector3 cornerA = new Vector3(-width / 2, 0, -length / 2);
        Vector3 cornerB = new Vector3(width / 2, 0, -length / 2);
        Vector3 cornerC = new Vector3(-width / 2, 0, length / 2);
        Vector3 cornerD = new Vector3(width / 2, 0, length / 2);


        for (int i = 0; i < roomWidth; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Vector3 posWallA = node.center + new Vector3((-width / 2) + i + (excessWidth / 2), j, roomWidth / 2 + (excessLength / 2));
                Instantiate(blockPrefab, posWallA, Quaternion.identity);

                Vector3 posWallB = node.center + new Vector3((-width / 2) + i + (excessWidth / 2), j, -roomWidth / 2 - (excessLength / 2));   // mirrored Z
                Instantiate(blockPrefab, posWallB, Quaternion.identity);

            }
            
        }
    }
}