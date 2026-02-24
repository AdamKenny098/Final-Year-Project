using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "HealthSense", story: "Grabs Health from Monster to Blackboard", category: "Action", id: "4a883ae3ef1e000115383db3c9b3dd64")]
public partial class HealthSenseAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> CurrentHealth;
    [SerializeReference] public BlackboardVariable<float> MaxHealth;
    [SerializeReference] public BlackboardVariable<bool> IsDead;

    Monster monster;

    protected override Status OnStart()
    {
        if (Agent == null)
        {
            Debug.LogError("HealthSenseAction: Agent variable not bound.");
            return Status.Failure;
        }

        if (Agent.Value == null)
            return Status.Running;

        monster = Agent.Value.GetComponent<Monster>();
        if (monster == null)
        {
            Debug.LogError($"HealthSenseAction: No Monster component on {Agent.Value.name}");
            return Status.Failure;
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (monster == null) return Status.Running;
        if (monster.stats == null || monster.stats.maxHealth <= 0f)
            return Status.Running;

        if (MaxHealth != null) MaxHealth.Value = monster.stats.maxHealth;
        if (CurrentHealth != null) CurrentHealth.Value = monster.stats.health;
        if (IsDead != null) IsDead.Value = monster.isDead;

        return Status.Running;
    }

    protected override void OnEnd()
    {
        monster = null;
    }
}

