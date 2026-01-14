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
    public Item goldItem;

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
            if (slot.item == goldItem)
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

    // === BUYING/SELLING ===
    public bool BuyItem(Item item)
    {
        if (merchantInventory == null || item == null)
            return false;

        int price = item.value;

        // Check player can afford
        if (!playerInventory.HasItem(goldItem, price))
            return false;

        // Pay gold
        playerInventory.RemoveItem(goldItem, price);
        merchantInventory.AddItem(goldItem, price);

        // Transfer item
        merchantInventory.RemoveItem(item, 1);
        playerInventory.AddItem(item, 1);

        NotifyTradeChanged();
        return true;
    }


    public bool SellItem(Item item, int amount = 1)
    {
        if (merchantInventory == null || item == null)
            return false;

        if (!item.isSellable)
            return false;

        int payout = item.value * amount;

        // Check player has item
        if (!playerInventory.HasItem(item, amount))
            return false;

        // Check merchant can pay
        if (!merchantInventory.HasItem(goldItem, payout))
            return false;
        
        playerInventory.RemoveItem(item, amount);
        merchantInventory.AddItem(item, amount);

        merchantInventory.RemoveItem(goldItem, payout);
        playerInventory.AddItem(goldItem, payout);

        NotifyTradeChanged();
        return true;
    }

    public void NotifyTradeChanged()
    {
        InventoryUI.Instance.OnTradeChanged();
    }


}
