using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchFlicker : MonoBehaviour
{
    public float minIntensity = 1.0f;
    public float maxIntensity = 3.0f;
    public float flickerSpeed = 8f;

    public Light fireLight;

    void Awake()
    {
        fireLight = GetComponent<Light>();
    }

    void Update()
    {
        // Adds a flickering effect to the torch light using Perlin noise (Yes this is mainly copilot)
        // Intenstiy is calculated between min and max intensity values by sampling Perlin noise over time
        fireLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, Mathf.PerlinNoise(Time.time * flickerSpeed, 0f));
    }
}