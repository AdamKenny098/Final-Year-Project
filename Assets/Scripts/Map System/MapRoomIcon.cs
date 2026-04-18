using UnityEngine;
using UnityEngine.UI;

public class MapRoomIcon : MonoBehaviour
{
    public Room room;
    public Image image;

    [Header("Fill Colors")]
    public Color undiscoveredColor = new Color(0f, 0f, 0f, 0f);
    public Color discoveredRoomColor = new Color(0.78f, 0.73f, 0.67f, 0.62f);
    public Color discoveredCorridorColor = new Color(0.49f, 0.46f, 0.42f, 0.40f);
    public Color currentRoomColor = new Color(0.85f, 0.70f, 0.42f, 0.95f);

    [Header("Outline Colors")]
    public Color undiscoveredOutlineColor = new Color(0f, 0f, 0f, 0f);
    public Color roomOutlineColor = new Color(0.07f, 0.06f, 0.05f, 0.45f);
    public Color corridorOutlineColor = new Color(0.07f, 0.06f, 0.05f, 0.30f);
    public Color currentOutlineColor = new Color(0.98f, 0.86f, 0.62f, 0.85f);

    [Header("Shadow Colors")]
    public Color roomShadowColor = new Color(0f, 0f, 0f, 0.18f);
    public Color currentShadowColor = new Color(0f, 0f, 0f, 0.30f);

    [Header("Effect Settings")]
    public Vector2 normalOutlineDistance = new Vector2(1f, 1f);
    public Vector2 currentOutlineDistance = new Vector2(2f, 2f);
    public Vector2 shadowDistance = new Vector2(2f, -2f);

    RectTransform rectTransform;
    Outline outline;
    Shadow shadow;

    public void Bind(Room targetRoom, Image targetImage)
    {
        room = targetRoom;
        image = targetImage;
        rectTransform = targetImage.rectTransform;

        EnsureEffects();
        Refresh(false);
    }

    void EnsureEffects()
    {
        if (image == null)
            return;

        outline = image.GetComponent<Outline>();
        if (outline == null)
            outline = image.gameObject.AddComponent<Outline>();

        outline.useGraphicAlpha = true;

        shadow = image.GetComponent<Shadow>();
        if (shadow == null)
            shadow = image.gameObject.AddComponent<Shadow>();

        shadow.useGraphicAlpha = true;
        shadow.effectDistance = shadowDistance;
    }

    public void Refresh(bool isCurrentRoom)
    {
        if (room == null || image == null)
            return;

        EnsureEffects();

        if (rectTransform != null)
            rectTransform.localScale = Vector3.one;

        if (!room.visited)
        {
            image.color = undiscoveredColor;
            outline.effectColor = undiscoveredOutlineColor;
            outline.effectDistance = normalOutlineDistance;
            shadow.enabled = false;
            return;
        }

        if (isCurrentRoom)
        {
            image.color = currentRoomColor;
            outline.effectColor = currentOutlineColor;
            outline.effectDistance = currentOutlineDistance;
            shadow.enabled = true;
            shadow.effectColor = currentShadowColor;
            return;
        }

        if (room.isCorridor)
        {
            image.color = discoveredCorridorColor;
            outline.effectColor = corridorOutlineColor;
        }
        else
        {
            image.color = discoveredRoomColor;
            outline.effectColor = roomOutlineColor;
        }

        outline.effectDistance = normalOutlineDistance;
        shadow.enabled = true;
        shadow.effectColor = roomShadowColor;
    }
}