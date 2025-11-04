using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Item/ConsumableItem")]
public class ConsumableItem : Item
{
    public enum ConsumableType
    {
        HealthPotion,
        ManaPotion,
        StaminaPotion,
    }

    public ConsumableType consumableType;
    public int restoreValue;
    public int duration;
}
