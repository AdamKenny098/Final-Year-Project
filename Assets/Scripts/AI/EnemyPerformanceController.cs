using UnityEngine;

public class EnemyPerformanceController : MonoBehaviour
{
    public bool isActive = true;
    public float dormantSenseInterval = 0.75f;

    private float nextSenseTime;

    public bool CanRunSense()
    {
        if (isActive)
            return true;

        if (Time.time < nextSenseTime)
            return false;

        nextSenseTime = Time.time + dormantSenseInterval;
        return true;
    }

    public void SetActiveState(bool activeState)
    {
        isActive = activeState;
    }
}