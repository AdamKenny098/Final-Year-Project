using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AttackAnim", story: "Agent plays attack anim", category: "Action", id: "988cbc95f4c9140b9adc42c4527e188f")]
public partial class AttackAnimAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<bool> ChaseLocked;
    [SerializeReference] public BlackboardVariable<bool> IsAttacking;
    public float fallbackDuration = 0.6f;
    public int fixedAttack = 1;
    Animator anim;
    float endTime;

    protected override Status OnStart()
    {
        if (Self.Value == null) return Status.Failure;

        anim = Self.Value.GetComponentInChildren<Animator>();
        if (anim == null) return Status.Failure;

        int atk = UnityEngine.Random.Range(1, 3);
        anim.SetInteger("Attack", atk);

        if (ChaseLocked != null) ChaseLocked.Value = true;
        if (IsAttacking != null) IsAttacking.Value = true;

        endTime = Time.time + fallbackDuration;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (anim == null) return Status.Failure;
        return (Time.time < endTime) ? Status.Running : Status.Success;
    }

    protected override void OnEnd()
    {
        if (IsAttacking != null) IsAttacking.Value = false;
        if (anim != null) anim.SetInteger("Attack", 0);
    }
}

