using UnityEngine;

public class LootBag : MonoBehaviour, IInteractable
{
    public Item item;
    public Inventory inventory;
    public ItemPickupPrompt pickupPrompt;

    [Header("Audio")]
    [SerializeField] private AudioClip pickupSfx;
    [Range(0f, 1f)] [SerializeField] private float pickupVolume = 1f;

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
        if (item == null)
            return;

        if (QuestSystem.Instance != null)
            QuestSystem.Instance.NotifyItemCollected(item.itemId, 1);

        bool added = inventory.AddItem(item, 1);
        if (!added)
            return;

        if (pickupSfx != null)
        {
            AudioSource.PlayClipAtPoint(pickupSfx, transform.position, pickupVolume);
        }

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