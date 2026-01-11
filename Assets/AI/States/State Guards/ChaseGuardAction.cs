using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ChaseGuardAction", story: "Fail if not in Chase state or target lost", category: "Action", id: "f19fcd09b25efa762b095631af62b4b8")]
public class ChaseGuardAction : Action
{
    [SerializeReference] public BlackboardVariable<State> AIState;
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Transform> Target;
    [SerializeReference] public BlackboardVariable<bool> IsFleeing;

    NavMeshAgent nav;

    protected override Status OnStart()
    {
        if (Agent?.Value == null)
            return Status.Failure;

        nav = Agent.Value.GetComponent<NavMeshAgent>();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (IsFleeing.Value)
        {
            return Status.Failure;
        }

        if (AIState.Value != State.Chase)
        {
            if (nav && nav.isOnNavMesh)
                nav.ResetPath();

            return Status.Failure;
        }

        if (Target?.Value == null)
        {
            if (nav && nav.isOnNavMesh)
                nav.ResetPath();

            return Status.Failure;
        }

        return Status.Success;
    }
}

