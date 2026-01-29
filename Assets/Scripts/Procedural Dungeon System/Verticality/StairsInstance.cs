using UnityEngine;
public enum StairDirection
{
    Up,
    Down
}

public class StairsInstance : MonoBehaviour
{
    public StairDirection direction;

    private void OnTriggerEnter(Collider other)
    {   
        if (other == null) return;
        if (!other.CompareTag("Player")) return;
        if (direction == StairDirection.Up)
        LabyrinthManager.Instance.TravelUp();
        else
        LabyrinthManager.Instance.TravelDown();
        
        
    }
}
