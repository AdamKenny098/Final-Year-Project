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
        Vector3 dir = (Agent.Value.transform.position - Player.Value.position).normalized;
        Vector3 desiredPos = Agent.Value.transform.position + dir * fleeDistance;

        if (NavMesh.SamplePosition(desiredPos, out NavMeshHit hit, 3f, NavMesh.AllAreas))
        {
            if (FleeTarget.Value == null)
            {
                GameObject temp = new GameObject("FleeTarget");
                FleeTarget.Value = temp.transform;
            }

            FleeTarget.Value.position = hit.position;
        }

        return Status.Running;
    }
}

