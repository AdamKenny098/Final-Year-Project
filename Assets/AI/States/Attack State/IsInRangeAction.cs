using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "IsInRange", story: "Checks if target in range", category: "Action", id: "058d0ffeeb70dbc30216b870e12f4d66")]
public partial class IsInRangeAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Transform> CombatTarget;
    [SerializeReference] public BlackboardVariable<int> Slot;
    [SerializeReference] public BlackboardVariable<bool> IsInRange;

    // how much closer than ability range
    public float beyondInRange = 0.5f;
    public float fallbackRange = 2.5f;

    protected override Status OnUpdate()
    {
        if (Self.Value == null) return Status.Failure;
        if (CombatTarget.Value == null) return Status.Failure;
        if (IsInRange == null) return Status.Failure;

        var abilities = Self.Value.GetComponentInChildren<AbilityManager>();
        float newRange = fallbackRange;

        if (abilities != null)
        {
            int slot = Slot?.Value ?? 0;
            var ability = abilities.GetAbility(slot);
            if (ability != null && ability.range > 0f)
            {
                newRange = ability.range;
            }
        }

        float effectiveRange = Mathf.Max(0.05f, newRange - beyondInRange);
        Vector3 a = Self.Value.transform.position;
        Vector3 b = CombatTarget.Value.position;
        a.y = 0f; 
        b.y = 0f;

        float distance = Vector3.Distance(a, b);
        IsInRange.Value = distance <= effectiveRange;
        return Status.Success;
    }
}