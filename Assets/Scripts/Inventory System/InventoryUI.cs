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
        // Toggle only if not trading
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


    private void BuildPlayerList()
    {
        ClearChildren(playerContent);

        for (int i = 0; i < playerInventory.invSlots.Count; i++)
        {
            InventorySlot slot = playerInventory.invSlots[i];
            if (slot == null) continue;

            GameObject row = Instantiate(rowPrefab, playerContent);
            TMP_Text[] texts = row.GetComponentsInChildren<TMP_Text>();

            texts[0].text = slot.item.name;
            texts[1].text = $"x{slot.amount}";

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
        // Player
        ClearChildren(tradePlayerContent);
        for (int i = 0; i < playerInventory.invSlots.Count; i++)
        {
            InventorySlot slot = playerInventory.invSlots[i];
            if (slot == null) continue;

            GameObject row = Instantiate(rowPrefab, tradePlayerContent);
            TMP_Text[] texts = row.GetComponentsInChildren<TMP_Text>();
            
            texts[0].text = slot.item.name;
            texts[1].text = $"x{slot.amount}";

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

        // Other
        ClearChildren(tradeContainerContent);
        for (int i = 0; i < containerInventory.invSlots.Count; i++)
        {
            InventorySlot slot = containerInventory.invSlots[i];
            if (slot == null) continue;

            GameObject row = Instantiate(rowPrefab, tradeContainerContent);
            TMP_Text[] texts = row.GetComponentsInChildren<TMP_Text>();
            if (texts.Length >= 2)
            {
                texts[0].text = slot.item.name;
                texts[1].text = $"x{slot.amount}";
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
        itemNameText.text = item.name;
        int displayPrice = item.value;

        if (containerInventory != null && containerInventory.isMerchant)
        {
            displayPrice = ShopSystem.Instance.GetEffectivePrice(item);
        }

        rarityValueText.text = $"Rarity: {item.rarity}   Value: {displayPrice}";


        primaryButton.gameObject.SetActive(true);
        secondaryButton.gameObject.SetActive(true);
        tertiaryButton.gameObject.SetActive(true);

        if (containerInventory == null)
        {
            primaryButton.GetComponentInChildren<TMP_Text>().text = "Use";
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

            primaryButton.GetComponentInChildren<TMP_Text>().text =
                isMerchant ? "Buy" : "Take";

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
        {
            return;
        }

        if (!containerInventory.isMerchant)
        {
            return;
        }

        if (selectedInventory != containerInventory)
        {
            return;
        }

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