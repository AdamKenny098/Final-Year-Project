using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SelectSearchTargetAction", story: "Selects what the AI should search for", category: "Action", id: "d490182665038579646e826e3b6660d4")]
public partial class SelectSearchTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<State> AIState;
    [SerializeReference] public BlackboardVariable<Transform> LastKnownPlayerTransform;
    [SerializeReference] public BlackboardVariable<Transform> SoundPosition;
    [SerializeReference] public BlackboardVariable<Transform> SearchTarget;
    [SerializeReference] public BlackboardVariable<SearchSourceType> SearchSource;

    protected override Status OnUpdate()
    {
        // Only valid during Search state
        if (AIState.Value != State.Search)
            return Status.Failure;

        // 1: Player
        if (LastKnownPlayerTransform.Value != null)
        {
            SearchTarget.Value = LastKnownPlayerTransform.Value;
            SearchSource.Value = SearchSourceType.Player;
            return Status.Success;
        }

        // 2: Noise
        if (SoundPosition.Value != null)
        {
            SearchTarget.Value = SoundPosition.Value;
            SearchSource.Value = SearchSourceType.Noise;
            return Status.Success;
        }

        // Nothing to search
        SearchTarget.Value = null;
        SearchSource.Value = SearchSourceType.None;
        return Status.Failure;
    }
}

