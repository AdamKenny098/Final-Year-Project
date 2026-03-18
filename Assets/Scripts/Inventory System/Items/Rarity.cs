using UnityEngine;

public class Rarity : MonoBehaviour
{

    public static Rarity Instance { get; private set; }
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    public float GetMultiplier(int rarity)
    {
        switch (rarity)
        {
            case 1: return 1f;      // Common
            case 2: return 1.2f;    // Uncommon
            case 3: return 1.5f;    // Rare
            case 4: return 2f;      // Epic
            case 5: return 3f;      // Legendary
        }

        return 1f;
    }

    public Color GetColor(int rarity)
    {
        switch (rarity)
        {
            case 1: return Color.white;
            case 2: return Color.green;
            case 3: return Color.blue;
            case 4: return new Color(0.6f,0f,1f);
            case 5: return new Color(1f,0.8f,0f);
        }

        return Color.white;
    }

    public string GetRarityName(int rarity)
    {
        switch (rarity)
        {
            case 1: return "Common";
            case 2: return "Uncommon";
            case 3: return "Rare";
            case 4: return "Epic";
            case 5: return "Legendary";
            default: return "Unknown";
        }
    }
}