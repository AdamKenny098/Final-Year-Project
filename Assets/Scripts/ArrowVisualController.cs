using System;
using System.Collections.Generic;
using UnityEngine;

public enum ArrowVisualType
{
    Normal,
    PiercingShot,
    PowerShot,
    QuickShot,
    SnareShot,
    Volley
}

[Serializable]
public class ArrowVisualEntry
{
    public ArrowVisualType type;
    public Material arrowMaterial;
    public Vector3 visualScale = Vector3.one;

    public float trailTime = 0.15f;
    public float trailStartWidth = 0.08f;
    public float trailEndWidth = 0f;
    public Color trailColor = Color.white;

    public GameObject flightVfxPrefab;
    public GameObject impactVfxPrefab;
}

public class ArrowVisualController : MonoBehaviour
{
    public MeshRenderer[] arrowRenderers;
    public TrailRenderer trailRenderer;
    public List<ArrowVisualEntry> visuals = new();

    ArrowVisualEntry currentVisual;
    GameObject activeFlightVfx;

    public void ApplyVisual(ArrowVisualType type)
    {
        currentVisual = GetEntry(type);
        if (currentVisual == null)
            return;

        transform.localScale = currentVisual.visualScale;

        if (currentVisual.arrowMaterial != null)
        {
            for (int i = 0; i < arrowRenderers.Length; i++)
            {
                if (arrowRenderers[i] != null)
                    arrowRenderers[i].material = currentVisual.arrowMaterial;
            }
        }

        if (trailRenderer != null)
        {
            trailRenderer.Clear();
            trailRenderer.time = currentVisual.trailTime;
            trailRenderer.startWidth = currentVisual.trailStartWidth;
            trailRenderer.endWidth = currentVisual.trailEndWidth;
            trailRenderer.colorGradient = BuildGradient(currentVisual.trailColor);
        }

        if (activeFlightVfx != null)
            Destroy(activeFlightVfx);

        if (currentVisual.flightVfxPrefab != null)
            activeFlightVfx = Instantiate(currentVisual.flightVfxPrefab, transform.position, transform.rotation, transform);
    }

    public void SpawnImpact(Vector3 point, Vector3 normal)
    {
        if (currentVisual == null || currentVisual.impactVfxPrefab == null)
            return;

        Quaternion rotation = normal.sqrMagnitude > 0.001f? Quaternion.LookRotation(normal): Quaternion.identity;

        Instantiate(currentVisual.impactVfxPrefab, point, rotation);
    }

    public ArrowVisualEntry GetEntry(ArrowVisualType type)
    {
        for (int i = 0; i < visuals.Count; i++)
        {
            if (visuals[i] != null && visuals[i].type == type)
                return visuals[i];
        }

        return null;
    }

    public Gradient BuildGradient(Color color)
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(color, 0f),
                new GradientColorKey(color, 1f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0.2f, 0.7f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        return gradient;
    }
}