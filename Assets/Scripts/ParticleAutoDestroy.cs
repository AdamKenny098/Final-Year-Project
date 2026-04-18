using UnityEngine;

public class ParticleAutoDestroy : MonoBehaviour
{
    [SerializeField] private float extraLifetime = 0.25f;

    private void OnEnable()
    {
        ParticleSystem[] systems = GetComponentsInChildren<ParticleSystem>(true);

        float longest = 0f;

        for (int i = 0; i < systems.Length; i++)
        {
            ParticleSystem ps = systems[i];
            ps.Play(true);

            var main = ps.main;
            float duration = main.duration;

            if (!main.loop)
                duration += main.startLifetime.constantMax;

            if (duration > longest)
                longest = duration;
        }

        Destroy(gameObject, longest + extraLifetime);
    }
}