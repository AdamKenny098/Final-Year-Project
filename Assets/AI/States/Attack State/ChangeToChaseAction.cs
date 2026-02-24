using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ChangeToChase", story: "Agent chases Target", category: "Action", id: "b7e3b15b73f49d28f8c7829b0dc9891c")]
public partial class ChangeToChaseAction : Action
{
    [SerializeReference] public BlackboardVariable<State> AIState;
    [SerializeReference] public BlackboardVariable<bool> ChaseLocked;

    protected override Status OnUpdate()
    {
        if (ChaseLocked != null) ChaseLocked.Value = false;
        if (AIState != null) AIState.Value = State.Chase;
        return Status.Success;
    }
}

