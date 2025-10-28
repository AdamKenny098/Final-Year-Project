using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Item")]
public class Item : ScriptableObject
{
    public string name;
    public int maxStack = 100;
    public Sprite icon;
}
