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
}
