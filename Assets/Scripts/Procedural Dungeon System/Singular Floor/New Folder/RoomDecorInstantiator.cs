using UnityEngine;

public class RoomDecorInstantiator
{
    public void InstantiatePlan(RoomDecorPlan plan, Transform specificParent, Transform genericPillarParent, Transform genericTorchParent)
    {
        if (plan == null || plan.room == null)
            return;

        InstantiateSpecific(plan, specificParent);
        InstantiateGeneric(plan, genericPillarParent, genericTorchParent);
    }

    void InstantiateSpecific(RoomDecorPlan plan, Transform parent)
    {
        for (int i = 0; i < plan.plannedSpecificPlacements.Count; i++)
        {
            PlannedDecorPlacement placement = plan.plannedSpecificPlacements[i];
            if (placement == null || placement.prefab == null || placement.spawnedInstance != null)
                continue;

            GameObject instance = Object.Instantiate(
                placement.prefab,
                placement.worldPosition,
                plan.room.transform.rotation * placement.localRotation,
                parent
            );

            placement.spawnedInstance = instance;
            TagDecorRecursive(instance);
        }
    }

    void InstantiateGeneric(RoomDecorPlan plan, Transform pillarParent, Transform torchParent)
    {
        for (int i = 0; i < plan.plannedGenericPlacements.Count; i++)
        {
            PlannedDecorPlacement placement = plan.plannedGenericPlacements[i];
            if (placement == null || placement.prefab == null || placement.spawnedInstance != null)
                continue;

            Transform parent = placement.itemTag == "GenericTorch" ? torchParent : pillarParent;

            GameObject instance = Object.Instantiate(
                placement.prefab,
                placement.worldPosition,
                plan.room.transform.rotation * placement.localRotation,
                parent
            );

            placement.spawnedInstance = instance;
            TagDecorRecursive(instance);
        }
    }

    void TagDecorRecursive(GameObject obj)
    {
        obj.tag = "Decor";

        for (int i = 0; i < obj.transform.childCount; i++)
            TagDecorRecursive(obj.transform.GetChild(i).gameObject);
    }
}