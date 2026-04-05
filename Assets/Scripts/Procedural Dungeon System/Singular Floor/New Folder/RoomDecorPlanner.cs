using System.Collections.Generic;
using UnityEngine;

public class RoomDecorPlanner
{
    public int clusterSize = 5;
    public int clusterRadius = 2;

    [Header("Selection")]
    public int minimumClusterSpacing = 5;

    public RoomDecorPlan BuildPlan(Room room)
    {
        if (room == null || room.isCorridor)
            return null;

        RoomDecorPlan plan = new RoomDecorPlan(room);

        GenerateClusterCandidates(plan);
        SelectClusters(plan);
        ReserveSelectedClusters(plan);

        return plan;
    }

    public void GenerateClusterCandidates(RoomDecorPlan plan)
    {
        if (plan == null || plan.grid == null)
            return;

        plan.ClearClusters();

        List<int> xCenters = GetAxisCenters(plan.grid.width);
        List<int> zCenters = GetAxisCenters(plan.grid.length);

        if (xCenters.Count == 0 || zCenters.Count == 0)
            return;

        int clusterIndex = 0;

        for (int z = 0; z < zCenters.Count; z++)
        {
            for (int x = 0; x < xCenters.Count; x++)
            {
                Vector2Int center = new Vector2Int(xCenters[x], zCenters[z]);

                DecorCluster cluster = BuildCandidateCluster(plan, center, clusterIndex);
                clusterIndex++;

                if (cluster != null && cluster.isValid)
                    plan.candidateClusters.Add(cluster);
            }
        }
    }

    DecorCluster BuildCandidateCluster(RoomDecorPlan plan, Vector2Int centerGridPos, int index)
    {
        if (plan == null || plan.grid == null)
            return null;

        DecorCluster cluster = new DecorCluster(
            $"Cluster_{plan.room.name}_{index}",
            centerGridPos
        );

        cluster.BuildStandard5x5Layout();

        if (!IsClusterInsideGrid(plan.grid, cluster))
        {
            cluster.isValid = false;
            return cluster;
        }

        if (!CanUseClusterFootprint(plan.grid, cluster, out string reason))
        {
            cluster.isValid = false;
            return cluster;
        }

        cluster.isValid = true;
        return cluster;
    }

    bool IsClusterInsideGrid(RoomGrid grid, DecorCluster cluster)
    {
        for (int i = 0; i < cluster.footprintCells.Count; i++)
        {
            Vector2Int pos = cluster.footprintCells[i];

            if (!grid.IsInside(pos.x, pos.y))
                return false;
        }

        return true;
    }

    bool CanUseClusterFootprint(RoomGrid grid, DecorCluster cluster, out string reason)
    {
        reason = string.Empty;

        for (int i = 0; i < cluster.footprintCells.Count; i++)
        {
            Vector2Int pos = cluster.footprintCells[i];
            RoomTile tile = grid.GetTile(pos);

            if (tile == null)
            {
                reason = "NullTile";
                return false;
            }

            if (tile.blocked)
            {
                reason = "Blocked";
                return false;
            }

            if (tile.isDoorway)
            {
                reason = "Doorway";
                return false;
            }

            if (tile.isDoorBuffer)
            {
                reason = "DoorBuffer";
                return false;
            }

            if (!tile.CanReserve(DecorReservationPriority.Cluster))
            {
                reason = $"Reserved:{tile.reservation.type}";
                return false;
            }
        }

        return true;
    }

    public void SelectClusters(RoomDecorPlan plan)
    {
        if (plan == null)
            return;

        plan.selectedClusters.Clear();

        if (plan.candidateClusters.Count == 0)
            return;

        List<DecorCluster> shuffled = new List<DecorCluster>(plan.candidateClusters);
        Shuffle(shuffled);

        int targetCount = Mathf.Min(GetTargetClusterCount(plan.room), shuffled.Count);

        for (int i = 0; i < shuffled.Count; i++)
        {
            if (plan.selectedClusters.Count >= targetCount)
                break;

            DecorCluster candidate = shuffled[i];
            if (candidate == null || !candidate.isValid)
                continue;

            if (!HasEnoughSpacing(plan.selectedClusters, candidate))
                continue;

            candidate.isSelected = true;
            plan.selectedClusters.Add(candidate);
        }
    }

