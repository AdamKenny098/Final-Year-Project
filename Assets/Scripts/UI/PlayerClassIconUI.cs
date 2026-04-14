using UnityEngine;
using UnityEngine.UI;

public class PlayerClassIconUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Character player;
    [SerializeField] Image classIconImage;

    [Header("Class Icons")]
    [SerializeField] Sprite warriorIcon;
    [SerializeField] Sprite mageIcon;
    [SerializeField] Sprite thiefIcon;
    [SerializeField] Sprite archerIcon;

    ClassSystem.Classes lastClass;
    bool iconSet;

    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player")?.GetComponentInParent<Character>();

        RefreshIcon(true);
    }

    void Update()
    {
        if (player == null)
            return;

        if (!iconSet || player.characterClass != lastClass)
            RefreshIcon(false);
    }

    void RefreshIcon(bool force)
    {
        if (player == null || classIconImage == null)
            return;

        if (!force && iconSet && player.characterClass == lastClass)
            return;

        lastClass = player.characterClass;
        iconSet = true;

        Sprite chosenIcon = null;

        switch (player.characterClass)
        {
            case ClassSystem.Classes.Warrior:
                chosenIcon = warriorIcon;
                break;

            case ClassSystem.Classes.Mage:
                chosenIcon = mageIcon;
                break;

            case ClassSystem.Classes.Thief:
                chosenIcon = thiefIcon;
                break;

            case ClassSystem.Classes.Archer:
                chosenIcon = archerIcon;
                break;
        }

        classIconImage.sprite = chosenIcon;
        classIconImage.enabled = chosenIcon != null;
    }
}