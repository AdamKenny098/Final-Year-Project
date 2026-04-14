using System.Collections.Generic;
using UnityEngine;

public class GenericDecorReservationPlacer
{
    public bool TryPlanPillar(RoomDecorPlan plan, GameObject prefab, Vector2Int centerCell)
    {
        if (plan == null || prefab == null)
            return false;

        List<Vector2Int> cells = new List<Vector2Int> { centerCell };

        if (!CanUseGenericCells(plan, cells))
            return false;

        Vector3 worldPos = plan.grid.GetPlacementWorldCenter(centerCell.x, centerCell.y, 1, 1);

        PlannedDecorPlacement placement = new PlannedDecorPlacement(
            "GenericPillar",
            "GenericPillar",
            string.Empty,
            DecorSlotTier.Tertiary,
            centerCell,
            1,
            1,
            prefab,
            worldPos,
            Quaternion.identity,
            cells
        );

        plan.RegisterGenericPlacement(placement);
        ReserveGenericCells(plan, cells, "GenericPillar");

        return true;
    }

    public bool TryPlanTorch(RoomDecorPlan plan, GameObject prefab, Vector2Int cell, Quaternion localRotation)
    {
        if (plan == null || prefab == null)
            return false;

        List<Vector2Int> cells = new List<Vector2Int> { cell };

        if (!CanUseGenericCells(plan, cells))
            return false;

        Vector3 worldPos = plan.grid.GetPlacementWorldCenter(cell.x, cell.y, 1, 1);

        PlannedDecorPlacement placement = new PlannedDecorPlacement(
            "GenericTorch",
            "GenericTorch",
            string.Empty,
            DecorSlotTier.Tertiary,
            cell,
            1,
            1,
            prefab,
            worldPos,
            localRotation,
            cells
        );

        plan.RegisterGenericPlacement(placement);
        ReserveGenericCells(plan, cells, "GenericTorch");

        return true;
    }

    bool CanUseGenericCells(RoomDecorPlan plan, List<Vector2Int> cells)
    {
        for (int i = 0; i < cells.Count; i++)
        {
            Vector2Int pos = cells[i];
            RoomTile tile = plan.grid.GetTile(pos);

            if (tile == null)
                return false;

            if (tile.blocked)
                return false;

            if (tile.isDoorway || tile.isDoorBuffer)
                return false;

            if (plan.IsPlacementCellOccupied(pos))
                return false;

            if (tile.reservation.priority >= DecorReservationPriority.Cluster)
                return false;
        }

        return true;
    }

    void ReserveGenericCells(RoomDecorPlan plan, List<Vector2Int> cells, string source)
    {
        for (int i = 0; i < cells.Count; i++)
        {
            Vector2Int pos = cells[i];

            plan.grid.TryReserveCell(
                pos.x,
                pos.y,
                DecorReservationPriority.Generic,
                DecorReservationType.Generic,
                source
            );
        }
    }
}