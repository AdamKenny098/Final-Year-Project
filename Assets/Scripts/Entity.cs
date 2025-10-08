using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public string species;
    public bool isHostile;
    public int currentHitPoints, maxHitPoints, attack, defense, speed, stamina, mana, level, height;
    // Start is called before the first frame update
    void Start()
    {
        currentHitPoints = maxHitPoints;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void TakeDamage(int amount)
    {
        currentHitPoints = currentHitPoints - amount;
        if (currentHitPoints <= 0)
        {
            Die();
        }
    }

    void Heal(int amount)
    {
        currentHitPoints = currentHitPoints + amount;
        if (currentHitPoints >= maxHitPoints)
        {
            currentHitPoints = maxHitPoints;
        }
    }

    void Die()
    {
        GameObject.Destroy(this);
    }
}
