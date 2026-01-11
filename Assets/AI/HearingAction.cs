using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Hearing", story: "Detects nearby Audio sources", category: "Action", id: "bc709b4ddc97364aa346fd3c1941bb73")]
public partial class HearingAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<bool> HearsNoise;
    [SerializeReference] public BlackboardVariable<Transform> SoundPosition;
    public float hearingRadius = 30f;
    public float memoryTime = 3f;

    float lastHeardTime;
    Transform cachedSoundTransform;

    protected override Status OnUpdate()
    {
        if (Agent?.Value == null)
            return Status.Running;

        bool heard = ScanForAudio();

        if (heard)
        {
            HearsNoise.Value = true;
            lastHeardTime = Time.time;
            return Status.Running;
        }

        // Memory decay
        if (HearsNoise.Value && Time.time - lastHeardTime > memoryTime)
        {
            HearsNoise.Value = false;
            SoundPosition.Value = null;
        }

        return Status.Running;
    }

    bool ScanForAudio()
    {
        Collider[] hits = Physics.OverlapSphere(
            Agent.Value.transform.position,
            hearingRadius
        );

        foreach (var hit in hits)
        {
            AudioSource audio = hit.GetComponentInChildren<AudioSource>();
            if (audio == null || !audio.isPlaying)
                continue;

            // We heard something
            if (SoundPosition.Value == null)
            {
                GameObject temp = new GameObject("SoundPosition");
                cachedSoundTransform = temp.transform;
                SoundPosition.Value = cachedSoundTransform;
            }

            SoundPosition.Value.position = audio.transform.position;

            Debug.DrawLine(
                Agent.Value.transform.position,
                audio.transform.position,
                Color.yellow,
                0.1f
            );

            Debug.Log($"Heard sound from {audio.gameObject.name}");

            return true;
        }

        return false;
    }
}

