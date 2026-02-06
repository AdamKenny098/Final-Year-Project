using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Vision", story: "Calculating if Agent Can See [Player]", category: "Action", id: "7082f207e7e538659c549d5d6bbbe5f0")]
public partial class VisionAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<bool> CanSeePlayer;
    [SerializeReference] public BlackboardVariable<Transform> PlayerTransform;
    [SerializeReference] public BlackboardVariable<Transform> LastKnownPlayerTransform;

    [Header("Vision Settings")]
    public float viewDistance = 12f;
    public float viewAngle = 90f;

    Transform eyeOrigin;
    Transform player;

    protected override Status OnStart()
    {
        if (Agent.Value == null)
            return Status.Failure;

        eyeOrigin = Agent.Value.transform;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (!player)
            return Status.Running;

        bool canSee = ComputeVision();

        CanSeePlayer.Value = canSee;

        if (canSee)
        {
            PlayerTransform.Value = player;
            LastKnownPlayerTransform.Value = player;
        }

        if (!canSee)
        {
            PlayerTransform.Value = null;
        }

        return Status.Running;
    }

    bool ComputeVision()
    {
        Vector3 origin = eyeOrigin.position + Vector3.up * 1.6f;
        Vector3 target = player.position + Vector3.up * 1.2f;
        Vector3 dir = (target - origin);
        float dist = dir.magnitude;

        if (dist > viewDistance) return false;
        if (Vector3.Angle(eyeOrigin.forward, dir) > viewAngle * 0.5f) return false;

        var hits = Physics.RaycastAll(origin, dir.normalized, viewDistance, ~0, QueryTriggerInteraction.Ignore);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (var h in hits)
        {
            if (!h.transform) continue;

            // skip any collider belonging to this agent
            if (h.transform.root == Agent.Value.transform.root)
                continue;

            Debug.Log($"[Vision] First non-self hit: {h.collider.name} | Root: {h.transform.root.name} | Tag: {h.collider.tag}", h.collider);

            return h.transform.CompareTag("Player") || h.transform.root.CompareTag("Player");
        }

        return false;
    }

}

