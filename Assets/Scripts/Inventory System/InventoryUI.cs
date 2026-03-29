// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-08-19
// Description: Manages the inventory UI for both player and container inventories, handles slot creation and item transfer logic. 
//              Also shows UI for player Upgrades

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("Data")]
    public Inventory playerInventory;

    [Header("UI Prefabs & Parents")]
    public GameObject rowPrefab;
    public GameObject playerPanel;
    public Transform playerContent;
    public GameObject tradePanel;
    public Transform tradePlayerContent;
    public Transform tradeContainerContent;

    public GameObject itemPanel;

    [Header("Input")]
    public KeyCode toggleInventoryKey = KeyCode.I;
    public KeyCode closeKey = KeyCode.Escape;

    [Header("Item Panel")]
    public Image itemIcon;
    public TMP_Text itemNameText;
    public TMP_Text rarityValueText;
    public Button primaryButton;
    public Button secondaryButton;
    public Button tertiaryButton;

    public Inventory containerInventory = null;
    private Inventory selectedInventory = null;
    private InventorySlot selectedSlot = null;
    private int selectedIndex = -1;

    private Character playerCharacter;

    public static InventoryUI Instance;

    public CanvasGroup inventoryGroup;
    public CanvasGroup tradeGroup;
    public CanvasGroup middleGroup;
    public GameObject questUI;
    public GameObject statsUI;
    public GameObject tabsUI;
    public GameObject playerMenuRoot;
    public GameObject combatUI;

    public void Awake()
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

    void Start()
    {
        primaryButton.onClick.AddListener(HandlePrimaryButton);
        secondaryButton.onClick.AddListener(HandleSecondaryButton);
        tertiaryButton.onClick.AddListener(HandleTertiaryButton);
        playerCharacter = FindPlayerCharacter();
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleInventoryKey) && GameStates.Instance.currentState == GameState.Exploration)
        {
            OpenPlayerInventory();
        }
    }

    public void OpenPlayerInventory()
    {
        SetPanel(inventoryGroup, true);
        BuildPlayerList();
        ClearSelection();
    }

    public void ClosePlayerInventory()
    {
        ClearChildren(playerContent);
        SetPanel(inventoryGroup, false);
        ClearSelection();
    }

    public void OpenTrade(Inventory container)
    {
        containerInventory = container;

        playerMenuRoot.SetActive(true);
        questUI.SetActive(false);
        statsUI.SetActive(false);
        tabsUI.SetActive(false);
        combatUI.SetActive(false);

        SetPanel(tradeGroup, true);
        SetPanel(middleGroup, true);

        BuildTradeLists();
        ClearSelection();
    }

    public void CloseTrade()
    {
        ClearChildren(tradePlayerContent);
        ClearChildren(tradeContainerContent);

        SetPanel(tradeGroup, false);
        SetPanel(middleGroup, false);
        questUI.SetActive(true);
        statsUI.SetActive(true);
        tabsUI.SetActive(true);
        playerMenuRoot.SetActive(false);
        combatUI.SetActive(true);

        containerInventory = null;
        ClearSelection();
    }

    string GetRarityName(int rarity)
    {
        switch (rarity)
        {
            case 1: return "Common";
            case 2: return "Uncommon";
            case 3: return "Rare";
            case 4: return "Epic";
            case 5: return "Legendary";
            default: return "Unknown";
        }
    }

    Color GetRarityColor(int rarity)
    {
        switch (rarity)
        {
            case 1: return Color.white;
            case 2: return Color.green;
            case 3: return Color.blue;
            case 4: return new Color(0.6f, 0f, 1f);
            case 5: return new Color(1f, 0.84f, 0f);
            default: return Color.white;
        }
    }

    public void BuildPlayerList()
    {
        ClearChildren(playerContent);

        for (int i = 0; i < playerInventory.invSlots.Count; i++)
        {
            InventorySlot slot = playerInventory.invSlots[i];
            if (slot == null || slot.item == null) continue;

            GameObject row = Instantiate(rowPrefab, playerContent);
            TMP_Text[] texts = row.GetComponentsInChildren<TMP_Text>();

            if (texts.Length >= 2)
            {
                texts[0].text = $"{GetRarityName(slot.item.rarity)} {slot.item.name}";
                texts[0].color = GetRarityColor(slot.item.rarity);
                texts[1].text = $"x{slot.amount}";
            }

            Image[] images = row.GetComponentsInChildren<Image>();
            foreach (Image image in images)
            {
                if (image.gameObject != row && image.GetComponent<Button>() == null)
                {
                    image.enabled = slot.item.icon != null;
                    image.sprite = slot.item.icon;
                    break;
                }
            }

            int index = i;
            Button btn = row.GetComponent<Button>();
            if (btn)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => HandleSlotClick(playerInventory, index));
            }
        }
    }

    public void BuildTradeLists()
    {
        ClearChildren(tradePlayerContent);

        for (int i = 0; i < playerInventory.invSlots.Count; i++)
        {
            InventorySlot slot = playerInventory.invSlots[i];
            if (slot == null || slot.item == null) continue;

            GameObject row = Instantiate(rowPrefab, tradePlayerContent);
            TMP_Text[] texts = row.GetComponentsInChildren<TMP_Text>();

            if (texts.Length >= 2)
            {
                texts[0].text = $"{slot.item.name}";
                texts[0].color = GetRarityColor(slot.item.rarity);
                texts[1].text = $"x{slot.amount}";
            }

            Image[] images = row.GetComponentsInChildren<Image>();
            foreach (Image image in images)
            {
                if (image.gameObject != row && image.GetComponent<Button>() == null)
                {
                    image.enabled = slot.item.icon != null;
                    image.sprite = slot.item.icon;
                    break;
                }
            }

            int index = i;
            Button btn = row.GetComponent<Button>();
            if (btn)
            {
                btn.onClick.RemoveAllListeners();

                if (!slot.item.isSellable)
                {
                    btn.interactable = false;
                    continue;
                }

                btn.onClick.AddListener(() => HandleSlotClick(playerInventory, index));
            }
        }

        ClearChildren(tradeContainerContent);

        if (containerInventory == null)
            return;

        for (int i = 0; i < containerInventory.invSlots.Count; i++)
        {
            InventorySlot slot = containerInventory.invSlots[i];
            if (slot == null || slot.item == null) continue;

            GameObject row = Instantiate(rowPrefab, tradeContainerContent);
            TMP_Text[] texts = row.GetComponentsInChildren<TMP_Text>();

            if (texts.Length >= 2)
            {
                texts[0].text = $"{GetRarityName(slot.item.rarity)} {slot.item.name}";
                texts[0].color = GetRarityColor(slot.item.rarity);
                texts[1].text = $"x{slot.amount}";
            }

            Image[] images = row.GetComponentsInChildren<Image>();
            foreach (Image image in images)
            {
                if (image.gameObject != row && image.GetComponent<Button>() == null)
                {
                    image.enabled = slot.item.icon != null;
                    image.sprite = slot.item.icon;
                    break;
                }
            }

            int index = i;
            Button btn = row.GetComponent<Button>();
            if (btn)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => HandleSlotClick(containerInventory, index));
            }
        }
    }

    private void HandleSlotClick(Inventory inventory, int index)
    {
        selectedInventory = inventory;
        selectedIndex = index;
        selectedSlot = inventory.invSlots[index];

        if (selectedSlot == null || selectedSlot.item == null)
            return;

        DisplaySelectedItem(selectedSlot.item, inventory == playerInventory);
    }

    private void DisplaySelectedItem(Item item, bool isPlayerItem)
    {
        if (item == null) return;

        if (!item.isSellable && containerInventory != null)
        {
            primaryButton.gameObject.SetActive(false);
            secondaryButton.gameObject.SetActive(false);
            tertiaryButton.gameObject.SetActive(false);
            return;
        }

        itemIcon.enabled = true;
        itemIcon.sprite = item.icon;

        itemNameText.text = $"{GetRarityName(item.rarity)} {item.name}";

        int displayPrice = item.value;
        if (ShopSystem.Instance != null)
        {
            displayPrice = ShopSystem.Instance.GetEffectivePrice(item);
        }

        rarityValueText.text = $"Rarity: {GetRarityName(item.rarity)}   Value: {displayPrice}";

        primaryButton.gameObject.SetActive(true);
        secondaryButton.gameObject.SetActive(true);
        tertiaryButton.gameObject.SetActive(true);

        if (containerInventory == null)
        {
            primaryButton.GetComponentInChildren<TMP_Text>().text = "Use";
            secondaryButton.gameObject.SetActive(false);
            tertiaryButton.gameObject.SetActive(false);
        }
        else if (isPlayerItem)
        {
            primaryButton.GetComponentInChildren<TMP_Text>().text = "Sell";
            secondaryButton.gameObject.SetActive(false);
            tertiaryButton.gameObject.SetActive(false);
        }
        else
        {
            bool isMerchant = containerInventory.isMerchant;

            primaryButton.GetComponentInChildren<TMP_Text>().text = isMerchant ? "Buy" : "Take";
            secondaryButton.GetComponentInChildren<TMP_Text>().text = "Barter";
            tertiaryButton.GetComponentInChildren<TMP_Text>().text = "Steal";

            secondaryButton.gameObject.SetActive(isMerchant);
            tertiaryButton.gameObject.SetActive(isMerchant);
        }
    }

    private void HandlePrimaryButton()
    {
        if (selectedInventory == null || selectedSlot == null)
            return;

        if (containerInventory == null)
        {
            return;
        }

        if (selectedInventory == playerInventory)
        {
            ShopSystem.Instance.SellItem(selectedSlot.item, 1);
        }
        else if (selectedInventory == containerInventory)
        {
            ShopSystem.Instance.BuyItem(selectedSlot.item);
        }
    }

    private void HandleSecondaryButton()
    {
        if (selectedSlot == null || containerInventory == null)
            return;

        if (!containerInventory.isMerchant)
            return;

        if (selectedInventory != containerInventory)
            return;

        ShopSystem.Instance.AttemptBarter(selectedSlot.item, playerCharacter);
    }

    private void HandleTertiaryButton()
    {
        if (selectedSlot == null || containerInventory == null)
            return;

        if (!containerInventory.isMerchant)
            return;

        if (selectedInventory != containerInventory)
            return;

        ShopSystem.Instance.AttemptSteal(selectedSlot.item, playerCharacter);
    }

    private void ClearChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }

    private void ClearSelection()
    {
        selectedInventory = null;
        selectedSlot = null;
        selectedIndex = -1;

        itemIcon.enabled = false;
        itemNameText.text = "";
        rarityValueText.text = "";
    }

    public void SetPanel(CanvasGroup group, bool show)
    {
        group.alpha = show ? 1f : 0f;
        group.interactable = show;
        group.blocksRaycasts = show;
    }

    public void OnTradeChanged(Item reselectItem = null)
    {
        BuildTradeLists();
        ClearSelection();

        if (reselectItem != null)
            ReSelectItem(reselectItem);
    }

    private Character FindPlayerCharacter()
    {
        GameObject taggedPlayer = GameObject.FindWithTag("Player");
        if (taggedPlayer == null) return null;

        return taggedPlayer.GetComponentInParent<Character>();
    }

    public void ReSelectItem(Item item)
    {
        if (item == null) return;
        if (containerInventory == null) return;

        for (int i = 0; i < containerInventory.invSlots.Count; i++)
        {
            InventorySlot slot = containerInventory.invSlots[i];
            if (slot == null || slot.item == null)
                continue;

            if (slot.item == item)
            {
                HandleSlotClick(containerInventory, i);
                return;
            }
        }
    }
}