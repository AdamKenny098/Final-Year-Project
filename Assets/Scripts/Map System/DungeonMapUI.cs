using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonMapUI : MonoBehaviour
{
    [Header("References")]
    public RectTransform mapViewport;
    public RectTransform mapContainer;
    public Image roomIconPrefab;

    [Header("Optional Theme References")]
    public Image backgroundImage;
    public Image frameImage;

    [Header("Map Scale")]
    public float pixelsPerWorldUnit = 6f;
    public bool includeCorridors = true;

    [Header("Minimap Follow")]
    public bool centerOnCurrentRoom = false;

    [Header("Theme Colors")]
    public Color backgroundColor = new Color(0.08f, 0.07f, 0.06f, 0.95f);
    public Color frameColor = new Color(0.16f, 0.13f, 0.11f, 0.95f);

    [Header("Shape Styling")]
    public float roomInsetPixels = 3f;
    public float corridorInsetPixels = 2f;
    public float minimumVisualSize = 4f;

    DungeonRoomBuilder roomBuilder;
    public List<MapRoomIcon> spawnedIcons = new();
    public Dictionary<Room, RectTransform> roomRects = new();

    public float minimumX;
    public float minimumZ;
    public float maximumX;
    public float maximumZ;

    public void SetRoomBuilder(DungeonRoomBuilder builder)
    {
        roomBuilder = builder;
    }

    public void BuildMap()
    {
        ClearMap();

        if (roomBuilder == null || mapContainer == null || roomIconPrefab == null)
            return;

        if (roomBuilder.allRooms == null || roomBuilder.allRooms.Count == 0)
            return;

        ApplyTheme();
        CalculateMapBounds();
        PrepareContainer();

        foreach (Room room in roomBuilder.allRooms)
        {
            if (room == null)
                continue;

            if (!includeCorridors && room.isCorridor)
                continue;

            CreateIcon(room);
        }

        if (centerOnCurrentRoom)
            UpdateCurrentRoomView();
        else
            CenterWholeMap();
    }

    void Update()
    {
        Room current = RoomMapTracker.Instance != null ? RoomMapTracker.Instance.CurrentRoom : null;

        for (int i = 0; i < spawnedIcons.Count; i++)
        {
            if (spawnedIcons[i] == null || spawnedIcons[i].room == null)
                continue;

            spawnedIcons[i].Refresh(spawnedIcons[i].room == current);
        }

        if (centerOnCurrentRoom)
            UpdateCurrentRoomView();
    }

    void ApplyTheme()
    {
        if (backgroundImage == null && mapViewport != null)
            backgroundImage = mapViewport.GetComponent<Image>();

        if (backgroundImage != null)
            backgroundImage.color = backgroundColor;

        if (frameImage != null)
            frameImage.color = frameColor;
    }

    void CalculateMapBounds()
    {
        bool first = true;

        foreach (Room room in roomBuilder.allRooms)
        {
            if (room == null || room.node == null)
                continue;

            float halfWidth = Mathf.FloorToInt(room.node.width) * 0.5f;
            float halfLength = Mathf.FloorToInt(room.node.length) * 0.5f;

            float roomMinX = room.transform.position.x - halfWidth;
            float roomMinZ = room.transform.position.z - halfLength;
            float roomMaxX = room.transform.position.x + halfWidth;
            float roomMaxZ = room.transform.position.z + halfLength;

            if (first)
            {
                minimumX = roomMinX;
                minimumZ = roomMinZ;
                maximumX = roomMaxX;
                maximumZ = roomMaxZ;
                first = false;
            }
            else
            {
                minimumX = Mathf.Min(minimumX, roomMinX);
                minimumZ = Mathf.Min(minimumZ, roomMinZ);
                maximumX = Mathf.Max(maximumX, roomMaxX);
                maximumZ = Mathf.Max(maximumZ, roomMaxZ);
            }
        }
    }

    void PrepareContainer()
    {
        float width = (maximumX - minimumX) * pixelsPerWorldUnit;
        float height = (maximumZ - minimumZ) * pixelsPerWorldUnit;

        mapContainer.anchorMin = new Vector2(0.5f, 0.5f);
        mapContainer.anchorMax = new Vector2(0.5f, 0.5f);
        mapContainer.pivot = new Vector2(0.5f, 0.5f);
        mapContainer.sizeDelta = new Vector2(width, height);
        mapContainer.anchoredPosition = Vector2.zero;
    }

    void CenterWholeMap()
    {
        if (mapContainer == null)
            return;

        mapContainer.anchoredPosition = Vector2.zero;
    }

    void UpdateCurrentRoomView()
    {
        if (!centerOnCurrentRoom)
            return;

        if (mapViewport == null || mapContainer == null)
            return;

        Room current = RoomMapTracker.Instance != null ? RoomMapTracker.Instance.CurrentRoom : null;
        if (current == null)
            return;

        if (!roomRects.TryGetValue(current, out RectTransform currentRect) || currentRect == null)
            return;

        Vector2 roomCenter = currentRect.anchoredPosition + (currentRect.sizeDelta * 0.5f);
        Vector2 containerCenter = mapContainer.sizeDelta * 0.5f;

        mapContainer.anchoredPosition = containerCenter - roomCenter;
    }

    void CreateIcon(Room room)
    {
        if (room.node == null)
            return;

        Image iconImage = Instantiate(roomIconPrefab, mapContainer);
        iconImage.raycastTarget = false;

        RectTransform rectTransform = iconImage.rectTransform;

        float baseWidth = Mathf.FloorToInt(room.node.width) * pixelsPerWorldUnit;
        float baseHeight = Mathf.FloorToInt(room.node.length) * pixelsPerWorldUnit;

        float halfWidth = Mathf.FloorToInt(room.node.width) * 0.5f;
        float halfLength = Mathf.FloorToInt(room.node.length) * 0.5f;

        float roomMinX = room.transform.position.x - halfWidth;
        float roomMinZ = room.transform.position.z - halfLength;

        float uiX = (roomMinX - minimumX) * pixelsPerWorldUnit;
        float uiY = (roomMinZ - minimumZ) * pixelsPerWorldUnit;

        float drawWidth = baseWidth;
        float drawHeight = baseHeight;
        float drawX = uiX;
        float drawY = uiY;

        if (room.isCorridor)
        {
            float yRotation = room.transform.eulerAngles.y;

            bool horizontal =
                Mathf.Abs(Mathf.DeltaAngle(yRotation, 90f)) < 1f ||
                Mathf.Abs(Mathf.DeltaAngle(yRotation, 270f)) < 1f;

            if (horizontal)
            {
                drawWidth = baseHeight;
                drawHeight = baseWidth;

                float centerX = uiX + (baseWidth * 0.5f);
                float centerY = uiY + (baseHeight * 0.5f);

                drawX = centerX - (drawWidth * 0.5f);
                drawY = centerY - (drawHeight * 0.5f);
            }
        }

        float inset = room.isCorridor ? corridorInsetPixels : roomInsetPixels;
        drawX += inset;
        drawY += inset;
        drawWidth = Mathf.Max(minimumVisualSize, drawWidth - (inset * 2f));
        drawHeight = Mathf.Max(minimumVisualSize, drawHeight - (inset * 2f));

        rectTransform.anchorMin = new Vector2(0f, 0f);
        rectTransform.anchorMax = new Vector2(0f, 0f);
        rectTransform.pivot = new Vector2(0f, 0f);
        rectTransform.anchoredPosition = new Vector2(drawX, drawY);
        rectTransform.sizeDelta = new Vector2(drawWidth, drawHeight);
        rectTransform.localEulerAngles = Vector3.zero;
        rectTransform.localScale = Vector3.one;

        MapRoomIcon icon = iconImage.GetComponent<MapRoomIcon>();
        if (icon == null)
            icon = iconImage.gameObject.AddComponent<MapRoomIcon>();

        icon.Bind(room, iconImage);

        spawnedIcons.Add(icon);
        roomRects[room] = rectTransform;
    }

    public void ClearMap()
    {
        if (mapContainer == null)
            return;

        for (int i = mapContainer.childCount - 1; i >= 0; i--)
            Destroy(mapContainer.GetChild(i).gameObject);

        spawnedIcons.Clear();
        roomRects.Clear();
        mapContainer.anchoredPosition = Vector2.zero;
    }
}