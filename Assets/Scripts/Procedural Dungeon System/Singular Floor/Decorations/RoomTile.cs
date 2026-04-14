using UnityEngine;

[System.Serializable]
public class RoomTile
{
    public Vector2Int gridPos;
    public Vector3 worldPos;

    public bool blocked;
    public bool isDoorway;
    public bool isDoorBuffer;

    public float wallScore;
    public float centerScore;
    public float doorDistance;

    public DecorReservation reservation = new DecorReservation();

    public bool IsFree =>
        !blocked &&
        reservation.priority == DecorReservationPriority.None;

    public bool IsReserved =>
        reservation.priority != DecorReservationPriority.None;

    public void MarkBlocked(string source = "Blocked")
    {
        blocked = true;
        reservation.gridPos = gridPos;
        reservation.Set(DecorReservationPriority.Protected, DecorReservationType.Blocked, source);
    }

    public bool CanReserve(DecorReservationPriority incomingPriority)
    {
        if (blocked)
            return false;

        return reservation.CanBeOverwrittenBy(incomingPriority);
    }

    public bool TryReserve(DecorReservationPriority priority, DecorReservationType type, string source)
    {
        if (!CanReserve(priority))
            return false;

        reservation.gridPos = gridPos;
        reservation.Set(priority, type, source);
        return true;
    }

    public void ClearReservation()
    {
        if (blocked)
            return;

        reservation.Clear();
    }
}