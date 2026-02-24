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
    [SerializeReference] public BlackboardVariable<GameObject> Threat;
    [SerializeReference] public BlackboardVariable<Transform> FleeTarget;

    public float fleeDistance = 8f;
    public float sampleRadius = 3f;

    [Header("Panic")]
    public float directionHoldTime = 0.6f;     // how long to keep one “panic direction”
    public float sidewaysJitter = 0.9f;        // how much side-to-side randomness
    public float backBias = 1.2f;              // >1 means “prefer away from threat”
    public float randomAngleDegrees = 35f;     // adds cone randomness around away dir

    float nextRepickTime;
    Vector3 chosenDir;

    protected override Status OnStart()
    {
        nextRepickTime = 0f;
        chosenDir = Vector3.zero;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Agent?.Value == null) return Status.Failure;
        if (Threat?.Value == null) return Status.Failure;
        if (FleeTarget == null) return Status.Failure;

        Vector3 agentPos = Agent.Value.transform.position;
        Vector3 threatPos = Threat.Value.transform.position;

        // Base “away” direction
        Vector3 away = (agentPos - threatPos);
        away.y = 0f;

        if (away.sqrMagnitude < 0.0001f)
            away = Agent.Value.transform.forward;
        else
            away.Normalize();

        // Periodically repick a new panic direction (zig-zag)
        if (Time.time >= nextRepickTime || chosenDir.sqrMagnitude < 0.0001f)
        {
            nextRepickTime = Time.time + Mathf.Max(0.05f, directionHoldTime);

            // 1) cone around away direction
            float angle = UnityEngine.Random.Range(-randomAngleDegrees, randomAngleDegrees);
            Vector3 coneDir = Quaternion.AngleAxis(angle, Vector3.up) * away;

            // 2) add sideways jitter
            Vector3 right = new Vector3(-away.z, 0f, away.x); // perpendicular on XZ
            float side = UnityEngine.Random.Range(-sidewaysJitter, sidewaysJitter);

            // 3) combine with bias to keep “mostly away”
            Vector3 raw = (coneDir * backBias) + (right * side);
            raw.y = 0f;

            if (raw.sqrMagnitude < 0.0001f) raw = away;
            chosenDir = raw.normalized;
        }

        Vector3 desiredPos = agentPos + chosenDir * fleeDistance;

        if (!NavMesh.SamplePosition(desiredPos, out NavMeshHit hit, sampleRadius, NavMesh.AllAreas))
        {
            // If sample fails, force a repick next tick instead of failing the whole flee
            nextRepickTime = 0f;
            return Status.Running;
        }

        if (FleeTarget.Value == null)
        {
            GameObject temp = new GameObject("FleeTarget");
            FleeTarget.Value = temp.transform;
        }

        FleeTarget.Value.position = hit.position;
        return Status.Running;
    }
}