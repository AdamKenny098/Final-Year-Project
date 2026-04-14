using System.Collections.Generic;
using UnityEngine;

public class RoomGrid
{
    public Room room;
    public int width;
    public int length;

    public float minimumX;
    public float minimumZ;

    public RoomTile[,] tiles;
    public List<DecorAnchor> anchors = new List<DecorAnchor>();

    public RoomGrid(Room room)
    {
        this.room = room;

        width = Mathf.FloorToInt(room.node.width) - 2;
        length = Mathf.FloorToInt(room.node.length) - 2;

        width = Mathf.Max(1, width);
        length = Mathf.Max(1, length);

        minimumX = room.transform.position.x - Mathf.FloorToInt(room.node.width) / 2f + 1.5f;
        minimumZ = room.transform.position.z - Mathf.FloorToInt(room.node.length) / 2f + 1.5f;

        tiles = new RoomTile[width, length];

        BuildTiles();
        BuildStaticScores();
        MarkDoorwaysAndBuffers();
    }

    void BuildTiles()
    {
        for (int gridX = 0; gridX < width; gridX++)
        {
            for (int gridZ = 0; gridZ < length; gridZ++)
            {
                Vector3 worldPosition = new Vector3(minimumX + gridX, room.transform.position.y + 1f, minimumZ + gridZ);

                tiles[gridX, gridZ] = new RoomTile
                {
                    gridPos = new Vector2Int(gridX, gridZ),
                    worldPos = worldPosition,
                    blocked = false,
                    isDoorway = false,
                    isDoorBuffer = false
                };
                tiles[gridX, gridZ].reservation.gridPos = new Vector2Int(gridX, gridZ);
            }
        }
    }

