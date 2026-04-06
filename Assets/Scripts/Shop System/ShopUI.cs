using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [Header("Shop Panels")]
    [SerializeField] private CanvasGroup merchantInventory;
    [SerializeField] private CanvasGroup middlePanel;

    void Awake()
    {
        HideShop();
    }

    public void ShowShop()
    {
        SetGroup(merchantInventory, true);
        SetGroup(middlePanel, true);
    }

    public void HideShop()
    {
        SetGroup(merchantInventory, false);
        SetGroup(middlePanel, false);
    }

    void SetGroup(CanvasGroup group, bool show)
    {
        if (group == null)
            return;

        group.gameObject.SetActive(show);
        group.alpha = show ? 1f : 0f;
        group.interactable = show;
        group.blocksRaycasts = show;
    }
}