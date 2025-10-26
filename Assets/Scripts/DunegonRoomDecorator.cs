using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DunegonRoomDecorator : MonoBehaviour
{

    public GameObject pillarPrefab;

    public List< GenericRoomItem> genericRoomItems = new List< GenericRoomItem>();
    public DungeonRoomBuilder dungeonRoomBuilder;

    void Awake()
    {
        genericRoomItems.Add(new  GenericRoomItem()
        {
            name = "Torch",
            prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Generic/Torch"),
            type =  GenericRoomItem.GenericType.Torch
        });

        genericRoomItems.Add(new  GenericRoomItem()
        {
            name = "Pillar",
            prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Generic/Pillar"),
            type =  GenericRoomItem.GenericType.Pillar
        });

        genericRoomItems.Add(new  GenericRoomItem()
        {
            name = "TorchPillar",
            prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Generic/TorchPillar"),
            type =  GenericRoomItem.GenericType.TorchPillar
        });

        genericRoomItems.Add(new  GenericRoomItem()
        {
            name = "Banner",
            prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Generic/Banner"),
            type =  GenericRoomItem.GenericType.Banner
        });
    }

    public void DecorateRoomGenerically(Room room)
    {
        if (room.isCorridor) return;

        Vector3 roomCenter = room.transform.position;
        float roomWidth = room.node.width;
        float roomLength = room.node.length;

        Quaternion rot = room.transform.rotation;

        // === Pillars ===
        float pillarSpacing = Random.Range(5f, 10f);
        pillarSpacing = Mathf.Floor(pillarSpacing); ;

        for (float i = 0; i < roomWidth - pillarSpacing; i += pillarSpacing)
        {
            Vector3 offsetA = new Vector3((-roomWidth / 2f) + i + 0.5f + pillarSpacing, 2, roomLength / 2f - 0.5f - pillarSpacing);
            Vector3 offsetB = new Vector3((-roomWidth / 2f) + i + 0.5f + pillarSpacing, 2, -roomLength / 2f + 0.5f + pillarSpacing);

            offsetA.x = Mathf.Floor(offsetA.x);
            offsetA.z = Mathf.Floor(offsetA.z);
            offsetB.x = Mathf.Floor(offsetB.x);
            offsetB.z = Mathf.Floor(offsetB.z);

            Vector3 posA = roomCenter + rot * offsetA;
            Vector3 posB = roomCenter + rot * offsetB;

            Instantiate(pillarPrefab, posA, rot, room.transform);
            Instantiate(pillarPrefab, posB, rot, room.transform);
        }

        for (float k = 0; k < roomLength - pillarSpacing; k += pillarSpacing)
        {
            Vector3 offsetC = new Vector3(roomWidth / 2f - 0.5f - pillarSpacing, 2, (-roomLength / 2f) + k + 0.5f + pillarSpacing);
            Vector3 offsetD = new Vector3(-roomWidth / 2f + 0.5f + pillarSpacing, 2, (-roomLength / 2f) + k + 0.5f + pillarSpacing);

            offsetC.x = Mathf.Floor(offsetC.x);
            offsetC.z = Mathf.Floor(offsetC.z);
            offsetD.x = Mathf.Floor(offsetD.x);
            offsetD.z = Mathf.Floor(offsetD.z);

            Vector3 posC = roomCenter + rot * offsetC;
            Vector3 posD = roomCenter + rot * offsetD;

            Instantiate(pillarPrefab, posC, rot, room.transform);
            Instantiate(pillarPrefab, posD, rot, room.transform);
        }
    }

    public void DecorateRoom(Room room)
    {
        DecorateRoomGenerically(room);

        float availableRoomArea = room.roomArea * Random.Range(0.3f, 0.6f);

        availableRoomArea = Mathf.Floor(availableRoomArea);
        room.availableArea = availableRoomArea;
    }

    public Vector3 RandomRoomPosition(Room room)
    {
        Vector3 roomCenter = room.node.center;
        float roomWidth = room.node.width;
        float roomLength = room.node.length;

        float randomX = Random.Range(roomCenter.x - roomWidth / 2f + 1f, roomCenter.x + roomWidth / 2f - 1f);
        float randomZ = Random.Range(roomCenter.z - roomLength / 2f + 1f, roomCenter.z + roomLength / 2f - 1f);

        return new Vector3(randomX, roomCenter.y, randomZ);
    }
}
