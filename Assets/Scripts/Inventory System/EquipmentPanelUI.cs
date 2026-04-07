using UnityEngine;
using UnityEngine.UI;

public class EquipmentPanelUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] EquipmentManager equipmentManager;

    [Header("Icon Images")]
    [SerializeField] Image helmetIcon;
    [SerializeField] Image chestplateIcon;
    [SerializeField] Image leggingsIcon;
    [SerializeField] Image bootsIcon;
    [SerializeField] Image weaponIcon;

    void Start()
    {
        if (equipmentManager == null)
            equipmentManager = FindFirstObjectByType<EquipmentManager>();

        Refresh();
    }

    public void Refresh()
    {
        if (equipmentManager == null)
            return;

        SetIcon(helmetIcon, equipmentManager.helmet);
        SetIcon(chestplateIcon, equipmentManager.chestplate);
        SetIcon(leggingsIcon, equipmentManager.leggings);
        SetIcon(bootsIcon, equipmentManager.boots);
        SetIcon(weaponIcon, equipmentManager.equippedWeapon);
    }

    void SetIcon(Image iconImage, Item item)
    {
        if (iconImage == null)
            return;

        if (item != null && item.icon != null)
        {
            iconImage.enabled = true;
            iconImage.sprite = item.icon;
        }
        else
        {
            iconImage.enabled = false;
            iconImage.sprite = null;
        }
    }
}