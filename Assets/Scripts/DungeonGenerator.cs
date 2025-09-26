using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DungeonGenerator : MonoBehaviour
{

    public enum DungeonSize
    {
        Small, //50x50
        Medium, //100 x 100
        Large //150 x 150
    }

    public int floorSize;

    [Header("Prefabs")]
    public Room[] dungeonRooms;   // Array of room prefabs to be used in the dungeon
    public Room[] hallways;       // Array of hallway prefabs to be used in the dungeon
    public GameObject nodePrefab;

    [Header("Dungeon Settings")]
    public int dungeonWidth;      // Grid width (not used yet)
    public int dungeonLength;     // Grid length (not used yet)
    public int minRoomWidth;      // For BSP or advanced gen later
    public int minRoomLength;     // For BSP or advanced gen later
    public double maxDungeonRooms;   // Max number of dungeonRooms to be generated
    public int maxTreeLevels;

    [Header("Tracking")]
    public List<GameObject> nodes = new List<GameObject>(); // List of rooms already placed

    void Start()
    {
        CreateDungeon();
    }

    public void CreateDungeon()
    {
        // Step 1: Spawn the starting node / level bounds
        int size = UnityEngine.Random.Range(0, 3);
        switch (size)
        {
            case 0:
                {
                    floorSize = 50;
                    break;
                }

            case 1:
                {
                    floorSize = 100;
                    break;
                }

            case 2:
                {
                    floorSize = 150;
                    break;
                }
        }
        Node starterNode = new Node(floorSize, floorSize, Vector3.zero);
        SplitNode(starterNode, 0); // Start at 2^0 aka 1 node
        CheckIsLeaf(starterNode);
    }

    public void SplitNode(Node node, int numGenerations)
    {
        // 2 to the power of maxTreeLevels
        double maxDungeonRooms = Math.Pow(2, maxTreeLevels);

        if (numGenerations >= maxTreeLevels)
        {
            return;
        }

        if (node.width < minRoomWidth || node.length < minRoomLength)
        {
            return;
        }

        if (nodes.Count >= maxDungeonRooms)
        {
            return;
        }

        int direction = UnityEngine.Random.Range(0, 2);

        float splitPercent = UnityEngine.Random.Range(0.35f, 0.65f);

        if (direction == 0) //Vertical
        {
            float aWidth = node.width * splitPercent;
            float bWidth = node.width - aWidth;

            if (aWidth < minRoomWidth || bWidth < minRoomWidth)
            {
                return;
            }

            float aCenterX = node.center.x - (node.width / 2f - aWidth / 2f);
            float bCenterX = node.center.x + (node.width / 2f - bWidth / 2f);

            node.aChild = new Node(node.length, aWidth, new Vector3(aCenterX, 0, node.center.z));
            node.bChild = new Node(node.length, bWidth, new Vector3(bCenterX, 0, node.center.z));
            node.aChild.isLeaf = true;
            node.bChild.isLeaf = true;
        }

        if (direction == 1) //Horizontal
        {
            float aLength = node.length * splitPercent;
            float bLength = node.length - aLength;

            if (aLength < minRoomLength || bLength < minRoomLength)
            {
                return;
            }

            float aCenterZ = node.center.z - (node.length / 2f - aLength / 2f);
            float bCenterZ = node.center.z + (node.length / 2f - bLength / 2f);

            node.aChild = new Node(aLength, node.width, new Vector3(node.center.x, 0, aCenterZ));
            node.bChild = new Node(bLength, node.width, new Vector3(node.center.x, 0, bCenterZ));
            node.aChild.isLeaf = true;
            node.bChild.isLeaf = true;
        }

        node.isLeaf = false;

        SplitNode(node.aChild, numGenerations + 1);
        SplitNode(node.bChild, numGenerations + 1);
    }

    public void CheckIsLeaf(Node node)
    {
        if (node.isLeaf)
        {
            SpawnCollider(node);
        }

        else
        {
            CheckIsLeaf(node.aChild);
            CheckIsLeaf(node.bChild);
        }
    }

    public void SpawnCollider(Node node)
    {
        GameObject nodeObject = Instantiate(nodePrefab, node.center, Quaternion.identity);
        BoxCollider boxC = nodeObject.GetComponent<BoxCollider>();
        boxC.size = new Vector3(node.width, 1, node.length);
        nodes.Add(nodeObject);
    }
}

//UnityEngine namespace used for Random.Range to differ from System.Random
//System included to use a 2^ in SplitNode()

