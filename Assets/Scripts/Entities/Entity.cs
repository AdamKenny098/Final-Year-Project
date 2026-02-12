using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public Stats stats;

    public bool isDead => stats != null && stats.health <= 0;

    public virtual void Awake()
    {
        if (stats != null)
            stats.FillToMax();
    }

    public virtual void TakeDamage(DamageInfo info)
    {
        TakeDamage(info.amount);
    }


    public void TakeDamage(int amount)
    {
        if (isDead) return;
        stats.health -= amount;

        if (stats.health <= 0)
        {
            Die();
        }  
    }

    public void Heal(int amount)
    {
        if (stats == null) return;

        stats.health += amount;
        if (stats.health > stats.maxHealth)
            stats.health = stats.maxHealth;
    }


    public virtual void Die()
    {
        
    }

    public bool TryUseAbilityOn(Entity target, AbilityData ability, Vector3 hitPoint)
    {
        if (target == null || ability == null)
            return false;

        if (!CombatSystem.Instance.CanPayCosts(this, ability))
            return false;

        CombatSystem.Instance.PayCosts(this, ability);

        CombatResult res = CombatSystem.Instance.Resolve(this, target, ability);

        DamageInfo dmg = new DamageInfo();
        dmg.source = gameObject;
        dmg.amount = res.damage;
        dmg.type = ability.damageType;
        dmg.outcome = res.outcome;
        dmg.hitPoint = hitPoint;

        if (res.damage > 0 || res.outcome == RollOutcome.Crit || res.outcome == RollOutcome.Hit)
            target.TakeDamage(dmg);

        return true;
    }

}
