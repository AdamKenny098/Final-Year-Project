using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomDecorPlan
{
    public Room room;
    public RoomGrid grid;

    public Bounds roomBounds;

    public List<DecorCluster> candidateClusters = new List<DecorCluster>();
    public List<DecorCluster> selectedClusters = new List<DecorCluster>();

    public List<PlannedDecorPlacement> plannedSpecificPlacements = new List<PlannedDecorPlacement>();
    public List<PlannedDecorPlacement> plannedGenericPlacements = new List<PlannedDecorPlacement>();
    public HashSet<Vector2Int> occupiedPlacementCells = new HashSet<Vector2Int>();

    public RoomDecorPlan(Room room)
    {
        this.room = room;
        grid = new RoomGrid(room);

        roomBounds = new Bounds(
            room.transform.position,
            new Vector3(room.node.width, 4f, room.node.length)
        );
    }

    public void ClearPlan()
    {
        candidateClusters.Clear();
        selectedClusters.Clear();
        plannedSpecificPlacements.Clear();
        plannedGenericPlacements.Clear();
        occupiedPlacementCells.Clear();
    }

    public void ClearClusters()
    {
        candidateClusters.Clear();
        selectedClusters.Clear();
    }

    public DecorCluster GetClusterById(string clusterId)
    {
        for (int i = 0; i < selectedClusters.Count; i++)
        {
            if (selectedClusters[i] != null && selectedClusters[i].id == clusterId)
                return selectedClusters[i];
        }

        return null;
    }

    public List<DecorSlot> GetAllSelectedSlots()
    {
        List<DecorSlot> slots = new List<DecorSlot>();

        for (int i = 0; i < selectedClusters.Count; i++)
        {
            DecorCluster cluster = selectedClusters[i];
            if (cluster == null) continue;

            slots.AddRange(cluster.primarySlots);
            slots.AddRange(cluster.secondarySlots);
            slots.AddRange(cluster.tertiarySlots);
        }

        return slots;
    }

    public List<DecorSlot> GetSelectedSlotsByTier(DecorSlotTier tier)
    {
        List<DecorSlot> slots = new List<DecorSlot>();

        for (int i = 0; i < selectedClusters.Count; i++)
        {
            DecorCluster cluster = selectedClusters[i];
            if (cluster == null)
                continue;

            List<DecorSlot> source = null;

            switch (tier)
            {
                case DecorSlotTier.Primary:
                    source = cluster.primarySlots;
                    break;

                case DecorSlotTier.Secondary:
                    source = cluster.secondarySlots;
                    break;

                case DecorSlotTier.Tertiary:
                    source = cluster.tertiarySlots;
                    break;
            }

            if (source == null)
                continue;

            for (int j = 0; j < source.Count; j++)
            {
                DecorSlot slot = source[j];
                if (slot != null && slot.enabled)
                    slots.Add(slot);
            }
        }

        return slots;
    }

    public bool IsPlacementCellOccupied(Vector2Int cell)
    {
        return occupiedPlacementCells.Contains(cell);
    }

    public bool ArePlacementCellsFree(List<Vector2Int> cells)
    {
        for (int i = 0; i < cells.Count; i++)
        {
            if (occupiedPlacementCells.Contains(cells[i]))
                return false;
        }

        return true;
    }

    public void RegisterPlacement(PlannedDecorPlacement placement)
    {
        if (placement == null)
            return;

        plannedSpecificPlacements.Add(placement);

        for (int i = 0; i < placement.occupiedCells.Count; i++)
            occupiedPlacementCells.Add(placement.occupiedCells[i]);

        MarkCoveredSlotsOccupied(placement);
    }

    public void RegisterGenericPlacement(PlannedDecorPlacement placement)
    {
        if (placement == null)
            return;

        plannedGenericPlacements.Add(placement);

        for (int i = 0; i < placement.occupiedCells.Count; i++)
            occupiedPlacementCells.Add(placement.occupiedCells[i]);
    }

    void MarkCoveredSlotsOccupied(PlannedDecorPlacement placement)
    {
        if (placement == null)
            return;

        for (int i = 0; i < selectedClusters.Count; i++)
        {
            DecorCluster cluster = selectedClusters[i];
            if (cluster == null)
                continue;

            foreach (DecorSlot slot in cluster.GetAllSlots())
            {
                if (slot == null || slot.occupied)
                    continue;

                if (placement.occupiedCells.Contains(slot.gridPos))
                    slot.Occupy(placement.itemName);
            }
        }
    }
}