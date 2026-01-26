using UnityEngine;

public class FloorExitUp : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        LabyrinthManager.Instance.GoToLastFloor();
    }
}
