using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SearchTimer", story: "The actual search timer", category: "Action", id: "c0d6c228c51c0477739b74eaf7c64d00")]
public partial class SearchTimerAction : Action
{
    [SerializeReference] public BlackboardVariable<float> SearchTimeRemaining;
    [SerializeReference] public BlackboardVariable<bool> IsSearching;
    [SerializeReference] public BlackboardVariable<bool> CanSeePlayer;
    [SerializeReference] public BlackboardVariable<bool> HearsNoise;

    protected override Status OnUpdate()
    {
        if (CanSeePlayer.Value)
        {
            IsSearching.Value = false;
            return Status.Failure;
        }

        if (HearsNoise.Value)
        {
            IsSearching.Value = false;
            return Status.Failure;
        }

        SearchTimeRemaining.Value -= Time.deltaTime;

        if (SearchTimeRemaining.Value <= 0f)
        {
            SearchTimeRemaining.Value = 0f;
            IsSearching.Value = false;
            return Status.Failure; 
        }
        return Status.Running;
    }

}

