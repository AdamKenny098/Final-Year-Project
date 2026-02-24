using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "TryUseAbility", story: "Try use ability on target", category: "Action", id: "c9a9073caa7f43cfb59c4bab974d1f03")]
public partial class TryUseAbilityAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Transform> CombatTarget;

    protected override Status OnUpdate()
    {
        if (Self.Value == null) return Status.Failure;
        if (CombatTarget.Value == null) return Status.Failure;

        var ai = Self.Value.GetComponentInChildren<AIAbilityManager>();
        if (ai == null) return Status.Failure;

        var targetEntity = CombatTarget.Value.GetComponentInParent<Entity>();
        if (targetEntity == null || targetEntity.isDead) return Status.Failure;

        Vector3 hitPoint = CombatTarget.Value.position;
        bool cast = ai.TryAttackNow(targetEntity, hitPoint);

        return cast ? Status.Success : Status.Running;
    }
}