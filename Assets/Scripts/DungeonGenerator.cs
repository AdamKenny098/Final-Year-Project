using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using DelaunatorSharp;

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
    public int minRoomWidth;      // For BSP or advanced gen later
    public int minRoomLength;     // For BSP or advanced gen later
    public int maxTreeLevels;

    public float gap;

    [Header("Tracking")]
    public List<Vector3> nodeCenters = new List<Vector3>(); // List of rooms already placed
    public List<IPoint> roomCenters = new List<IPoint>();
    void Start()
    {
        CreateDungeon();
    }

    public Node CreateDungeon()
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

        ConvertRoomCenters();
        Debug.Log("Number of Rooms: " + roomCenters.Count);

        RunDelaunator();

        return starterNode;
    }

    public void SplitNode(Node node, int numGenerations)
    {
        // 2 to the power of maxTreeLevels
        double maxDungeonRooms = Math.Pow(2, maxTreeLevels);

        if (numGenerations >= maxTreeLevels) return;
        if (node.width < minRoomWidth || node.length < minRoomLength) return;
        if (nodeCenters.Count >= maxDungeonRooms) return;

        int direction = UnityEngine.Random.Range(0, 2);
        if (node.width > node.length) direction = 0;
        else direction = 1;

        float splitPercent = UnityEngine.Random.Range(0.35f, 0.65f);

        if (direction == 0) //Vertical
        {
            float aWidth = node.width * splitPercent;
            float bWidth = node.width - aWidth;

            if (aWidth < minRoomWidth || bWidth < minRoomWidth) return;

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

            if (aLength < minRoomWidth || bLength < minRoomWidth) return;

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

    }

    public void SpawnCollider(Node node)
    {
        GameObject nodeObject = Instantiate(nodePrefab, node.center, Quaternion.identity);
        BoxCollider boxC = nodeObject.GetComponent<BoxCollider>();
        boxC.size = new Vector3(node.width, 1, node.length);

        node.roomCenter = boxC.bounds.center;
        nodeCenters.Add(node.roomCenter);
    }

    public void ConvertRoomCenters()
    {
        foreach (Vector3 v in nodeCenters)
        {
            roomCenters.Add(new Point(v.x, v.z));
        }
    }

    public void RunDelaunator()
    {
        Delaunator delaunator = new Delaunator(roomCenters.ToArray());

        foreach (var edge in delaunator.GetEdges())
        {
            //https://github.com/nol1fe/delaunator-sharp/blob/master/DelaunatorSharp/Interfaces/IEdge.cs
            //Converted to float due to RoomCenter being a Vector3 of flaots
            // Since is Vector2 and RoomCenter.Y is negligible we pass RoomCenter.Z as the Y value
            Vector2 from = new Vector2((float)edge.P.X, (float)edge.P.Y);
            Vector2 to = new Vector2((float)edge.Q.X, (float)edge.Q.Y);

            Debug.DrawLine(new Vector3(from.x, 0, from.y), new Vector3(to.x, 0, to.y), Color.red);
        }
    }

}

//UnityEngine namespace used for Random.Range to differ from System.Random
//System included to use a 2^ in SplitNode()
