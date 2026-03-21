using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "InvestigateTargetAction", story: "Agent moves to Target", category: "Action", id: "7286853cf702193036db9d3b4d43b237")]
public partial class InvestigateTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<State> AIState;
    [SerializeReference] public BlackboardVariable<Transform> Target;
    [SerializeReference] public BlackboardVariable<float> ArrivalDistance = new(1.5f);

    NavMeshAgent nav;
    Vector3 cachedTargetPosition;

    protected override Status OnStart()
    {
        if (Agent?.Value == null || Target?.Value == null)
            return Status.Failure;

        nav = Agent.Value.GetComponent<NavMeshAgent>();
        if (!nav || !nav.isOnNavMesh)
            return Status.Failure;

        cachedTargetPosition = Target.Value.position;

        nav.isStopped = false;
        nav.stoppingDistance = ArrivalDistance.Value;
        nav.SetDestination(cachedTargetPosition);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (AIState.Value != State.Search)
            return Status.Success;

        if (!nav.pathPending && nav.remainingDistance <= nav.stoppingDistance)
        {
            return Status.Success;
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (nav && nav.isOnNavMesh)
            nav.ResetPath();
    }
}

