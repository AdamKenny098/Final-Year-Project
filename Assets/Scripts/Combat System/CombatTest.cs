using UnityEngine;

public class CombatTest : MonoBehaviour
{
    public Entity attacker;
    public Entity target;
    public AbilityData ability;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (attacker == null) attacker = GetComponent<Entity>();
            if (attacker == null || target == null || ability == null) return;

            Vector3 hitPoint = target.transform.position;
            bool ok = attacker.TryUseAbilityOn(target, ability, hitPoint);

            Debug.Log("UseAbility=" + ok + " TargetHP=" + target.stats.health);
        }
    }
}
