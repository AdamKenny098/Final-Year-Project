using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dungeon/Room Type Set", fileName = "RoomTypeSet_")]
public class RoomTypeSet : ScriptableObject
{
    public Room.RoomType roomType;
    public List<RoomItemDefinition> items = new List<RoomItemDefinition>();
}