using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Disable AI", story: "Disables the behavior tree. No loose ends", category: "Action", id: "658df4c9aaa9b39e768bd76e983a2bcf")]
public partial class DisableAiAction : Action
{
    protected override Status OnStart()
    {
        var behavior = GameObject.GetComponent<BehaviorGraphAgent>();

        if (behavior != null)
        {
            behavior.enabled = false;
        }

        return Status.Success;
    }
}

