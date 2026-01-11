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
        Vector3 toPlayer = player.position - eyeOrigin.position;
        float distance = toPlayer.magnitude;

        if (distance > viewDistance)
            return false;

        float angle = Vector3.Angle(eyeOrigin.forward, toPlayer);
        if (angle > viewAngle * 0.5f)
            return false;

        if (Physics.Raycast(eyeOrigin.position, toPlayer.normalized, out RaycastHit hit, viewDistance))
        {
            Debug.DrawLine(eyeOrigin.position, hit.point, Color.red);
            return hit.transform.CompareTag("Player") || hit.transform.root.CompareTag("Player");
        }
        return false;
    }
}

