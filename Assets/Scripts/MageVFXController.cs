using UnityEngine;

public class MageVFXController : MonoBehaviour
{
    [Header("Points")]
    [SerializeField] private Transform castPoint;

    [Header("Firebolt")]
    [SerializeField] private GameObject fireboltCastVFX;
    [SerializeField] private GameObject fireboltImpactVFX;

    [Header("Fireball")]
    [SerializeField] private GameObject fireballCastVFX;
    [SerializeField] private GameObject fireballImpactVFX;

    [Header("Frost Nova")]
    [SerializeField] private GameObject frostNovaVFX;
    [SerializeField] private Vector3 frostNovaOffset;

    [Header("Meteor")]
    [SerializeField] private GameObject meteorSummonVFX;
    [SerializeField] private GameObject meteorImpactVFX;
    [SerializeField] private float meteorSpawnHeight = 12f;

    [Header("Chain Lightning")]
    [SerializeField] private GameObject chainLightningCastVFX;
    [SerializeField] private GameObject chainLightningHitVFX;
    [SerializeField] private Vector3 chainHitOffset = new Vector3(0f, 1f, 0f);

    public void PlayFireboltCast()
    {
        SpawnAtCastPoint(fireboltCastVFX);
    }

    public void PlayFireboltImpact(Vector3 position)
    {
        SpawnAtPosition(fireboltImpactVFX, position);
    }

    public void PlayFireballCast()
    {
        SpawnAtCastPoint(fireballCastVFX);
    }

    public void PlayFireballImpact(Vector3 position)
    {
        SpawnAtPosition(fireballImpactVFX, position);
    }

    public void PlayFrostNova()
    {
        if (frostNovaVFX == null)
            return;

        Instantiate(frostNovaVFX, transform.position + frostNovaOffset, Quaternion.identity);
    }

    public void PlayMeteorSummon(Vector3 targetPosition)
    {
        SpawnAtPosition(meteorSummonVFX, targetPosition);
    }

    public void PlayMeteorImpact(Vector3 targetPosition, Vector3 normal)
    {
        if (meteorImpactVFX == null)
            return;

        Quaternion rotation = normal.sqrMagnitude > 0.001f
            ? Quaternion.LookRotation(normal)
            : Quaternion.identity;

        Instantiate(meteorImpactVFX, targetPosition, rotation);
    }

    public void PlayChainLightningCast()
    {
        SpawnAtCastPoint(chainLightningCastVFX);
    }

    public void PlayChainLightningHit(Transform target)
    {
        if (target == null || chainLightningHitVFX == null)
            return;

        Instantiate(chainLightningHitVFX, target.position + chainHitOffset, Quaternion.identity);
    }

    private void SpawnAtCastPoint(GameObject prefab)
    {
        if (prefab == null || castPoint == null)
            return;

        Instantiate(prefab, castPoint.position, castPoint.rotation);
    }

    private void SpawnAtPosition(GameObject prefab, Vector3 position)
    {
        if (prefab == null)
            return;

        Instantiate(prefab, position, Quaternion.identity);
    }
}