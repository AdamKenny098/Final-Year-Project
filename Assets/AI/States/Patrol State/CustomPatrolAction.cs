using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CustomPatrolAction", story: "[Target] patrols [points]", category: "Action", id: "573d3f416e28e1aaafb275c9e77c15c3")]
public class PatrolStateAction : Action
{
    [SerializeReference] public BlackboardVariable<State> AIState;
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<List<GameObject>> Waypoints;
    NavMeshAgent nav;
    int index;

    protected override Status OnStart()
    {
        nav = Agent.Value.GetComponent<NavMeshAgent>();
        nav.isStopped = false;
        index = 0;
        nav.SetDestination(Waypoints.Value[index].transform.position);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (AIState.Value != State.Patrol)
        {
            nav.ResetPath();
            return Status.Failure;
        }

        if (!nav.pathPending && nav.remainingDistance <= nav.stoppingDistance)
        {
            index = (index + 1) % Waypoints.Value.Count;
            nav.SetDestination(Waypoints.Value[index].transform.position);
        }
        return Status.Running;
    }
}


