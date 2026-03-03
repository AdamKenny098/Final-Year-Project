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
        public GameObject pageRoot;
    }

    public List<Tab> tabs = new();
    public string defaultTabId = "Stats";
    public string currentTabId;

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

        for (int i = 0; i < tabs.Count; i++)
        {
            bool active = string.Equals(tabs[i].id, id);

            if (tabs[i].pageRoot != null)
                tabs[i].pageRoot.SetActive(active);

            if (tabs[i].button != null)
                tabs[i].button.interactable = !active;
        }
    }
}
