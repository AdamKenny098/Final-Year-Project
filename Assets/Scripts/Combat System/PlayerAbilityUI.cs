using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerAbilityUI : MonoBehaviour
{
    [Header("Refs")]
    public Character player;
    public AbilityManager abilities;

    [Header("Ability Slots (dynamic)")]
    public AbilitySlotUI slotPrefab;
    public Transform slotsRoot;
    public bool autoBuildSlots = true;
    public AbilitySlotUI[] slots = new AbilitySlotUI[5];
    public Image[] cooldownFills;
    public string[] Keys = { "LMB", "RMB", "1", "2", "3" };

    AbilityLoadout lastLoadout;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player")?.GetComponentInParent<Character>();
        }

        if (player != null && abilities == null)
        {
            abilities = player.GetComponentInParent<AbilityManager>();
        }

        if (autoBuildSlots)
        {
            BuildSlotsIfNeeded();
        }

        RefreshAbilityUI(true);
    }

    void Update()
    {
        if (player == null || player.stats == null) return;

        if (abilities != null && abilities.loadout != lastLoadout)
        {
            RefreshAbilityUI(false);
        }

        UpdateCooldowns();
    }

    void BuildSlotsIfNeeded()
    {
        if (slotsRoot == null || slotPrefab == null) return;

        
        bool anyAssigned = false;
        for (int i = 0; i < slots.Length; i++)
            if (slots[i] != null) 
            { 
                anyAssigned = true; 
                break; 
            }
        if (anyAssigned) return;

        for (int i = 0; i < 5; i++)
        {
            AbilitySlotUI slotUI = Instantiate(slotPrefab, slotsRoot);
            slotUI.SetKey(i < Keys.Length ? Keys[i] : "");
            slots[i] = slotUI;
        }
    }

    void RefreshAbilityUI(bool force)
    {
        if (abilities == null) return;
        if (!force && abilities.loadout == lastLoadout) return;

        lastLoadout = abilities.loadout;

        for (int i = 0; i < 5; i++)
        {
            AbilityData abilityData = abilities.GetAbility(i);
            if (slots != null && i < slots.Length && slots[i] != null)
            {
                slots[i].BindAbility(abilityData);
            }
        }
    }

    void UpdateCooldowns()
    {
        if (abilities == null || slots == null) return;

        for (int i = 0; i < 5; i++)
        {
            if (i >= slots.Length) break;
            if (slots[i] == null) continue;

            slots[i].UpdateCooldown(abilities, i);
        }
    }
}
