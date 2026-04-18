using UnityEngine;

public class ArrowVisualProjectile : MonoBehaviour
{
    [SerializeField] ArrowVisualController visualController;
    [SerializeField] float lifeTime = 3f;
    [SerializeField] float hitDistance = 0.15f;

    Vector3 targetPoint;
    float moveSpeed;
    bool launched;

    public void Launch(Vector3 point, ArrowVisualType visualType, float speed)
    {
        targetPoint = point;
        moveSpeed = Mathf.Max(1f, speed);
        launched = true;

        if (visualController != null)
            visualController.ApplyVisual(visualType);

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (!launched)
            return;

        Vector3 toTarget = targetPoint - transform.position;
        float distance = toTarget.magnitude;

        if (distance <= hitDistance)
        {
            ImpactAndDestroy();
            return;
        }

        Vector3 dir = toTarget / distance;
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);

        float step = moveSpeed * Time.deltaTime;
        if (step >= distance)
        {
            transform.position = targetPoint;
            ImpactAndDestroy();
            return;
        }

        transform.position += dir * step;
    }

    void ImpactAndDestroy()
    {
        if (visualController != null)
        {
            Vector3 normal = -transform.forward;
            visualController.SpawnImpact(transform.position, normal);
        }

        Destroy(gameObject);
    }
}