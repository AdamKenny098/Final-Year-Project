using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DungeonGenerator : MonoBehaviour
{

    public int smallFloorSize;
    public int mediumFloorSize;
    public int largeFloorSize;


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

    public float gap;

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
                    floorSize = smallFloorSize;
                    break;
                }

            case 1:
                {
                    floorSize = mediumFloorSize;
                    break;
                }

            case 2:
                {
                    floorSize = largeFloorSize;
                    break;
                }
        }
        Node starterNode = new Node(floorSize, floorSize, Vector3.zero);
        SplitNode(starterNode, 0); // Start at 2^0 aka 1 node
        CheckIsLeaf(starterNode);

        SpawnCorridor(starterNode);
        ConnectRooms(starterNode);
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

        if (node.width > node.length)
        {
            direction = 0;
        }

        else
        {
            direction = 1;
        }

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

            node.aChild = new Node(node.length - gap, aWidth - gap, new Vector3(aCenterX, 0, node.center.z));
            node.bChild = new Node(node.length - gap, bWidth - gap, new Vector3(bCenterX, 0, node.center.z));
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

            node.aChild = new Node(aLength - gap, node.width - gap, new Vector3(node.center.x, 0, aCenterZ));
            node.bChild = new Node(bLength - gap, node.width - gap, new Vector3(node.center.x, 0, bCenterZ));
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

        if (node.aChild != null && node.bChild != null)
        {
            ConnectNode(node);
        }

    }

    public void SpawnCollider(Node node)
    {
        GameObject nodeObject = Instantiate(nodePrefab, node.center, Quaternion.identity);
        BoxCollider boxC = nodeObject.GetComponent<BoxCollider>();
        boxC.size = new Vector3(node.width, 1, node.length);
        nodes.Add(nodeObject);
    }

    public void ConnectNode(Node node)
    {
        if (node.aChild == null || node.bChild == null) return;

        Vector3 centerA = node.aChild.roomCenter;
        Vector3 centerB = node.bChild.roomCenter;

        // Start door positions at the centers
        Vector3 doorA = centerA;
        Vector3 doorB = centerB;

        // Directional differences
        float dx = centerB.x - centerA.x;
        float dz = centerB.z - centerA.z;

        // Horizontal corridor (rooms are left/right relative to each other)
        if (Mathf.Abs(dx) > Mathf.Abs(dz))
        {
            if (dx > 0)
            {
                // B is to the right of A
                doorA.x = centerA.x + (node.aChild.width / 2f);
                doorB.x = centerB.x - (node.bChild.width / 2f);
            }
            else
            {
                // B is to the left of A
                doorA.x = centerA.x - (node.aChild.width / 2f);
                doorB.x = centerB.x + (node.bChild.width / 2f);
            }

            // Corridor center & size
            Vector3 corridorCenter = new Vector3((doorA.x + doorB.x) / 2f, 0, doorA.z);
            float corridorLength = Mathf.Abs(doorB.x - doorA.x);

            GameObject corridor = Instantiate(nodePrefab, corridorCenter, Quaternion.identity);
            BoxCollider boxC = corridor.GetComponent<BoxCollider>();
            boxC.size = new Vector3(corridorLength, 1f, 5f); // 5 units thick
        }
        // Vertical corridor (rooms are above/below relative to each other)
        else
        {
            if (dz > 0)
            {
                // B is above A
                doorA.z = centerA.z + (node.aChild.length / 2f);
                doorB.z = centerB.z - (node.bChild.length / 2f);
            }
            else
            {
                // B is below A
                doorA.z = centerA.z - (node.aChild.length / 2f);
                doorB.z = centerB.z + (node.bChild.length / 2f);
            }

            // Corridor center & size
            Vector3 corridorCenter = new Vector3(doorA.x, 0, (doorA.z + doorB.z) / 2f);
            float corridorLength = Mathf.Abs(doorB.z - doorA.z);

            GameObject corridor = Instantiate(nodePrefab, corridorCenter, Quaternion.identity);
            BoxCollider boxC = corridor.GetComponent<BoxCollider>();
            boxC.size = new Vector3(5f, 1f, corridorLength); // 5 units thick
        }
    }


    public void SpawnCorridor(Node node)
    {
        if (node == null) return;

        if (node.aChild != null && node.bChild != null)
        {
            ConnectNode(node);

            SpawnCorridor(node.aChild);
            SpawnCorridor(node.bChild);
        }

        else return;
    }

    public Vector3 ConnectRooms(Node node)
    {
        if (node == null)
        {
            return Vector3.zero;
        }

        if (node.isLeaf)
        {
            node.roomCenter = node.center;
            return node.roomCenter;
        }

        Vector3 centerA = ConnectRooms(node.aChild);
        Vector3 centerB = ConnectRooms(node.bChild);

        ConnectNode(node);

        int choice = UnityEngine.Random.Range(0, 2);

        if (choice == 0)
        {
            node.roomCenter = centerA;
            return node.roomCenter;
        }

        else
        {
            node.roomCenter = centerB;
            return node.roomCenter;
        }

    }

}

//UnityEngine namespace used for Random.Range to differ from System.Random
//System included to use a 2^ in SplitNode()

