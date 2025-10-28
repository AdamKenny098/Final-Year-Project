// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-07-16
// Description: Handles the player's inventory system, including adding, removing, and transferring items.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // List of Inventory Slots
    public List<InventorySlot> invSlots = new List<InventorySlot>();

    // Adds an item to the inventory, stacking if possible.
    public bool AddItem(Item item, int amount)
    {
        // First try stacking
        foreach (InventorySlot slot in invSlots)
        {
            if (slot.item == item && slot.amount < item.maxStack)
            {
                int spaceLeft = item.maxStack - slot.amount;
                int addAmount = Mathf.Min(spaceLeft, amount);

                slot.amount += addAmount;
                amount -= addAmount;

                if (amount <= 0)
                {
                    return true;
                }
            }
        }

        // If there's still some left add to a new slot
        InventorySlot newSlot = new InventorySlot
        {
            item = item,
            amount = amount
        };
        invSlots.Add(newSlot);
        return true;
    }

    // Removes an item from a given slot.
    public void RemoveItemAt(int index)
    {
        if (index >= 0 && index < invSlots.Count)
        {
            invSlots.RemoveAt(index);
        }
    }

    // Transfers an item from this inventory to another.
    public bool TransferTo(Inventory targetInventory, int index)
    {
        if (index < 0 || index >= invSlots.Count)
        {
            return false;
        }
        
        InventorySlot slot = invSlots[index];

        bool wasAdded = targetInventory.AddItem(slot.item, slot.amount);

        if (wasAdded)
        {
            invSlots.RemoveAt(index);
            return true;
        }

        return false;
    }
}
