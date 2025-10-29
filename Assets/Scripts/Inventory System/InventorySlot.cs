// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-07-16
// Description: Represents an inventory slot that stores an item and its quantity. Includes logic for clearing and checking emptiness.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Represents a slot in the inventory with its item and quantity.
[System.Serializable]
public class InventorySlot
{
    public Item item;    // Stores the item in the slot.
    public int amount;   // Stores the number of items in the slot.
}
