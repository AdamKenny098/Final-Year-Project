using UnityEngine;
using System.Collections.Generic;

public class LootDropper : MonoBehaviour
{
    public List<LootTableItem> lootTable = new List<LootTableItem>();
    public Transform dropPoint;
    public GameObject lootBagPrefab;

    public void DropLoot()
    {
        Debug.Log($"DropLoot CALLED on {gameObject.name}");

        int roll = Random.Range(1, 101);

        foreach (LootTableItem entry in lootTable)
        {
            if (roll >= entry.minRoll && roll <= entry.maxRoll)
            {
                int amount = Random.Range(entry.minAmount, entry.maxAmount + 1);

                for (int i = 0; i < amount; i++)
                {
                    Vector2 offset2D = Random.insideUnitCircle * 0.5f;
                    Vector3 offset3D = new Vector3(offset2D.x, 0f, offset2D.y);

                    GameObject bagObj = Instantiate(
                        lootBagPrefab,
                        dropPoint.position + offset3D,
                        Quaternion.identity
                    );

                    LootBag bag = bagObj.GetComponent<LootBag>();
                    bag.SetItem(entry.item);

                }

                return;
            }
        }
    }
}
