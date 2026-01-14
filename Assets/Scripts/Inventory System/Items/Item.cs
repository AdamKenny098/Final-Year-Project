using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Item")]
public abstract class Item : ScriptableObject
{
    public string name;
    public int maxStack = 100;
    public Sprite icon;
    public int rarity = 1; // 1 Common to 5 Legendary
    public int value = 10;
    public bool isSellable = true;
}
