using UnityEngine;
using System.Collections;

public class AIAbilityManager : MonoBehaviour
{
    public Entity owner;
    public AbilityManager abilityManager;

    [Header("Animation")]
    public EnemyAnimationController animController;
    public float minTimeBetweenAnimTriggers = 0.2f;
    public string attackStateTag = "Attack";

    float nextAnimAllowed;

    bool attackAnimating;
    Coroutine attackResetRoutine;

    void Awake()
    {
        if (owner == null) owner = GetComponentInParent<Entity>();
        if (abilityManager == null) abilityManager = GetComponentInParent<AbilityManager>();
        if (abilityManager != null) abilityManager.owner = owner;
        if (animController == null) animController = GetComponentInParent<EnemyAnimationController>();
    }

    public bool TrySlot(int slot, Entity targetEntity, Vector3 hitPoint)
    {
        if (owner == null || owner.isDead)
            return false;

        if (abilityManager == null)
            return false;

        if (targetEntity == null || targetEntity.isDead)
            return false;

        if (attackAnimating)
            return false;

        bool ready = abilityManager.IsReady(slot);
        if (!ready)
            return false;

        AbilityData abilityData = abilityManager.GetAbility(slot);
        if (abilityData == null)
            return false;

        if (abilityData.range > 0f)
        {
            float distance = Vector3.Distance(owner.transform.position, targetEntity.transform.position);

            if (distance > abilityData.range)
                return false;
        }

        bool ok = abilityManager.TryCast(slot, abilityData, targetEntity, hitPoint);

        if (!ok)
            return false;

        if (animController == null)
            return true;

        if (Time.time < nextAnimAllowed)
            return true;

        int attackValue = Random.value < 0.5f ? 1 : 2;
        animController.PlayAttack(attackValue);

        attackAnimating = true;
        nextAnimAllowed = Time.time + minTimeBetweenAnimTriggers;

        if (attackResetRoutine != null)
            StopCoroutine(attackResetRoutine);

        attackResetRoutine = StartCoroutine(ResetAttackWhenStateFinishes());

        return true;
    }

    public bool TryAttackNow(int slot, Entity targetEntity, Vector3 hitPoint)
    {
        if (owner == null || owner.isDead) return false;
        if (abilityManager == null || abilityManager.loadout == null) return false;
        if (targetEntity == null || targetEntity.isDead) return false;

        return TrySlot(slot, targetEntity, hitPoint);
    }

    public bool TryAttackNow(Entity targetEntity, Vector3 hitPoint)
    {
        return TryAttackNow(0, targetEntity, hitPoint);
    }

    IEnumerator ResetAttackWhenStateFinishes()
    {
        Animator anim = animController != null ? animController.GetAnimator() : null;
        if (anim == null)
        {
            attackAnimating = false;
            attackResetRoutine = null;
            yield break;
        }

        yield return null;

        int enterSafety = 0;

        while (anim != null)
        {
            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
            if (state.IsTag(attackStateTag))
                break;

            enterSafety++;
            if (enterSafety > 300)
            {
                attackAnimating = false;
                attackResetRoutine = null;
                yield break;
            }

            yield return null;
        }

        if (animController != null)
            animController.ResetAttack();

        int exitSafety = 0;

        while (anim != null)
        {
            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
            if (!state.IsTag(attackStateTag))
                break;

            exitSafety++;
            if (exitSafety > 300)
            {
                attackAnimating = false;
                attackResetRoutine = null;
                yield break;
            }

            yield return null;
        }

        attackAnimating = false;
        attackResetRoutine = null;
    }
}