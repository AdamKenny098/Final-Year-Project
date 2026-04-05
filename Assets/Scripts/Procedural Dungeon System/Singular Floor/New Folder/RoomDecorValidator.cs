using System.Collections.Generic;
using UnityEngine;

public class RoomDecorValidator
{
    public bool removeInvalidObjects = false;
    public bool logWarnings = true;

    public void ValidatePlan(RoomDecorPlan plan)
    {
        if (plan == null || plan.room == null)
            return;

        ValidatePlacements(plan, plan.plannedSpecificPlacements, "Specific");
        ValidatePlacements(plan, plan.plannedGenericPlacements, "Generic");
    }

    void ValidatePlacements(RoomDecorPlan plan, List<PlannedDecorPlacement> placements, string label)
    {
        for (int i = 0; i < placements.Count; i++)
        {
            PlannedDecorPlacement placement = placements[i];
            if (placement == null || placement.spawnedInstance == null)
                continue;

            if (!TryGetCombinedColliderBounds(placement.spawnedInstance, out Bounds bounds))
                continue;

            bool invalid = false;

            if (!IsWithinRoomBounds(plan.room, bounds, 0.1f))
                invalid = true;

            if (IntersectsDoorwayBounds(plan.room, bounds, 0.1f))
                invalid = true;

            if (invalid)
            {
                if (logWarnings)
                {
                    Debug.LogWarning(
                        $"[RoomDecorValidator] {label} decor invalid in room '{plan.room.name}' -> {placement.itemName}"
                    );
                }

                if (removeInvalidObjects)
                    Object.Destroy(placement.spawnedInstance);
            }
        }
    }

    bool TryGetCombinedColliderBounds(GameObject obj, out Bounds combinedBounds)
    {
        combinedBounds = new Bounds();
        Collider[] colliders = obj.GetComponentsInChildren<Collider>();

        bool found = false;

        for (int i = 0; i < colliders.Length; i++)
        {
            Collider col = colliders[i];
            if (col == null || !col.enabled)
                continue;

            if (!found)
            {
                combinedBounds = col.bounds;
                found = true;
            }
            else
            {
                combinedBounds.Encapsulate(col.bounds);
            }
        }

        return found;
    }

    bool IsWithinRoomBounds(Room room, Bounds objectBounds, float inset)
    {
        Vector3 roomCenter = room.transform.position;

        float halfWidth = Mathf.FloorToInt(room.node.width) * 0.5f;
        float halfLength = Mathf.FloorToInt(room.node.length) * 0.5f;

        float minX = roomCenter.x - halfWidth + inset;
        float maxX = roomCenter.x + halfWidth - inset;
        float minZ = roomCenter.z - halfLength + inset;
        float maxZ = roomCenter.z + halfLength - inset;

        if (objectBounds.min.x < minX) return false;
        if (objectBounds.max.x > maxX) return false;
        if (objectBounds.min.z < minZ) return false;
        if (objectBounds.max.z > maxZ) return false;

        return true;
    }

    bool IntersectsDoorwayBounds(Room room, Bounds objectBounds, float padding)
    {
        for (int i = 0; i < room.doorways.Count; i++)
        {
            Transform door = room.doorways[i];
            if (door == null)
                continue;

            Bounds doorBounds = new Bounds(
                door.position + Vector3.up * 1.5f,
                new Vector3(3f + padding, 3f, 3f + padding)
            );

            if (doorBounds.Intersects(objectBounds))
                return true;
        }

        return false;
    }
}