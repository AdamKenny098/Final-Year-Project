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
    public List<Node> leafNodes = new List<Node>();

    public DungeonRoomBuilder dungeonRoomBuilder;

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

        nodeCenters.Clear();
        roomCenters.Clear();
        leafNodes.Clear();

        SplitNode(starterNode, 0);
        CheckIsLeaf(starterNode);

        BuildLeafNodes();
        BuildStraightConnections();

        dungeonRoomBuilder.CarveDoorways();
        dungeonRoomBuilder.StartBuildProcess();

        return starterNode;
    }

    public void SplitNode(Node node, int numGenerations)
{
    if (numGenerations >= maxTreeLevels) return;
    if (node.width < minRoomWidth || node.length < minRoomLength) return;

    int direction = UnityEngine.Random.Range(0, 2);
    if (node.width > node.length) direction = 0;
    else direction = 1;

    float splitPercent = UnityEngine.Random.Range(0.35f, 0.65f);

    if (direction == 0) // Vertical
    {
        float aWidth = node.width * splitPercent;
        float bWidth = node.width - aWidth;

        float rawChildLength = node.length - gap;
        float rawChildAWidth = aWidth - gap;
        float rawChildBWidth = bWidth - gap;

        float childLength = SnapOddSize(rawChildLength);
        float childAWidth = SnapOddSize(rawChildAWidth);
        float childBWidth = SnapOddSize(rawChildBWidth);

        if (childAWidth < minRoomWidth || childBWidth < minRoomWidth || childLength < minRoomLength)
            return;

        float aCenterX = node.center.x - (node.width / 2f - aWidth / 2f);
        float bCenterX = node.center.x + (node.width / 2f - bWidth / 2f);

        node.aChild = new Node(childLength, childAWidth, new Vector3(SnapWhole(aCenterX), 0f, SnapWhole(node.center.z)));

        node.bChild = new Node(childLength, childBWidth, new Vector3(SnapWhole(bCenterX), 0f, SnapWhole(node.center.z)));

        node.aChild.isLeaf = true;
        node.bChild.isLeaf = true;
    }

    if (direction == 1) // Horizontal
    {
        float aLength = node.length * splitPercent;
        float bLength = node.length - aLength;

        float rawChildALength = aLength - gap;
        float rawChildBLength = bLength - gap;
        float rawChildWidth = node.width - gap;

        float childALength = SnapOddSize(rawChildALength);
        float childBLength = SnapOddSize(rawChildBLength);
        float childWidth = SnapOddSize(rawChildWidth);

        if (childALength < minRoomLength || childBLength < minRoomLength || childWidth < minRoomWidth)
            return;

        float aCenterZ = node.center.z - (node.length / 2f - aLength / 2f);
        float bCenterZ = node.center.z + (node.length / 2f - bLength / 2f);

        node.aChild = new Node(childALength, childWidth, new Vector3(SnapWhole(node.center.x), 0f, SnapWhole(aCenterZ)));

        node.bChild = new Node(childBLength, childWidth, new Vector3(SnapWhole(node.center.x), 0f, SnapWhole(bCenterZ)));

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
            leafNodes.Add(node);
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
        nodeObject.tag = "Node";
        BoxCollider boxC = nodeObject.GetComponent<BoxCollider>();
        boxC.size = new Vector3(node.width, gap / 2, node.length);
        nodeObject.transform.localScale = boxC.size;
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

    public List<IEdge> RunDelaunator()
    {
        Delaunator delaunator = new Delaunator(roomCenters.ToArray());

        List<IEdge> edges = new List<IEdge>(delaunator.GetEdges());

        foreach (var edge in delaunator.GetEdges())
        {
            //https://github.com/nol1fe/delaunator-sharp/blob/master/DelaunatorSharp/Interfaces/IEdge.cs
            //Converted to float due to RoomCenter being a Vector3 of flaots
            // Since is Vector2 and RoomCenter.Y is negligible we pass RoomCenter.Z as the Y value
            Vector2 from = new Vector2((float)edge.P.X, (float)edge.P.Y);
            Vector2 to = new Vector2((float)edge.Q.X, (float)edge.Q.Y);

            //Debug.DrawLine(new Vector3(from.x, 0, from.y), new Vector3(to.x, 0, to.y), Color.red, 1000f);
        }
        return edges;
    }

    public List<IEdge> RunKruskals(List<IEdge> edges)
    {
        KruskalsAlgorithm kruskal = new KruskalsAlgorithm();
        kruskal.Vertices = roomCenters.Count;

        List<IEdge> mstEdges = kruskal.Kruskal(roomCenters.Count, edges, roomCenters);

        // Debug: show MST edges in yellow
        foreach (var edge in mstEdges)
        {
            Vector3 from = new Vector3((float)edge.P.X, 0, (float)edge.P.Y);
            Vector3 to = new Vector3((float)edge.Q.X, 0, (float)edge.Q.Y);

            SpawnCorridor(from, to);
            //Debug.DrawLine(from, to, Color.yellow, 1000f); // MST edges
        }

        return mstEdges;
    }

    void SpawnCorridor(Vector3 a, Vector3 b)
    {
        Vector3 corridorCenter = (a + b) / 2f;

        //goes to b from a w corridorCenter in the middle
        Vector3 distance = b - a;
        float length = distance.magnitude;

        GameObject corridor = Instantiate(nodePrefab, corridorCenter, Quaternion.identity);
        corridor.tag = "Node";
        corridor.transform.localScale = new Vector3(gap / 2, gap / 2 - 1, length);

        // rotate corridor along direction of distance (z)
        corridor.transform.rotation = Quaternion.LookRotation(distance);

        // update collider (so it matches new scale)
        BoxCollider boxC = corridor.GetComponent<BoxCollider>();
        boxC.size = Vector3.one; // reset collider to match 1×1×1 scaled object

        Node node = new Node(length, gap / 2, corridorCenter);
        dungeonRoomBuilder.BuildRoom(node, corridor.transform.eulerAngles.y, true, Room.RoomType.Default);
    }

    void BuildLeafNodes()
    {
        foreach (var node in leafNodes)
        {
            dungeonRoomBuilder.BuildRoom(node, 0f, false, Room.RoomType.Default);
        }
    }

    
    public class StraightConnection
    {
        public Node nodeA;
        public Node nodeB;
        public bool vertical;
        public Vector3 start;
        public Vector3 end;
        public float distance;
    }

    public int BuiltWidth(Node node)
    {
        return Mathf.FloorToInt(node.width);
    }

    public int BuiltLength(Node node)
    {
        return Mathf.FloorToInt(node.length);
    }

    public float MinX(Node node) => node.roomCenter.x - BuiltWidth(node) / 2f;
    public float MaxX(Node node) => node.roomCenter.x + BuiltWidth(node) / 2f;
    public float MinZ(Node node) => node.roomCenter.z - BuiltLength(node) / 2f;
    public float MaxZ(Node node) => node.roomCenter.z + BuiltLength(node) / 2f;

    public bool TryCreateStraightConnection(Node nodeA, Node nodeB, out StraightConnection connection)
    {
        connection = null;

        float overlapMinX = Mathf.Max(MinX(nodeA), MinX(nodeB));
        float overlapMaxX = Mathf.Min(MaxX(nodeA), MaxX(nodeB));

        float overlapMinZ = Mathf.Max(MinZ(nodeA), MinZ(nodeB));
        float overlapMaxZ = Mathf.Min(MaxZ(nodeA), MaxZ(nodeB));

        // Keep corridors away from room corners by shrinking the usable overlap band.
        // 2f = 2 + center + 2
        float edgeMargin = 2f;

        float usableMinX = overlapMinX + edgeMargin;
        float usableMaxX = overlapMaxX - edgeMargin;

        float usableMinZ = overlapMinZ + edgeMargin;
        float usableMaxZ = overlapMaxZ - edgeMargin;

        bool canVertical = usableMaxX - usableMinX >= 0f;
        bool canHorizontal = usableMaxZ - usableMinZ >= 0f;

        if (!canVertical && !canHorizontal)
            return false;

        if (canVertical)
        {
            float x = SnapWhole((usableMinX + usableMaxX) * 0.5f);

            bool bAbove = nodeB.roomCenter.z > nodeA.roomCenter.z;

            float startZ = bAbove ? MaxZ(nodeA) - 0.5f : MinZ(nodeA) + 0.5f;
            float endZ   = bAbove ? MinZ(nodeB) + 0.5f : MaxZ(nodeB) - 0.5f;

            Vector3 start = new Vector3(x, 0f, startZ);
            Vector3 end   = new Vector3(x, 0f, endZ);

            float dist = Mathf.Abs(end.z - start.z);
            if (dist < 1f) return false;

            connection = new StraightConnection
            {
                nodeA = nodeA,
                nodeB = nodeB,
                vertical = true,
                start = start,
                end = end,
                distance = dist
            };

            return true;
        }

        if (canHorizontal)
        {
            float z = SnapWhole((usableMinZ + usableMaxZ) * 0.5f);

            bool bRight = nodeB.roomCenter.x > nodeA.roomCenter.x;

            float startX = bRight ? MaxX(nodeA) - 0.5f : MinX(nodeA) + 0.5f;
            float endX   = bRight ? MinX(nodeB) + 0.5f : MaxX(nodeB) - 0.5f;

            Vector3 start = new Vector3(startX, 0f, z);
            Vector3 end   = new Vector3(endX, 0f, z);

            float dist = Mathf.Abs(end.x - start.x);
            if (dist < 1f) return false;

            connection = new StraightConnection
            {
                nodeA = nodeA,
                nodeB = nodeB,
                vertical = false,
                start = start,
                end = end,
                distance = dist
            };

            return true;
        }

        return false;
    }

    public List<StraightConnection> GetAllValidStraightConnections()
    {
        List<StraightConnection> connections = new List<StraightConnection>();

        for (int i = 0; i < leafNodes.Count; i++)
        {
            for (int j = i + 1; j < leafNodes.Count; j++)
            {
                if (TryCreateStraightConnection(leafNodes[i], leafNodes[j], out StraightConnection connection))
                {
                    connections.Add(connection);
                }
            }
        }

        connections.Sort((a, b) => a.distance.CompareTo(b.distance));
        return connections;
    }

    public void BuildStraightConnections()
    {
        List<StraightConnection> candidates = GetAllValidStraightConnections();
        if (candidates.Count == 0)
        {
            return;
        }

        Dictionary<Node, int> indexMap = new Dictionary<Node, int>();
        for (int i = 0; i < leafNodes.Count; i++)
            indexMap[leafNodes[i]] = i;

        int[] parent = new int[leafNodes.Count];
        for (int i = 0; i < parent.Length; i++)
            parent[i] = i;

        int Find(int x)
        {
            while (parent[x] != x)
                x = parent[x];
            return x;
        }

        void Union(int a, int b)
        {
            int rootA = Find(a);
            int rootB = Find(b);
            parent[rootA] = rootB;
        }

        int edgesUsed = 0;

        foreach (StraightConnection connection in candidates)
        {
            int aIndex = indexMap[connection.nodeA];
            int bIndex = indexMap[connection.nodeB];

            if (Find(aIndex) == Find(bIndex))
                continue;

            SpawnStraightCorridor(connection);
            RegisterDoorway(connection.nodeA, connection.start);
            RegisterDoorway(connection.nodeB, connection.end);

            Union(aIndex, bIndex);
            edgesUsed++;

            if (edgesUsed >= leafNodes.Count - 1)
                break;
        }
    }

    public void SpawnStraightCorridor(StraightConnection connection)
    {
        SpawnCorridorSegment(connection.start, connection.end);
    }

    public void SpawnCorridorSegment(Vector3 start, Vector3 end)
    {
        Vector3 delta = end - start;

        bool alongX = Mathf.Abs(delta.x) > Mathf.Abs(delta.z);
        float logicalLength = alongX ? Mathf.Abs(delta.x) : Mathf.Abs(delta.z);

        // Corridor is currently 1 voxel too long overall
        int builtLength = Mathf.RoundToInt(logicalLength - 1f);
        if (builtLength < 1)
            return;

        Vector3 center = (start + end) * 0.5f;

        float rotation = alongX ? 90f : 0f;
        int shellWidth = 5;

        Node corridorNode = new Node(builtLength, shellWidth, center);
        dungeonRoomBuilder.BuildRoom(corridorNode, rotation, true, Room.RoomType.Default);
    }

    public Room FindBuiltRoomByNode(Node targetNode)
    {
        foreach (Room room in dungeonRoomBuilder.allRooms)
        {
            if (!room.isCorridor && room.node == targetNode)
                return room;
        }

        return null;
    }

    public void RegisterDoorway(Node node, Vector3 doorwayPos)
    {
        Room room = FindBuiltRoomByNode(node);
        if (room != null)
        {
            Vector3 snapped = new Vector3(SnapWhole(doorwayPos.x), 0f, SnapWhole(doorwayPos.z));
            room.plannedDoorwayPositions.Add(snapped);
        }
    }

    public float SnapWhole(float v)
    {
        return Mathf.Round(v);
    }

    public float SnapOddSize(float value)
    {
        int v = Mathf.Max(3, Mathf.RoundToInt(value));

        if (v % 2 == 0)
            v -= 1;

        return v;
    }
}

//UnityEngine namespace used for Random.Range to differ from System.Random
//System included to use a 2^ in SplitNode()