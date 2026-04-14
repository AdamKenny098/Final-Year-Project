using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DecorCluster
{
    public string id;
    public Vector2Int centerGridPos;

    public bool isValid;
    public bool isSelected;

    public List<Vector2Int> footprintCells = new List<Vector2Int>();
    public List<DecorSlot> primarySlots = new List<DecorSlot>();
    public List<DecorSlot> secondarySlots = new List<DecorSlot>();
    public List<DecorSlot> tertiarySlots = new List<DecorSlot>();

    public DecorCluster(string id, Vector2Int centerGridPos)
    {
        this.id = id;
        this.centerGridPos = centerGridPos;
        isValid = true;
        isSelected = false;
    }

    public IEnumerable<DecorSlot> GetAllSlots()
    {
        for (int i = 0; i < primarySlots.Count; i++)
            yield return primarySlots[i];

        for (int i = 0; i < secondarySlots.Count; i++)
            yield return secondarySlots[i];

        for (int i = 0; i < tertiarySlots.Count; i++)
            yield return tertiarySlots[i];
    }

    public void BuildStandard5x5Layout()
    {
        footprintCells.Clear();
        primarySlots.Clear();
        secondarySlots.Clear();
        tertiarySlots.Clear();

        for (int x = -2; x <= 2; x++)
        {
            for (int z = -2; z <= 2; z++)
            {
                Vector2Int pos = centerGridPos + new Vector2Int(x, z);
                footprintCells.Add(pos);

                bool isCenter = x == 0 && z == 0;
                bool inThreeByThree = Mathf.Abs(x) <= 1 && Mathf.Abs(z) <= 1;

                if (isCenter)
                {
                    primarySlots.Add(new DecorSlot(pos, DecorSlotTier.Primary, id));
                }
                else if (inThreeByThree)
                {
                    secondarySlots.Add(new DecorSlot(pos, DecorSlotTier.Secondary, id));
                }
                else
                {
                    tertiarySlots.Add(new DecorSlot(pos, DecorSlotTier.Tertiary, id));
                }
            }
        }

        RandomizeSlotAvailability();
    }

    void RandomizeSlotAvailability()
    {
        RandomizeTier(primarySlots, .5f, 1f);
        RandomizeTier(secondarySlots, 0.15f, 0.3f);
        RandomizeTier(tertiarySlots, 0.05f, 0.20f);
    }

    void RandomizeTier(List<DecorSlot> slots, float minKeepPercent, float maxKeepPercent)
    {
        if (slots == null || slots.Count == 0)
            return;

        float keepPercent = Random.Range(minKeepPercent, maxKeepPercent);
        int keepCount = Mathf.Clamp(
            Mathf.RoundToInt(slots.Count * keepPercent), 0, slots.Count);

        List<int> indices = new List<int>();
        for (int i = 0; i < slots.Count; i++)
            indices.Add(i);

        for (int i = 0; i < indices.Count; i++)
        {
            int randomIndex = Random.Range(i, indices.Count);
            int temp = indices[i];
            indices[i] = indices[randomIndex];
            indices[randomIndex] = temp;
        }

        for (int i = 0; i < slots.Count; i++)
            slots[i].enabled = false;

        for (int i = 0; i < keepCount; i++)
            slots[indices[i]].enabled = true;
    }
}