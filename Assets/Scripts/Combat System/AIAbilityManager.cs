using UnityEngine;

public class AIAbilityManager : MonoBehaviour
{
    public Entity owner;
    public AbilityManager abilityManager;

    [Header("Target")]
    public Transform target;
    public float thinkRate = 0.2f;

    [Header("Animation")]
    public Animator anim;
    public string speedFloat = "Speed";
    public string attackTrigger = "Attack";
    public float minTimeBetweenAnimTriggers = 0.2f;

    float nextThink;
    float nextAnimAllowed;

    void Awake()
    {
        if (owner == null) owner = GetComponentInParent<Entity>();
        if (abilityManager == null) abilityManager = GetComponentInParent<AbilityManager>();
        if (abilityManager != null) abilityManager.owner = owner;
        if (anim == null) anim = GetComponentInParent<Animator>();
        if (target == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    void Update()
    {
        if (owner == null || owner.isDead) return;
        if (abilityManager == null || abilityManager.loadout == null) return;

        if (Time.time < nextThink) return;
        nextThink = Time.time + thinkRate;

        if (target == null) return;

        Entity targetEntity = target.GetComponentInParent<Entity>();
        if (targetEntity == null || targetEntity.isDead) return;

        Vector3 hitPoint = targetEntity.transform.position;

        TryOnlySlot(targetEntity, hitPoint);
    }

    void TryOnlySlot(Entity targetEntity, Vector3 hitPoint)
    {
        TrySlot(0, targetEntity, hitPoint);
    }

    bool TrySlot(int slot, Entity targetEntity, Vector3 hitPoint)
    {
        if (!abilityManager.IsReady(slot)) return false;

        AbilityData abilityData = abilityManager.GetAbility(slot);
        if (abilityData == null) return false;

        if (abilityData.range > 0f)
        {
            float distance = Vector3.Distance(owner.transform.position, targetEntity.transform.position);
            if (distance > abilityData.range) return false;
        }

        bool ok = abilityManager.TryCast(slot, abilityData, targetEntity, hitPoint);
        if (!ok) return false;

        if (anim != null && Time.time >= nextAnimAllowed)
        {
            if (!string.IsNullOrEmpty(speedFloat)) anim.SetFloat(speedFloat, 0f);
            if (!string.IsNullOrEmpty(attackTrigger)) anim.SetTrigger(attackTrigger);
            nextAnimAllowed = Time.time + minTimeBetweenAnimTriggers;
        }

        return true;
    }

    public bool TryAttackNow(Entity targetEntity, Vector3 hitPoint)
    {
        if (owner == null || owner.isDead) return false;
        if (abilityManager == null || abilityManager.loadout == null) return false;
        if (targetEntity == null || targetEntity.isDead) return false;
        return TrySlot(0, targetEntity, hitPoint);
    }

}
