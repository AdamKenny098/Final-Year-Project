using UnityEngine;

public class ArcherVFXController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Transform firePoint;
    [SerializeField] ArrowVisualProjectile arrowProjectilePrefab;

    [Header("Tuning")]
    [SerializeField] float arrowSpeed = 45f;
    [SerializeField] Vector3 targetOffset = new Vector3(0f, 1.1f, 0f);

    public void FireArrow(Vector3 hitPoint, ArrowVisualType visualType, Transform target = null)
    {
        if (arrowProjectilePrefab == null || firePoint == null)
            return;

        Vector3 spawnPos = firePoint.position;

        Vector3 aimPoint = hitPoint;
        if (target != null)
            aimPoint = target.position + targetOffset;

        Vector3 dir = aimPoint - spawnPos;
        if (dir.sqrMagnitude < 0.0001f)
            dir = firePoint.forward;

        Quaternion rot = Quaternion.LookRotation(dir.normalized, Vector3.up);

        ArrowVisualProjectile arrow = Instantiate(arrowProjectilePrefab, spawnPos, rot);
        arrow.Launch(aimPoint, visualType, arrowSpeed);
    }
}