    void BuildStaticScores()
    {
        Vector2 center = new Vector2((width - 1) * 0.5f, (length - 1) * 0.5f);
        float maxCenterDist = Vector2.Distance(Vector2.zero, center);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                RoomTile tile = tiles[i, j];

                int distToWall = Mathf.Min(i, j, width - 1 - i, length - 1 - j);
                tile.wallScore = 1f / (1f + distToWall);

                float centerDist = Vector2.Distance(new Vector2(i, j), center);
                tile.centerScore = maxCenterDist <= 0.001f ? 1f : 1f - (centerDist / maxCenterDist);
            }
        }
    }

    void MarkDoorwaysAndBuffers()
    {
        foreach (Vector3 doorwayWorldPosition in room.plannedDoorwayPositions)
        {
            Vector2Int doorGridPosition = WorldToGrid(doorwayWorldPosition);

            if (IsInside(doorGridPosition.x, doorGridPosition.y))
            {
                RoomTile doorwayTile = tiles[doorGridPosition.x, doorGridPosition.y];
                doorwayTile.isDoorway = true;
                doorwayTile.TryReserve(DecorReservationPriority.Protected, DecorReservationType.Doorway, "Doorway");
            }

            for (int offsetX = -2; offsetX <= 2; offsetX++)
            {
                for (int offsetZ = -2; offsetZ <= 2; offsetZ++)
                {
                    int bufferX = doorGridPosition.x + offsetX;
                    int bufferZ = doorGridPosition.y + offsetZ;

                    if (!IsInside(bufferX, bufferZ))
                        continue;

                    RoomTile bufferTile = tiles[bufferX, bufferZ];
                    bufferTile.isDoorBuffer = true;

                    if (!bufferTile.isDoorway)
                    {
                        bufferTile.TryReserve(DecorReservationPriority.Protected, DecorReservationType.DoorBuffer, "DoorBuffer");
                    }
                }
            }
        }

        ComputeDoorDistance();
    }

    void ComputeDoorDistance()
    {
        Queue<Vector2Int> tilesToCheck = new Queue<Vector2Int>();

        for (int gridX = 0; gridX < width; gridX++)
        {
            for (int gridZ = 0; gridZ < length; gridZ++)
            {
                if (tiles[gridX, gridZ].isDoorway)
                {
                    tiles[gridX, gridZ].doorDistance = 0f;
                    tilesToCheck.Enqueue(new Vector2Int(gridX, gridZ));
                }
                else
                {
                    tiles[gridX, gridZ].doorDistance = 9999f;
                }
            }
        }

        Vector2Int[] cardinalDirections =
        {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };

        while (tilesToCheck.Count > 0)
        {
            Vector2Int currentTilePos = tilesToCheck.Dequeue();
            float currentDistance = tiles[currentTilePos.x, currentTilePos.y].doorDistance;

            foreach (Vector2Int direction in cardinalDirections)
            {
                int nextX = currentTilePos.x + direction.x;
                int nextZ = currentTilePos.y + direction.y;

                if (!IsInside(nextX, nextZ))
                    continue;

                float nextDistance = currentDistance + 1f;

                if (nextDistance < tiles[nextX, nextZ].doorDistance)
                {
                    tiles[nextX, nextZ].doorDistance = nextDistance;
                    tilesToCheck.Enqueue(new Vector2Int(nextX, nextZ));
                }
            }
        }
    }

    public bool IsInside(int x, int z)
    {
        return x >= 0 && x < width && z >= 0 && z < length;
    }

    public Vector2Int WorldToGrid(Vector3 world)
    {
        int x = Mathf.RoundToInt(world.x - minimumX);
        int z = Mathf.RoundToInt(world.z - minimumZ);
        return new Vector2Int(x, z);
    }

    public Vector3 GetPlacementWorldCenter(int startX, int startZ, int itemWidth, int itemLength)
    {
        float centerX = minimumX + startX + (itemWidth - 1) * 0.5f;
        float centerZ = minimumZ + startZ + (itemLength - 1) * 0.5f;

        return new Vector3(centerX, room.transform.position.y + 2f, centerZ);
    }

    public RoomTile GetTile(int x, int z)
    {
        if (!IsInside(x, z))
            return null;

        return tiles[x, z];
    }

    public RoomTile GetTile(Vector2Int pos)
    {
        return GetTile(pos.x, pos.y);
    }

    public bool CanReserveCell(
        int x,
        int z,
        DecorReservationPriority priority)
    {
        if (!IsInside(x, z))
            return false;

        RoomTile tile = tiles[x, z];
        return tile.CanReserve(priority);
    }

    public bool TryReserveCell(
        int x,
        int z,
        DecorReservationPriority priority,
        DecorReservationType type,
        string source)
    {
        if (!IsInside(x, z))
            return false;

        return tiles[x, z].TryReserve(priority, type, source);
    }

    public bool CanReserveArea(
        int startX,
        int startZ,
        int areaWidth,
        int areaLength,
        DecorReservationPriority priority)
    {
        for (int x = startX; x < startX + areaWidth; x++)
        {
            for (int z = startZ; z < startZ + areaLength; z++)
            {
                if (!IsInside(x, z))
                    return false;

                if (!tiles[x, z].CanReserve(priority))
                    return false;
            }
        }

        return true;
    }

    public bool TryReserveArea(int startX, int startZ, int areaWidth, int areaLength, DecorReservationPriority priority,DecorReservationType type, string source)
    {
        if (!CanReserveArea(startX, startZ, areaWidth, areaLength, priority))
            return false;

        for (int x = startX; x < startX + areaWidth; x++)
        {
            for (int z = startZ; z < startZ + areaLength; z++)
            {
                tiles[x, z].TryReserve(priority, type, source);
            }
        }

        return true;
    }

    public bool CanReserveCells(List<Vector2Int> cells,DecorReservationPriority priority)
    {
        for (int i = 0; i < cells.Count; i++)
        {
            Vector2Int pos = cells[i];

            if (!IsInside(pos.x, pos.y))
                return false;

            if (!tiles[pos.x, pos.y].CanReserve(priority))
                return false;
        }
        return true;
    }

    public bool TryReserveCells(List<Vector2Int> cells,DecorReservationPriority priority,DecorReservationType type, string source)
    {
        if (!CanReserveCells(cells, priority))
            return false;

        for (int i = 0; i < cells.Count; i++)
        {
            Vector2Int pos = cells[i];
            tiles[pos.x, pos.y].TryReserve(priority, type, source);
        }

        return true;
    }

    public bool TryReserveClusterFootprint(DecorCluster cluster)
    {
        if (cluster == null)
            return false;

        return TryReserveCells( cluster.footprintCells, DecorReservationPriority.Cluster, DecorReservationType.ClusterFootprint, cluster.id);
    }

    public bool TryReserveSlot(DecorSlot slot)
    {
        if (slot == null)
            return false;

        DecorReservationType type = DecorReservationType.None;

        switch (slot.tier)
        {
            case DecorSlotTier.Primary:
                type = DecorReservationType.PrimarySlot;
                break;

            case DecorSlotTier.Secondary:
                type = DecorReservationType.SecondarySlot;
                break;

            case DecorSlotTier.Tertiary:
                type = DecorReservationType.TertiarySlot;
                break;
        }

        DecorReservationPriority priority = slot.tier == DecorSlotTier.Primary ? DecorReservationPriority.Protected : DecorReservationPriority.Cluster;

        return TryReserveCell(slot.gridPos.x, slot.gridPos.y, priority, type, slot.ownerClusterId);
    }

    public void ClearReservation(int x, int z)
    {
        if (!IsInside(x, z))
            return;

        tiles[x, z].ClearReservation();
    }

    public void ClearReservationsBySource(string source)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                RoomTile tile = tiles[x, z];

                if (tile.reservation.source == source && !tile.blocked)
                    tile.ClearReservation();
            }
        }
    }

    public bool CanPlace(int startX, int startZ, int itemWidth, int itemLength, int extraClearance = 0)
    {
        for (int x = startX - extraClearance; x < startX + itemWidth + extraClearance; x++)
        {
            for (int z = startZ - extraClearance; z < startZ + itemLength + extraClearance; z++)
            {
                if (!IsInside(x, z))
                    return false;

                if (!tiles[x, z].IsFree)
                    return false;
            }
        }

        return true;
    }

    public void Reserve(int startX, int startZ, int itemWidth, int itemLength)
    {
        TryReserveArea(startX, startZ, itemWidth, itemLength, DecorReservationPriority.Cluster, DecorReservationType.ClusterFootprint, "LegacyReserve");
    }

    public float ScorePlacement(RoomItem item, int startX, int startZ, int itemWidth, int itemLength)
    {
        float score = 0f;

        int centerX = startX + itemWidth / 2;
        int centerZ = startZ + itemLength / 2;

        if (!IsInside(centerX, centerZ))
            return float.NegativeInfinity;

        RoomTile centerTile = tiles[centerX, centerZ];

        score += centerTile.wallScore * item.preferWall;
        score += centerTile.centerScore * item.preferCenter;
        score -= centerTile.centerScore * item.avoidCenter;
        score += centerTile.doorDistance * item.avoidDoors;

        if (centerTile.isDoorBuffer)
            score -= 100f;

        if (!string.IsNullOrEmpty(item.anchorTag))
        {
            float bestAnchorScore = -9999f;

            foreach (DecorAnchor anchor in anchors)
            {
                if (anchor.tag != item.anchorTag)
                    continue;

                float dist = Mathf.Abs(anchor.gridPos.x - centerX) + Mathf.Abs(anchor.gridPos.y - centerZ);

                if (dist >= item.minAnchorDistance && dist <= item.maxAnchorDistance)
                    bestAnchorScore = Mathf.Max(bestAnchorScore, item.preferNearAnchor * 25f);
                else
                    bestAnchorScore = Mathf.Max(bestAnchorScore, -Mathf.Abs(dist - item.maxAnchorDistance));
            }

            score += bestAnchorScore;
        }

        if (!string.IsNullOrEmpty(item.itemTag) && item.preferNearSameTag > 0f)
        {
            foreach (DecorAnchor anchor in anchors)
            {
                if (anchor.tag != item.itemTag)
                    continue;

                float dist = Mathf.Abs(anchor.gridPos.x - centerX) + Mathf.Abs(anchor.gridPos.y - centerZ);
                score += Mathf.Max(0f, item.preferNearSameTag * (6f - dist));
            }
        }

        return score;
    }
}