using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PanicTimerAction", story: "Controls Panic Duration", category: "Action", id: "77f6911fc90fc67981ee04a901eb7212")]
public partial class PanicTimerAction : Action
{
    [SerializeReference] public BlackboardVariable<float> PanicTimeRemaining;
    [SerializeReference] public BlackboardVariable<bool> IsFleeing;
    [SerializeReference] public BlackboardVariable<bool> IsLowHealth;
    [SerializeReference] public BlackboardVariable<float> CurrentHealth;
    [SerializeReference] public BlackboardVariable<float> HealthThreshold;
    [SerializeReference] public BlackboardVariable<State> AIState;

    protected override Status OnUpdate()
    {
        PanicTimeRemaining.Value -= Time.deltaTime;

        if (PanicTimeRemaining.Value <= 0f)
        {
            PanicTimeRemaining.Value = 0f;
            CurrentHealth.Value = Math.Max(CurrentHealth.Value, HealthThreshold.Value + 1f); // Ensure above threshold
            AIState.Value = State.Patrol;
            IsFleeing.Value = false;
            IsLowHealth.Value = false;
            return Status.Failure;
        }

        return Status.Running;
    }
}

