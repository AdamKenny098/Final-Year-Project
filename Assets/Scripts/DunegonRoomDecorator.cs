using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DunegonRoomDecorator : MonoBehaviour
{
    public void DecorateRoom(Room room)
    {
        float totalRoomArea = room.roomArea;
        float availableRoomArea = totalRoomArea * Random.Range(0.3f, 0.6f);

        availableRoomArea = Mathf.Floor(availableRoomArea);
    }
}
