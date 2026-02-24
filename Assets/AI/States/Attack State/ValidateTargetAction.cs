using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ValidateTarget", story: "Validates target for combat", category: "Action", id: "78e1adadbe34743515f771c9dcf52f23")]
public partial class ValidateTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<GameObject> LastAttacker;
    [SerializeReference] public BlackboardVariable<Transform> CombatTarget;
    [SerializeReference] public BlackboardVariable<GameObject> CombatTargetGO;
    [SerializeReference] public BlackboardVariable<bool> IsTargetValid;

    protected override Status OnStart()
    {
        if (IsTargetValid != null) IsTargetValid.Value = false;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        GameObject target = null;

        if (LastAttacker != null && LastAttacker.Value != null)
        {
            target = LastAttacker.Value;
        }
        else if (Player != null && Player.Value != null)
        {
            target = Player.Value;
        }
        
        if (target == null) return Status.Failure;
        var ent = target.GetComponentInParent<Entity>();
        if (ent != null && ent.isDead) return Status.Failure;

        if (CombatTarget != null)
        {
            CombatTarget.Value = target.transform;
            IsTargetValid.Value = true;
        } 
        if (CombatTargetGO != null) CombatTargetGO.Value = target;
        return Status.Success;
    }
}

