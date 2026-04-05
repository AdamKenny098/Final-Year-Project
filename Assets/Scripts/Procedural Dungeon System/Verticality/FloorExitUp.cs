using UnityEngine;

public class FloorExitUp : MonoBehaviour
{
    static float blockUntilTime;

    public static void BlockTriggers(float duration)
    {
        blockUntilTime = Time.time + duration;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (Time.time < blockUntilTime)
            return;

        if (LabyrinthManager.Instance == null || LabyrinthManager.Instance.isLoadingFloor)
            return;

        LabyrinthManager.Instance.GoToLastFloor();
    }
}