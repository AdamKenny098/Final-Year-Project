using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Low Health", story: "Triggers Fleeing when health is low", category: "Action", id: "e7a1cc0dffe4c7fc76834cbbc3cba006")]
public partial class LowHealthAction : Action
{
    [SerializeReference] public BlackboardVariable<float> MaxHealth;
    [SerializeReference] public BlackboardVariable<float> Health;
    [SerializeReference] public BlackboardVariable<float> LowHealthThreshold;
    [SerializeReference] public BlackboardVariable<bool> IsFleeing;
    [SerializeReference] public BlackboardVariable<bool> IsLowHealth;

    protected override Status OnStart()
    {
        if (Health == null || LowHealthThreshold == null || IsFleeing == null || MaxHealth == null)
            return Status.Failure;

        LowHealthThreshold.Value = MaxHealth.Value * 0.15f;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {   
        if (Health.Value <= LowHealthThreshold.Value)
        {
            IsFleeing.Value = true;
            IsLowHealth.Value = true;
        }

        return Status.Success; // Never blocks
    }
}

