using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopSystem : MonoBehaviour
{
    public static ShopSystem Instance;

    [Header("References")]
    public Inventory playerInventory;
    public Inventory merchantInventory;
    public int playerGold;
    public GameObject shopUI;
    public GameObject middlePanel;

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
        ToggleTrade(false);
    }
    
    public int GetPlayerGold()
    {
        playerGold = 0;
        foreach (var slot in playerInventory.invSlots)
        {
            Item item = slot.item;
            if (item != null && item.name == "Gold")
            {
                playerGold += slot.amount;
            }
        }
        return playerGold;
    }

    // === SHOP OPEN/CLOSE ===
    public void OpenShop(Inventory merchant)
    {
        merchantInventory = merchant;

        ToggleTrade(true);
    }

    public void CloseShop()
    {
        ToggleTrade(false);
        merchantInventory = null;
    }

    // === BUYING/SELLING ===
    public bool BuyItem(Item item)
    {
        if (merchantInventory == null || item == null)
            return false;

        if (playerGold < item.value)
        {
            return false;
        }

        if (merchantInventory.RemoveItem(item, 1))
        {
            playerInventory.AddItem(item, 1);
            playerGold -= item.value;
            InventoryUI.Instance.BuildTradeLists();
            return true;
        }

        return false;
    }

    public bool SellItem(Item item, int amount = 1)
    {
        if (merchantInventory == null || item == null)
            return false;

        // Transfer item from player → merchant
        if (playerInventory.RemoveItem(item, amount))
        {
            merchantInventory.AddItem(item, amount);
            playerGold += item.value;
            InventoryUI.Instance.BuildTradeLists();
            return true;
        }

        return false;
    }

    public void ToggleTrade(bool isTrading)
    {
        if (isTrading)
        {
            if (DialogueSystem.Instance.canContinueToNextLine)
            {
                InventoryUI.Instance.BuildTradeLists();
                InventoryUI.Instance.OpenTrade(merchantInventory);
                shopUI.SetActive(isTrading);
            }
        }

        else
        {
            shopUI.SetActive(isTrading);
            middlePanel.SetActive(isTrading);
        }
        
        

        //Add the ability to disable player movement here
    }
}
