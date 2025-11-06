using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DunegonRoomDecorator : MonoBehaviour
{

    public GameObject pillarPrefab;
    public GameObject torchPrefab;
    public GameObject torchPillarPrefab;

    [Range(0, 5)] public float torchHeight = 3.5f;
    [Range(-3, 4)] public float wallInset = 1f;

    public List<GenericRoomItem> genericRoomItems = new List<GenericRoomItem>();
    public DungeonRoomBuilder dungeonRoomBuilder;

    void Awake()
    {
        genericRoomItems.Add(new GenericRoomItem()
        {
            name = "Torch",
            prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Generic/Torch"),
            type = GenericRoomItem.GenericType.Torch
        });

        genericRoomItems.Add(new GenericRoomItem()
        {
            name = "Pillar",
            prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Generic/Pillar"),
            type = GenericRoomItem.GenericType.Pillar
        });

        genericRoomItems.Add(new GenericRoomItem()
        {
            name = "TorchPillar",
            prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Generic/TorchPillar"),
            type = GenericRoomItem.GenericType.TorchPillar
        });

        genericRoomItems.Add(new GenericRoomItem()
        {
            name = "Banner",
            prefab = Resources.Load<GameObject>("Prefabs/RoomItems/Generic/Banner"),
            type = GenericRoomItem.GenericType.Banner
        });
    }

    public void StartDecorationProcess()
    {
        foreach (Room room in dungeonRoomBuilder.allRooms)
        {
            DecorateRoomGenerically(room);
        }
    }

    public void DecorateRoomGenerically(Room room)
    {
        if (room.isCorridor) return;

        Vector3 roomCenter = room.transform.position;
        float roomWidth = room.node.width;
        float roomLength = room.node.length;

        Quaternion rot = room.transform.rotation;

        GeneratePillars(room, rot, roomCenter, roomWidth, roomLength);
        PlaceTorches(room, rot, roomCenter, roomWidth, roomLength);
        ReplacePillarsWithTorchPillars(room);

        DeleteRandomGenericDecor(room);

    }

    public void DecorateRoom(Room room)
    {
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

    void GeneratePillars(Room room, Quaternion rot, Vector3 roomCenter, float roomWidth, float roomLength)
    {
        float pillarSpacing = Random.Range(5f, 10f);
        pillarSpacing = Mathf.Floor(pillarSpacing);

        GameObject pillarParent = new GameObject("Pillars");
        pillarParent.transform.SetParent(room.transform);
        pillarParent.transform.localPosition = Vector3.zero;
        pillarParent.transform.localRotation = Quaternion.identity;


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

            Instantiate(pillarPrefab, posA, rot, pillarParent.transform);
            Instantiate(pillarPrefab, posB, rot, pillarParent.transform);
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

            Instantiate(pillarPrefab, posC, rot, pillarParent.transform);
            Instantiate(pillarPrefab, posD, rot, pillarParent.transform);
        }
    }

    void PlaceTorches(Room room, Quaternion rot, Vector3 roomCenter, float roomWidth, float roomLength)
    {
        if (room.isCorridor) return;

        GameObject torchParent = new GameObject("Torches");
        torchParent.transform.SetParent(room.transform);
        torchParent.transform.localPosition = Vector3.zero;
        torchParent.transform.localRotation = Quaternion.identity;

        // === Torch placement parameters ===

        float spacing = Mathf.Clamp(Random.Range(6f, 10f), 4f, 10f); // room-dependent spacing

        // === North/South walls ===
        for (float i = spacing / 2; i < roomWidth - spacing / 2; i += spacing)
        {
            Vector3 northOffset = new Vector3(-roomWidth / 2f + i, torchHeight, roomLength / 2f - wallInset);
            Vector3 southOffset = new Vector3(-roomWidth / 2f + i, torchHeight, -roomLength / 2f + wallInset);

            Vector3 northPos = roomCenter + rot * northOffset;
            Vector3 southPos = roomCenter + rot * southOffset;

            Quaternion northRot = rot * Quaternion.Euler(0, 180, 0);
            Quaternion southRot = rot;

            Instantiate(torchPrefab, northPos, northRot, torchParent.transform);
            Instantiate(torchPrefab, southPos, southRot, torchParent.transform);
        }

        // === East/West walls ===
        for (float k = spacing / 2; k < roomLength - spacing / 2; k += spacing)
        {
            Vector3 eastOffset = new Vector3(roomWidth / 2f - wallInset, torchHeight, -roomLength / 2f + k);
            Vector3 westOffset = new Vector3(-roomWidth / 2f + wallInset, torchHeight, -roomLength / 2f + k);

            Vector3 eastPos = roomCenter + rot * eastOffset;
            Vector3 westPos = roomCenter + rot * westOffset;

            Quaternion eastRot = rot * Quaternion.Euler(0, 270, 0);
            Quaternion westRot = rot * Quaternion.Euler(0, 90, 0);

            Instantiate(torchPrefab, eastPos, eastRot, torchParent.transform);
            Instantiate(torchPrefab, westPos, westRot, torchParent.transform);
        }
    }

    void ReplacePillarsWithTorchPillars(Room room)
    {
        Transform pillarParent = room.transform.Find("Pillars");

        List<Transform> pillarsToReplace = new List<Transform>();

        foreach (Transform pillar in pillarParent)
        {
            float replaceChance = Random.Range(0f, 1f);
            if (replaceChance <= 0.3f) // 30% chance to replace pillar with torch pillar
            {
                pillarsToReplace.Add(pillar);
            }
        }

        foreach (Transform pillar in pillarsToReplace)
        {
            Vector3 position = pillar.position;
            Quaternion rotation = pillar.rotation;
            Destroy(pillar.gameObject);
            Instantiate(torchPillarPrefab, position, rotation, pillarParent);
        }
    }

    void DeleteRandomGenericDecor(Room room)
    {
        GameObject pillars = room.transform.Find("Pillars").gameObject;
        GameObject torches = room.transform.Find("Torches").gameObject;

        float deleteChance = Random.Range(0f, 1f);
        for(int i = 0; i < pillars.transform.childCount; i++)
        {
            Transform child = pillars.transform.GetChild(i);
            float chance = Random.Range(0f, 1f);
            if (chance < deleteChance)
            {
                Destroy(child.gameObject);
            }
        }
        
        for(int i = 0; i < torches.transform.childCount; i++)
        {
            Transform child = torches.transform.GetChild(i);
            float chance = Random.Range(0f, 1f);
            if (chance < deleteChance)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
