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

    // This should be the *raw* ability range (or a shared range value). We apply the buffer below.
    [SerializeReference] public BlackboardVariable<float> AttackRange = new(2.0f);

    // IMPORTANT: Chase should NOT set this true. Attack owns the lock.
    [SerializeReference] public BlackboardVariable<bool> ChaseLocked;

    public float stopBuffer = 0.25f;

    NavMeshAgent nav;
    Animator anim;

    protected override Status OnStart()
    {
        if (Agent?.Value == null || PlayerTransform?.Value == null)
            return Status.Failure;

        nav = Agent.Value.GetComponent<NavMeshAgent>();
        if (nav == null || !nav.isOnNavMesh)
            return Status.Failure;

        anim = Agent.Value.GetComponent<Animator>();

        nav.isStopped = false;

        float r = (AttackRange != null && AttackRange.Value > 0f) ? AttackRange.Value : 2.0f;
        nav.stoppingDistance = Mathf.Max(0.05f, r - stopBuffer);

        if (anim) anim.SetFloat("Speed", 1.0f);

        // Optional safety: if we ever re-enter chase, clear a stale lock.
        // If you only want Attack to clear it, delete this line.
        if (ChaseLocked != null) ChaseLocked.Value = false;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (AIState == null || AIState.Value != State.Chase)
            return Status.Failure;

        if (PlayerTransform?.Value == null)
            return Status.Failure;

        // If something still has chase locked, just wait in chase state.
        // (Prevents jitter if lock clears a frame later.)
        if (ChaseLocked != null && ChaseLocked.Value)
        {
            if (anim) anim.SetFloat("Speed", 0.0f);
            if (nav && nav.isOnNavMesh) nav.isStopped = true;
            return Status.Running;
        }

        if (nav && nav.isOnNavMesh)
        {
            nav.isStopped = false;
            nav.SetDestination(PlayerTransform.Value.position);
        }

        if (!nav.pathPending && nav.remainingDistance <= nav.stoppingDistance)
        {
            AIState.Value = State.Attack;
            if (anim) anim.SetFloat("Speed", 0.0f);

            // DO NOT set ChaseLocked here. Attack owns the lock.
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
