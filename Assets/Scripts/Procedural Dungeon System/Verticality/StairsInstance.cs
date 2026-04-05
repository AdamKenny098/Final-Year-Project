using UnityEngine;

public enum StairDirection
{
    Up,
    Down
}

public class StairsInstance : MonoBehaviour
{
    public StairDirection direction;

    static float blockUntilTime;

    public static void BlockTriggers(float duration)
    {
        blockUntilTime = Time.time + duration;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other == null)
            return;

        if (!other.CompareTag("Player"))
            return;

        if (Time.time < blockUntilTime)
            return;

        if (LabyrinthManager.Instance == null || LabyrinthManager.Instance.isLoadingFloor)
            return;

        if (direction == StairDirection.Up)
            LabyrinthManager.Instance.TravelUp();
        else
            LabyrinthManager.Instance.TravelDown();
    }
}