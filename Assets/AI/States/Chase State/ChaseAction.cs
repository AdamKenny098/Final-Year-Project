using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ChaseAction", story: "[Agent] chases Player", category: "Action", id: "564161ffcb50c0b38dd1b5f2e0886b2a")]
public partial class ChaseAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<State> AIState;
    [SerializeReference] public BlackboardVariable<Transform> PlayerTransform;
    [SerializeReference] public BlackboardVariable<float> AttackRange = new(2.0f);

    NavMeshAgent nav;

    protected override Status OnStart()
    {
        if (Agent?.Value == null || PlayerTransform?.Value == null)
            return Status.Failure;

        nav = Agent.Value.GetComponent<NavMeshAgent>();
        if (nav == null || !nav.isOnNavMesh)
            return Status.Failure;

        nav.isStopped = false;
        nav.stoppingDistance = AttackRange.Value;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (AIState.Value != State.Chase)
            return Status.Failure;

        if (PlayerTransform.Value == null)
            return Status.Failure;

        nav.SetDestination(PlayerTransform.Value.position);

        if (!nav.pathPending && nav.remainingDistance <= nav.stoppingDistance)
        {
            return Status.Success;
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (nav && nav.isOnNavMesh)
        {
            nav.ResetPath();
        }
    }
}

