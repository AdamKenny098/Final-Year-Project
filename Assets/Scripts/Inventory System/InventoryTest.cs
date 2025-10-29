using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryTest : MonoBehaviour
{
    public InventoryUI inventoryUI;
    public Inventory playerInventory;
    public Inventory chestInventory;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            inventoryUI.OpenTrade(chestInventory);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            playerInventory.AddItem(chestInventory.invSlots[0].item, 1);
            Debug.Log("Added one item to player inventory!");
        }
    }
}
