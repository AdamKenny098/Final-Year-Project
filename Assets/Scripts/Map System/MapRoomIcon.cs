using UnityEngine;
using UnityEngine.UI;

public class MapRoomIcon : MonoBehaviour
{
    public Room room;
    public Image image;

    [Header("Colors")]
    public Color undiscoveredColor = new Color(0f, 0f, 0f, 0f);
    public Color discoveredRoomColor = new Color(1f, 1f, 1f, 0.35f);
    public Color discoveredCorridorColor = new Color(0.7f, 0.7f, 0.7f, 0.3f);
    public Color currentRoomColor = new Color(1f, 1f, 1f, 1f);

    public void Bind(Room targetRoom, Image targetImage)
    {
        room = targetRoom;
        image = targetImage;
        Refresh(false);
    }

    public void Refresh(bool isCurrentRoom)
    {
        if (room == null || image == null)
            return;

        if (!room.visited)
        {
            image.color = undiscoveredColor;
            return;
        }

        if (isCurrentRoom)
        {
            image.color = currentRoomColor;
            return;
        }

        image.color = room.isCorridor ? discoveredCorridorColor : discoveredRoomColor;
    }
}