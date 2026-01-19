using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [Header("Shop Panels")]
    [SerializeField] private CanvasGroup merchantInventory;
    [SerializeField] private CanvasGroup middlePanel;

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
        group.alpha = show ? 1 : 0;
        group.interactable = show;
        group.blocksRaycasts = show;
    }
}
