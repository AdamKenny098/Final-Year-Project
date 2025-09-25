using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{

    public enum DungeonSize
    {
        Small, //50x50
        Medium, //100 x 100
        Large //150 x 150
    }

    public Vector3 floorSize;

    [Header("Prefabs")]
    public Room[] dungeonRooms;   // Array of room prefabs to be used in the dungeon
    public Room[] hallways;       // Array of hallway prefabs to be used in the dungeon
    public GameObject nodePrefab;

    [Header("Dungeon Settings")]
    public int dungeonWidth;      // Grid width (not used yet)
    public int dungeonLength;     // Grid length (not used yet)
    public int minRoomWidth;      // For BSP or advanced gen later
    public int minRoomLength;     // For BSP or advanced gen later
    public int maxDungeonRooms;   // Max number of dungeonRooms to be generated

    [Header("Tracking")]
    public List<GameObject> nodes = new List<GameObject>(); // List of rooms already placed

    void Start()
    {
        CreateDungeon();
    }

    public void CreateDungeon()
    {
        // Step 1: Spawn the starting node / level bounds
        int size = Random.Range(0, 3);
        switch (size)
        {
            case 0:
                {
                    floorSize = new Vector3(50, 1, 50);
                    break;
                }

            case 1:
                {
                    floorSize = new Vector3(100, 1, 100);
                    break;
                }

            case 2:
                {
                    floorSize = new Vector3(150, 1, 150);
                    break;
                }
        }
        GameObject starterNode = Instantiate(nodePrefab, Vector3.zero, Quaternion.identity);
        BoxCollider boxC = starterNode.GetComponent<BoxCollider>();
        boxC.size = floorSize;
        nodes.Add(starterNode);

        //The center for starter node
        Transform nodeCenter = nodes[0].transform;
        Bounds bounds = nodes[0].GetComponent<BoxCollider>().bounds;
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;

        // Step 2: Attempt to split first node.
        int direction = Random.Range(0, 2);
        if (direction == 0) //Vertical
        {
            float splitPercent = Random.Range(0.1f, .85f);
            //Get x axis somehow?
            float width = bounds.size.x;
            float depth = bounds.size.z;

            //split in two
            float aWidth = width * splitPercent;
            float bWidth = width - aWidth;

            float aCenter = (center.x - extents.x) + (aWidth / 2);
            float bCenter = (center.x + extents.x) - (bWidth / 2);

            GameObject childA = Instantiate(nodePrefab, new Vector3(aCenter, 0, 0), Quaternion.identity);
            GameObject childB = Instantiate(nodePrefab, new Vector3(bCenter, 0, 0), Quaternion.identity);

            BoxCollider boxA = childA.GetComponent<BoxCollider>();
            boxA.size = new Vector3(aWidth, 0, depth);

            BoxCollider boxB = childB.GetComponent<BoxCollider>();
            boxB.size = new Vector3(bWidth, 0, depth);



        }

        else if (direction == 1)
        {
            //Vertical
            float splitPercent = Random.Range(0.1f, .85f);
            //Get x axis somehow?
            float width = bounds.size.x;
            float depth = bounds.size.z;

            //split in two
            float aWidth = width * splitPercent;
            float bWidth = width - aWidth;

            float aCenter = (center.x - extents.x) + (aWidth / 2);
            float bCenter = (center.x + extents.x) - (bWidth / 2);

            GameObject childA = Instantiate(nodePrefab, new Vector3(aCenter, 0, 0), Quaternion.identity);
            GameObject childB = Instantiate(nodePrefab, new Vector3(bCenter, 0, 0), Quaternion.identity);

            BoxCollider boxA = childA.GetComponent<BoxCollider>();
            boxA.size = new Vector3(aWidth, 0, depth);
            
            BoxCollider boxB = childB.GetComponent<BoxCollider>();
            boxB.size = new Vector3(bWidth, 0 , depth);

        }
    }
}
