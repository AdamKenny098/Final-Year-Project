using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemPickupPrompt : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemNameText;

    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
        Hide();
    }

    private void Update()
    {
        if (root.activeSelf && cam != null)
        {
            Vector3 dir = transform.position - cam.transform.position;
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    public void Show(string displayName, Sprite icon)
    {
        root.SetActive(true);

        if (itemNameText != null)
            itemNameText.text = displayName;

        if (itemIcon != null)
        {
            itemIcon.sprite = icon;
            itemIcon.enabled = icon != null;
        }
    }

    public void Hide()
    {
        if (root != null)
            root.SetActive(false);
    }
}