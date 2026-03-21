using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SearchTimer", story: "The actual search timer", category: "Action", id: "c0d6c228c51c0477739b74eaf7c64d00")]
public partial class SearchTimerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<State> AIState;

    public float rotationSpeed = 120f;
    public float minTurnAngle = 60f;
    public float maxTurnAngle = 180f;

    Transform agentTransform;
    NavMeshAgent nav;
    float targetYaw;

    protected override Status OnStart()
    {
        if (Agent?.Value == null)
            return Status.Failure;

        agentTransform = Agent.Value.transform;
        nav = Agent.Value.GetComponent<NavMeshAgent>();

        if (nav != null)
        {
            nav.isStopped = true;
            nav.updateRotation = false;
        }

        PickNewYaw();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (AIState.Value != State.Search)
            return Status.Success;

        float currentY = agentTransform.eulerAngles.y;
        float newY = Mathf.MoveTowardsAngle(
            currentY,
            targetYaw,
            rotationSpeed * Time.deltaTime
        );

        agentTransform.rotation = Quaternion.Euler(0f, newY, 0f);

        if (Mathf.Abs(Mathf.DeltaAngle(newY, targetYaw)) < 1f)
            PickNewYaw();

        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (nav != null)
        {
            nav.updateRotation = true;
            nav.isStopped = false;
        }
    }

    void PickNewYaw()
    {
        float delta = UnityEngine.Random.Range(minTurnAngle, maxTurnAngle);
        if (UnityEngine.Random.value > 0.5f)
            delta = -delta;

        targetYaw = agentTransform.eulerAngles.y + delta;
    }
}

