using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    [SerializeField] ArrowVisualController visualController;

    public void Initialize(ArrowVisualType visualType)
    {
        if (visualController != null)
            visualController.ApplyVisual(visualType);
    }

    public void OnArrowHit(RaycastHit hit)
    {
        if (visualController != null)
            visualController.SpawnImpact(hit.point, hit.normal);
    }
}