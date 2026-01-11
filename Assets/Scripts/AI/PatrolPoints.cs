using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolPoints : MonoBehaviour
{
    [SerializeField] private float patrolRadius = 12f;
    [SerializeField] private float minDistanceBetweenPoints = 3f;
    [SerializeField] private int maxAttemptsPerPoint = 20;

    [Header("Waypoint GameObjects (children in prefab)")]
    [SerializeField] private List<GameObject> waypointObjects = new();

    IEnumerator Start()
    {
        // Validate references early
        if (waypointObjects == null || waypointObjects.Count == 0)
        {
            Debug.LogError($"{name}: No waypointObjects assigned.");
            yield break;
        }

        for (int i = 0; i < waypointObjects.Count; i++)
        {
            if (waypointObjects[i] == null)
            {
                Debug.LogError($"{name}: waypointObjects[{i}] is NULL.");
                yield break;
            }
        }

        // Let NavMesh + BehaviorGraph initialize
        yield return null;

        DetachWaypoints();
        PlaceWaypoints();
    }

    void DetachWaypoints()
    {
        foreach (var wp in waypointObjects)
        {
            wp.transform.SetParent(null, true); // detach, keep world position
        }
    }

    void PlaceWaypoints()
    {
        Vector3 origin = transform.position;
        List<Vector3> placed = new();

        for (int i = 0; i < waypointObjects.Count; i++)
        {
            if (TryFindValidPoint(origin, placed, out var p))
            {
                waypointObjects[i].transform.position = p;
                placed.Add(p);
            }
            else
            {
                waypointObjects[i].transform.position = origin;
            }
        }
    }

    bool TryFindValidPoint(Vector3 origin, List<Vector3> existing, out Vector3 result)
    {
        for (int attempt = 0; attempt < maxAttemptsPerPoint; attempt++)
        {
            Vector3 random = origin + Random.insideUnitSphere * patrolRadius;
            random.y = 0f;

            if (!NavMesh.SamplePosition(random, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                continue;

            if (TooClose(hit.position, existing))
                continue;

            NavMeshPath path = new NavMeshPath();
            if (!NavMesh.CalculatePath(origin, hit.position, NavMesh.AllAreas, path) ||
                path.status != NavMeshPathStatus.PathComplete)
                continue;

            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    bool TooClose(Vector3 candidate, List<Vector3> existing)
    {
        foreach (var p in existing)
        {
            if (Vector3.Distance(candidate, p) < minDistanceBetweenPoints)
                return true;
        }
        return false;
    }
}
