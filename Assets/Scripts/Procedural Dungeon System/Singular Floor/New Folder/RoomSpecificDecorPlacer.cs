using System.Collections.Generic;
using UnityEngine;

public class RoomSpecificDecorPlacer
{
    public int PlanRoom(RoomDecorPlan plan, RoomItemDatabase database)
    {
        if (plan == null || database == null || plan.room == null)
            return 0;

        if (!database.roomItems.TryGetValue(plan.room.roomType, out List<RoomItem> items))
            return 0;

        int plannedCount = 0;

        plannedCount += PlanTier(plan, items, RoomItem.Priority.Primary, DecorSlotTier.Primary);
        plannedCount += PlanTier(plan, items, RoomItem.Priority.Secondary, DecorSlotTier.Secondary);
        plannedCount += PlanTier(plan, items, RoomItem.Priority.Tertiary, DecorSlotTier.Tertiary);

        return plannedCount;
    }

    int PlanTier(
    RoomDecorPlan plan,
    List<RoomItem> items,
    RoomItem.Priority priority,
    DecorSlotTier tier)
    {
        List<RoomItem> tierItems = new List<RoomItem>();

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] != null && items[i].priority == priority)
                tierItems.Add(items[i]);
        }

        int plannedCount = 0;

        for (int i = 0; i < tierItems.Count; i++)
        {
            RoomItem item = tierItems[i];
            if (item == null || item.prefab == null)
                continue;

            int targetCount = Random.Range(item.minCount, item.maxCount + 1);

            int attempts = 0;
            int maxAttempts = Mathf.Max(targetCount * 3, targetCount);

            while (attempts < maxAttempts)
            {
                if (TryPlanItem(plan, item, tier))
                    plannedCount++;

                attempts++;
            }
        }

        return plannedCount;
    }

    bool TryPlanItem(RoomDecorPlan plan, RoomItem item, DecorSlotTier tier)
    {
        List<DecorSlot> slots = plan.GetSelectedSlotsByTier(tier);
        if (slots.Count == 0)
            return false;

        float bestScore = float.NegativeInfinity;
        PlacementCandidate best = null;

        for (int i = 0; i < slots.Count; i++)
        {
            DecorSlot slot = slots[i];
            if (slot == null || slot.occupied)
                continue;

            EvaluateCandidate(plan, item, slot, item.gridWidth, item.gridLength, Quaternion.identity, ref bestScore, ref best);

            if (item.allowRotation && item.gridWidth != item.gridLength)
            {
                EvaluateCandidate(
                    plan,
                    item,
                    slot,
                    item.gridLength,
                    item.gridWidth,
                    Quaternion.Euler(0f, 90f, 0f),
                    ref bestScore,
                    ref best
                );
            }
        }

        if (best == null)
            return false;

        PlannedDecorPlacement placement = new PlannedDecorPlacement(
            item.name,
            string.IsNullOrEmpty(item.itemTag) ? item.name : item.itemTag,
            best.slot.ownerClusterId,
            tier,
            best.slot.gridPos,
            best.width,
            best.length,
            item.prefab,
            best.worldPosition,
            best.localRotation,
            best.occupiedCells
        );

        plan.RegisterPlacement(placement);
        best.slot.Occupy(item.name);

        return true;
    }

    void EvaluateCandidate(
        RoomDecorPlan plan,
        RoomItem item,
        DecorSlot slot,
        int width,
        int length,
        Quaternion localRotation,
        ref float bestScore,
        ref PlacementCandidate best)
    {
        if (!TryBuildFootprint(plan, slot, width, length, out List<Vector2Int> occupiedCells, out int startX, out int startZ))
            return;

        float score = ScoreCandidate(plan, item, slot, occupiedCells);
        score += Random.Range(0f, 0.25f);

        if (score <= bestScore)
            return;

        bestScore = score;
        best = new PlacementCandidate
        {
            slot = slot,
            width = width,
            length = length,
            occupiedCells = occupiedCells,
            localRotation = localRotation,
            worldPosition = plan.grid.GetPlacementWorldCenter(startX, startZ, width, length)
        };
    }

    bool TryBuildFootprint(
        RoomDecorPlan plan,
        DecorSlot slot,
        int width,
        int length,
        out List<Vector2Int> occupiedCells,
        out int startX,
        out int startZ)
    {
        occupiedCells = new List<Vector2Int>();
        startX = 0;
        startZ = 0;

        if (plan == null || slot == null)
            return false;

        DecorCluster cluster = plan.GetClusterById(slot.ownerClusterId);
        if (cluster == null)
            return false;

        startX = slot.gridPos.x - Mathf.FloorToInt((width - 1) * 0.5f);
        startZ = slot.gridPos.y - Mathf.FloorToInt((length - 1) * 0.5f);

        for (int x = startX; x < startX + width; x++)
        {
            for (int z = startZ; z < startZ + length; z++)
            {
                Vector2Int cell = new Vector2Int(x, z);

                if (!plan.grid.IsInside(cell.x, cell.y))
                    return false;

                if (!cluster.footprintCells.Contains(cell))
                    return false;

                if (plan.IsPlacementCellOccupied(cell))
                    return false;

                occupiedCells.Add(cell);
            }
        }

        return true;
    }

    float ScoreCandidate(RoomDecorPlan plan, RoomItem item, DecorSlot slot, List<Vector2Int> occupiedCells)
    {
        RoomTile tile = plan.grid.GetTile(slot.gridPos);
        if (tile == null)
            return float.NegativeInfinity;

        float score = 0f;

        score += tile.wallScore * item.preferWall;
        score += tile.centerScore * item.preferCenter;
        score -= tile.centerScore * item.avoidCenter;
        score += tile.doorDistance * item.avoidDoors;

        if (tile.isDoorBuffer)
            score -= 100f;

        if (!string.IsNullOrEmpty(item.anchorTag))
            score += ScoreAnchorPreference(plan, slot.gridPos, item.anchorTag, item.preferNearAnchor, item.minAnchorDistance, item.maxAnchorDistance);

        if (!string.IsNullOrEmpty(item.itemTag) && item.preferNearSameTag > 0f)
            score += ScoreAnchorPreference(plan, slot.gridPos, item.itemTag, item.preferNearSameTag, 1, 6);

        score += occupiedCells.Count * 0.05f;

        return score;
    }

    float ScoreAnchorPreference(
        RoomDecorPlan plan,
        Vector2Int slotPos,
        string targetTag,
        float preference,
        int minDist,
        int maxDist)
    {
        float score = 0f;

        for (int i = 0; i < plan.plannedSpecificPlacements.Count; i++)
        {
            PlannedDecorPlacement placed = plan.plannedSpecificPlacements[i];
            if (placed == null || placed.itemTag != targetTag)
                continue;

            int dist = Mathf.Abs(slotPos.x - placed.anchorGridPos.x) + Mathf.Abs(slotPos.y - placed.anchorGridPos.y);

            if (dist >= minDist && dist <= maxDist)
                score += preference * 5f;
            else
                score -= Mathf.Abs(dist - maxDist) * 0.5f;
        }

        return score;
    }

    class PlacementCandidate
    {
        public DecorSlot slot;
        public int width;
        public int length;
        public Vector3 worldPosition;
        public Quaternion localRotation;
        public List<Vector2Int> occupiedCells;
    }
}