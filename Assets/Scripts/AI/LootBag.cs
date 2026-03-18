using UnityEngine;

public class LootBag : MonoBehaviour, IInteractable
{
    public Item item;   // ScriptableObject reference
    public Inventory inventory;
    public ItemPickupPrompt pickupPrompt;

    void Awake()
    {
        inventory = GameObject.Find("Heads Up Display").GetComponent<Inventory>();

        if (pickupPrompt == null)
        {
            pickupPrompt = GetComponentInChildren<ItemPickupPrompt>(true);
        }
    }

    public void SetItem(Item newItem)
    {
        item = newItem;

        if (pickupPrompt != null && item != null)
        {
            pickupPrompt.Show(item.name, item.icon);
        }
    }

    public void Interact()
    {
        if (QuestSystem.Instance != null)
            QuestSystem.Instance.NotifyItemCollected(item.itemId, 1);

        if (item == null) return;

        inventory.AddItem(item, 1);
        Destroy(gameObject);
    }

    public void ShowPrompt()
    {
        if (pickupPrompt != null && item != null)
        {
            pickupPrompt.Show(item.name, item.icon);
        }
    }

    public void HidePrompt()
    {
        if (pickupPrompt != null)
        {
            pickupPrompt.Hide();
        }
    }
}