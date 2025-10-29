using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FontChanger : MonoBehaviour
{
    public TMP_FontAsset defaultFont; 
    public TMP_FontAsset gameFont; 
    public Toggle fontToggle; 
    public TextMeshProUGUI[] textElements; 

    void Start()
    {
        
        fontToggle.onValueChanged.AddListener(delegate { ToggleFont(fontToggle.isOn); });

        ToggleFont(fontToggle.isOn);
    }

    void ToggleFont(bool isGame)
    {
        foreach (TextMeshProUGUI textElement in textElements)
        {
            textElement.font = isGame ? gameFont : defaultFont;
        }
    }
}
