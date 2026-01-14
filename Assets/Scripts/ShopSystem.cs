using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopSystem : MonoBehaviour
{
    public static ShopSystem Instance;
    public ShopUI shopUI;

    [Header("References")]
    public Inventory playerInventory;
    public Inventory merchantInventory;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }

        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        GetPlayerGold();
        //ToggleTrade(false);
    }
    
    public int GetPlayerGold()
    {
        int gold = 0;
        foreach (var slot in playerInventory.invSlots)
        {
            if (slot.item != null && slot.item.name == "Gold")
                gold += slot.amount;
        }
        return gold;
    }


    // === SHOP OPEN/CLOSE ===
    public void OpenShop(Inventory merchant)
    {
        merchantInventory = merchant;

        InventoryUI.Instance.OpenTrade(merchantInventory);
        InventoryUI.Instance.BuildTradeLists();

        GameStates.Instance.SetState(GameState.Trading);
        shopUI.ShowShop();
    }

    public void CloseShop()
    {
        shopUI.HideShop();
        merchantInventory = null;

        GameStates.Instance.SetState(GameState.Talking);
        DialogueSystem.Instance.ResumeDialogue();
    }

    // // === BUYING/SELLING ===
    // public bool BuyItem(Item item)
    // {
    //     if (merchantInventory == null || item == null)
    //         return false;

    //     if (GetPlayerGold() < item.value)
    //         return false;

    //     if (merchantInventory.RemoveItem(item, 1))
    //     {
    //         playerInventory.AddItem(item, 1);
    //         playerGold -= item.value;
    //         InventoryUI.Instance.BuildTradeLists();
    //         return true;
    //     }

    //     return false;
    // }

    // public bool SellItem(Item item, int amount = 1)
    // {
    //     if (merchantInventory == null || item == null)
    //         return false;
        
    //     // Transfer item from player → merchant
    //     if (playerInventory.RemoveItem(item, amount))
    //     {
    //         merchantInventory.AddItem(item, amount);
    //         playerGold += item.value;
    //         InventoryUI.Instance.BuildTradeLists();
    //         return true;
    //     }

    //     return false;
    // }
}
