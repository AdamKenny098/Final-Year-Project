using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "UpdateFleeTargetAction", story: "Continuosly sets flee target away from player", category: "Action", id: "ea8f9098d4c4290968a4eac3ed2a7e4b")]
public partial class UpdateFleeTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Transform> Player;
    [SerializeReference] public BlackboardVariable<Transform> FleeTarget;

    public float fleeDistance = 8f;

    protected override Status OnUpdate()
    {
        if (Agent == null || Agent.Value == null)
            return Status.Failure;

        if (Player == null || Player.Value == null)
            return Status.Failure;

        Vector3 agentPos = Agent.Value.transform.position;
        Vector3 dir = (agentPos - Player.Value.position).normalized;

        // If player exactly overlaps, pick any direction
        if (dir.sqrMagnitude < 0.0001f)
            dir = Agent.Value.transform.forward;

        Vector3 desiredPos = agentPos + dir * fleeDistance;

        if (NavMesh.SamplePosition(desiredPos, out NavMeshHit hit, 3f, NavMesh.AllAreas))
        {
            if (FleeTarget.Value == null)
            {
                GameObject temp = new GameObject("FleeTarget");
                FleeTarget.Value = temp.transform;
            }

            FleeTarget.Value.position = hit.position;
            return Status.Running;
        }

        return Status.Failure;
    }
}

