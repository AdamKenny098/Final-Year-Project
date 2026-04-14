using UnityEngine;

[System.Serializable]
public class DecorReservation
{
    public Vector2Int gridPos;
    public DecorReservationPriority priority;
    public DecorReservationType type;
    public string source;

    public bool IsReserved => priority != DecorReservationPriority.None;

    public DecorReservation()
    {
        gridPos = Vector2Int.zero;
        priority = DecorReservationPriority.None;
        type = DecorReservationType.None;
        source = string.Empty;
    }

    public DecorReservation(Vector2Int gridPos, DecorReservationPriority priority, DecorReservationType type, string source)
    {
        this.gridPos = gridPos;
        this.priority = priority;
        this.type = type;
        this.source = source;
    }

    public bool CanBeOverwrittenBy(DecorReservationPriority incomingPriority)
    {
        return incomingPriority >= priority;
    }

    public void Clear()
    {
        priority = DecorReservationPriority.None;
        type = DecorReservationType.None;
        source = string.Empty;
    }

    public void Set(DecorReservationPriority newPriority, DecorReservationType newType, string newSource)
    {
        priority = newPriority;
        type = newType;
        source = newSource;
    }
}