// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-07-16
// Description: Handles the UI logic for a single inventory slot, including setting icons, quantities, and click interactions.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI amountText;
    public int slotIndex; // Set when you create the slot
    public InventoryUI ownerUI; // Reference to who owns this slot
    public Inventory inventoryOwner; // Reference to the inventory this slot belongs to

    // Gets the InventoryUI owner at start.
    public void Start()
    {
        ownerUI = GetComponentInParent<InventoryUI>();
    }

    // Sets the icon and amount text for this slot.
    public void Set(Item item, int amount)
    {
        if (item)
        {
            icon.enabled = true;
            icon.sprite = item.icon != null ? item.icon : null;
            nameText.text = item.name;

            if (amount > 1)
            {
                amountText.text = amount.ToString();
            }
            else
            {
                amountText.text = "";
            }
        }
        else
        {
            icon.enabled = false;
            icon.sprite = null; // Clears out old icons
            amountText.text = "";
        }
    }
}
