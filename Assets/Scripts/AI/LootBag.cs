using UnityEngine;

public class LootBag : MonoBehaviour, IInteractable
{
    public Item item;   // ScriptableObject reference
    public Inventory inventory;

    void Awake()
    {
        inventory = GameObject.Find("Heads Up Display").GetComponent<Inventory>();
    }
    public void SetItem(Item newItem)
    {
        item = newItem;
    }

    public void Interact()
    {
        if (item == null) return;

        inventory.AddItem(item, 1);
        Destroy(gameObject);
    }
}
