using UnityEngine;

public class FloorExitDown : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        LabyrinthManager.Instance.GoToNextFloor();
    }
}
