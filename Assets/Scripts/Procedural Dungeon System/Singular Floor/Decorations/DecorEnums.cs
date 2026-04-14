using UnityEngine;

public class DecorEnums : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


    public enum DecorReservationPriority
    {
        None = 0,
        Generic = 1,
        Cluster = 2,
        Protected = 3
    }

    public enum DecorReservationType
    {
        None = 0,
        Generic,
        PrimarySlot,
        SecondarySlot,
        TertiarySlot,
        ClusterFootprint,
        Doorway,
        DoorBuffer,
        Blocked
    }

    public enum DecorSlotTier
    {
        Primary = 0,
        Secondary = 1,
        Tertiary = 2
    }