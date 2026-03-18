using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ShopSystem : MonoBehaviour
{
    public static ShopSystem Instance;
    public ShopUI shopUI;

    [Header("References")]
    public Inventory playerInventory;
    public Inventory merchantInventory;
    public Item goldItem;

    public NPC currentMerchant;

    private Item barteredItem = null;
    private int barteredPrice = 0;

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
        currentMerchant = merchant.GetComponentInParent<NPC>();

        MerchantStockGenerator generator = merchant.GetComponentInParent<MerchantStockGenerator>();

        if (generator != null)
        {
            generator.GenerateStock();
        }
        
        InventoryUI.Instance.OpenTrade(merchantInventory);
        InventoryUI.Instance.BuildTradeLists();

        GameStates.Instance.SetState(GameState.Trading);
        shopUI.ShowShop();
    }

    public void CloseShop()
    {
        InventoryUI.Instance.CloseTrade();
        shopUI.HideShop();

        GameStates.Instance.SetState(GameState.Talking);
        DialogueSystem.Instance.ResumeDialogue();
    }

    // === BUYING/SELLING ===
    public bool BuyItem(Item item)
    {
        if (merchantInventory == null || item == null)
            return false;

        int price = GetValue(item);

        if (item == barteredItem)
        {
            price = barteredPrice;
        }

        // Check player can afford
        if (!playerInventory.HasItem(goldItem, price))
            return false;

        // Pay gold
        playerInventory.RemoveItem(goldItem, price);
        merchantInventory.AddItem(goldItem, price);

        // Transfer item
        merchantInventory.RemoveItem(item, 1);
        playerInventory.AddItem(item, 1);

        barteredItem = null;
        barteredPrice = 0;

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

    bool RollCheck(int statValue, int targetNumber)
    {
        int roll = Random.Range(1, 21); // d20
        int modifier = (statValue - 10) / 2;

        return roll + modifier >= targetNumber;
    }

    int CalculateTargetNumber(int itemValue)
    {
        int baseTarget = 8;
        int valueDivisor = 10;

        return baseTarget + (itemValue / valueDivisor);
    }

    public bool AttemptBarter(Item item, Character player)
    {
        if (item == null || player == null) return false;

        int target = CalculateTargetNumber(item.value);

        bool success = RollCheck(player.stats.charisma, target);

        if (success)
        {
            BarterSuccess(item);
        }
        else
        {
            BarterFailure();
        }

        return success;
    }

    void BarterSuccess(Item item)
    {
        barteredItem = item;
        barteredPrice = CalculateBarteredPrice(item);
        DialogueSystem.Instance.SetTradeOutcome("barter_success");


        InventoryUI.Instance.OnTradeChanged(item);
        TradeFeedbackUI.Instance.Show($"Barter successful — new price: {barteredPrice}g");
    }

    void BarterFailure()
    {
        DialogueSystem.Instance.SetTradeOutcome("barter_refused");
        TradeFeedbackUI.Instance.Show("The merchant refuses to negotiate.");

        CloseShop();
    }


    int CalculateBarteredPrice(Item item)
    {
        int baseValue = item.value;

        int minPrice = Mathf.Max(1, Mathf.RoundToInt(baseValue * 0.5f));
        int maxPrice = baseValue;

        return Random.Range(minPrice, maxPrice + 1);
    }


    public bool AttemptSteal(Item item, Character player)
    {

        if (item == null || player == null) return false;

        int stealRiskBonus = 2; // easy tuning knob
        int target = CalculateTargetNumber(item.value) + stealRiskBonus;

        bool success = RollCheck(
            player.stats.dexterity,
            target
        );

        if (success)
        {
            StealSuccess(item);
        }
        else
        {
            StealFailure();
        }

        return success;
    }

    void StealSuccess(Item item)
    {
        if (playerInventory == null || merchantInventory == null)
            return;

        if (!merchantInventory.HasItem(item, 1))
        return;

        bool added = playerInventory.AddItem(item, 1);
        if (!added) return;

        merchantInventory.RemoveItem(item, 1);
        
        barteredItem = null;
        barteredPrice = 0;

        DialogueSystem.Instance.SetTradeOutcome("steal_success");

        InventoryUI.Instance.OnTradeChanged();
        TradeFeedbackUI.Instance.Show($"You stole the {item.name} unnoticed.");
    }

    void StealFailure()
    {
        if (currentMerchant == null)
            return;

        currentMerchant.isAlerted = true;
        currentMerchant.requiresForgivenessQuest = true;
        DialogueSystem.Instance.SetTradeOutcome("steal_caught");
        CloseShop();
    }

    public int GetEffectivePrice(Item item)
    {
        if (item == barteredItem)
            return barteredPrice;

        return item.value;
    }

    public int GetValue(Item item)
    {
        float multiplier = Rarity.Instance.GetMultiplier(item.rarity);
        return Mathf.RoundToInt(item.value * multiplier);
    }


}
