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

    [SerializeReference] public BlackboardVariable<float> Health; // add this var in graph if you want death safety

    protected override Status OnUpdate()
    {
        // If dead, stop fleeing immediately
        if (Health != null && Health.Value <= 0f)
        {
            IsFleeing.Value = false;
            IsLowHealth.Value = false;
            PanicTimeRemaining.Value = 0f;
            return Status.Failure;
        }

        PanicTimeRemaining.Value -= Time.deltaTime;

        if (PanicTimeRemaining.Value <= 0f)
        {
            PanicTimeRemaining.Value = 0f;
            IsFleeing.Value = false;
            // Keep IsLowHealth as-is (LowHealthAction will recompute it)
            return Status.Failure;
        }

        return Status.Running;
    }
}

