using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "StartPanicAction", story: "Initializes Panic", category: "Action", id: "c78faaa5d51882380a4c6a6f21c36166")]
public partial class StartPanicAction : Action
{
    [SerializeReference] public BlackboardVariable<float> PanicDuration;
    [SerializeReference] public BlackboardVariable<float> PanicTimeRemaining;
    [SerializeReference] public BlackboardVariable<bool> IsFleeing;

    protected override Status OnStart()
    {
        IsFleeing.Value = true;
        PanicTimeRemaining.Value = PanicDuration.Value;
        return Status.Success;
    }
}