    int GetTargetClusterCount(Room room)
    {
        if (room == null)
            return 0;

        float area = room.roomArea;

        if (area <= 150f)  return Random.Range(1, 3);
        if (area <= 250f)  return Random.Range(2, 4);
        if (area <= 400f)  return Random.Range(3, 6);
        if (area <= 600f)  return Random.Range(5, 8);
        if (area <= 800f)  return Random.Range(7, 11);
        if (area <= 1000f) return Random.Range(9, 13);
        if (area <= 1200f) return Random.Range(11, 15);
        if (area <= 1500f) return Random.Range(13, 18);
        if (area <= 1800f) return Random.Range(15, 21);
        if (area <= 2000f) return Random.Range(17, 24);
        if (area <= 2200f) return Random.Range(19, 26);
        if (area <= 2500f) return Random.Range(21, 29);

        return Random.Range(23, 31);
    }

    bool HasEnoughSpacing(List<DecorCluster> selected, DecorCluster candidate)
    {
        for (int i = 0; i < selected.Count; i++)
        {
            DecorCluster existing = selected[i];
            if (existing == null)
                continue;

            int dist = GridDistance(existing.centerGridPos, candidate.centerGridPos);

            if (dist < minimumClusterSpacing)
                return false;
        }

        return true;
    }

    int GridDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    public void ReserveSelectedClusters(RoomDecorPlan plan)
    {
        if (plan == null || plan.grid == null)
            return;

        List<DecorCluster> successfullyReserved = new List<DecorCluster>();

        for (int i = 0; i < plan.selectedClusters.Count; i++)
        {
            DecorCluster cluster = plan.selectedClusters[i];
            if (cluster == null)
                continue;

            bool reservedFootprint = plan.grid.TryReserveClusterFootprint(cluster);
            if (!reservedFootprint)
            {
                cluster.isSelected = false;
                continue;
            }

            for (int p = 0; p < cluster.primarySlots.Count; p++)
                plan.grid.TryReserveSlot(cluster.primarySlots[p]);

            for (int s = 0; s < cluster.secondarySlots.Count; s++)
                plan.grid.TryReserveSlot(cluster.secondarySlots[s]);

            for (int t = 0; t < cluster.tertiarySlots.Count; t++)
                plan.grid.TryReserveSlot(cluster.tertiarySlots[t]);

            successfullyReserved.Add(cluster);
        }

        plan.selectedClusters = successfullyReserved;
    }

    List<int> GetAxisCenters(int dimension)
    {
        List<int> centers = new List<int>();

        if (dimension < clusterSize)
            return centers;

        int blockCount = dimension / clusterSize;
        int remainder = dimension % clusterSize;

        if (blockCount <= 0)
            return centers;

        int[] gaps = BuildGapPattern(blockCount, remainder);

        int cursor = gaps[0];

        for (int i = 0; i < blockCount; i++)
        {
            int blockStart = cursor;
            int center = blockStart + clusterRadius;

            centers.Add(center);

            cursor = blockStart + clusterSize + gaps[i + 1];
        }

        return centers;
    }

    int[] BuildGapPattern(int blockCount, int remainder)
    {
        int[] gaps = new int[blockCount + 1];

        switch (remainder)
        {
            case 0:
                break;

            case 1:
                AddCenterGap(gaps, blockCount, 1);
                break;

            case 2:
                gaps[0] += 1;
                gaps[gaps.Length - 1] += 1;
                break;

            case 3:
                gaps[0] += 1;
                gaps[gaps.Length - 1] += 1;
                AddCenterGap(gaps, blockCount, 1);
                break;

            case 4:
                gaps[0] += 1;
                gaps[gaps.Length - 1] += 1;
                AddCenterGap(gaps, blockCount, 2);
                break;
        }

        return gaps;
    }

    void AddCenterGap(int[] gaps, int blockCount, int amount)
    {
        if (amount <= 0 || gaps == null || gaps.Length == 0)
            return;

        if (blockCount <= 1)
        {
            int left = amount / 2;
            int right = amount - left;

            gaps[0] += left;
            gaps[gaps.Length - 1] += right;
            return;
        }

        int internalGapCount = blockCount - 1;
        int targetGapIndex;

        if (internalGapCount % 2 == 1)
        {
            targetGapIndex = 1 + (internalGapCount / 2);
        }
        else
        {
            targetGapIndex = internalGapCount / 2;
        }

        targetGapIndex = Mathf.Clamp(targetGapIndex, 1, gaps.Length - 2);
        gaps[targetGapIndex] += amount;
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}