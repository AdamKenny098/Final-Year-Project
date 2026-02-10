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
    [SerializeReference] public BlackboardVariable<bool> IsLowHealth;
    [SerializeReference] public BlackboardVariable<bool> IsFleeing; // optional safety

    protected override Status OnStart()
    {
        if (Health == null || MaxHealth == null || LowHealthThreshold == null || IsLowHealth == null)
            return Status.Failure;

        if (LowHealthThreshold.Value <= 0f)
            LowHealthThreshold.Value = MaxHealth.Value * 0.15f;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        float max = MaxHealth.Value;
        float hp = Health.Value;

        // Dead = not fleeing
        if (hp <= 0f)
        {
            IsLowHealth.Value = false;
            if (IsFleeing != null) IsFleeing.Value = false;
            return Status.Running;
        }

        float threshold = LowHealthThreshold.Value;
        if (threshold <= 0f)
            LowHealthThreshold.Value = max * 0.15f;

        bool low = hp <= threshold;
        IsLowHealth.Value = low;

        return Status.Running;
    }
}