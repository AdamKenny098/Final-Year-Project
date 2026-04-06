 using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITabBar : MonoBehaviour
{
    [Serializable]
    public class Tab
    {
        public string id;
        public Button button;
        public List<GameObject> pageRoot;
    }

    public List<Tab> tabs = new();
    public string defaultTabId = "Stats";
    public string currentTabId;

    [Header("Inventory Specific")]
    public CanvasGroup inventory;
    public CanvasGroup middle;

    void Awake()
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            int index = i;
            if (tabs[index].button == null) continue;

            tabs[index].button.onClick.AddListener(() => OpenTab(tabs[index].id));
        }
    }

    public void OpenDefault()
    {
        OpenTab(string.IsNullOrEmpty(currentTabId) ? defaultTabId : currentTabId);
    }

    public void OpenTab(string id)
    {
        currentTabId = id;

        bool isInventoryTab = string.Equals(id, "Inventory", StringComparison.Ordinal);

        for (int i = 0; i < tabs.Count; i++)
        {
            bool active = string.Equals(tabs[i].id, id, StringComparison.Ordinal);

            if (tabs[i].pageRoot != null)
            {
                foreach (var page in tabs[i].pageRoot)
                {
                    if (page != null)
                        page.SetActive(active);
                }
            }

            if (tabs[i].button != null)
                tabs[i].button.interactable = !active;
        }

        if (isInventoryTab)
            OpenInventory();
        else
            CloseInventory();
    }

    public void OpenInventory()
    {
        inventory.alpha = 1f;
        inventory.interactable = true;
        inventory.blocksRaycasts = true;

        middle.alpha = 1f;
        middle.interactable = true;
        middle.blocksRaycasts = true;
        InventoryUI.Instance.BuildPlayerList();
    }

    public void CloseInventory()
    {
        inventory.alpha = 0f;
        inventory.interactable = false;
        inventory.blocksRaycasts = false;

        middle.alpha = 0f;
        middle.interactable = false;
        middle.blocksRaycasts = false;
    }

    public void CloseAll()
    {
        currentTabId = null;

        for (int i = 0; i < tabs.Count; i++)
        {
            if (tabs[i].pageRoot != null)
            {
                foreach (var page in tabs[i].pageRoot)
                {
                    if (page != null)
                        page.SetActive(false);
                }
            }

            if (tabs[i].button != null)
                tabs[i].button.interactable = true;
        }

        CloseInventory();
    }
}
