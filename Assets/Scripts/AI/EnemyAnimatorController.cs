using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimationController : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] NavMeshAgent agent;

    void Awake()
    {
        if (anim == null)
            anim = GetComponentInParent<Animator>();

        if (agent == null)
            agent = GetComponentInParent<NavMeshAgent>();
    }

    void Update()
    {
        UpdateMovement();
    }

    void UpdateMovement()
    {
        if (anim == null || agent == null)
            return;

        float speed = agent.velocity.magnitude;

        anim.SetFloat("Speed", speed);
        anim.SetBool("IsMoving", speed > 0.05f);
    }

    public void PlayAttack(int attackIndex)
    {
        if (anim == null)
            return;

        anim.SetInteger("Attack", attackIndex);
    }

    public void ResetAttack()
    {
        if (anim == null)
            return;

        anim.SetInteger("Attack", 0);
    }

    public void PlayHit()
    {
        if (anim == null)
            return;

        anim.SetTrigger("Hit");
    }

    public void PlayDeath()
    {
        if (anim == null)
            return;

        anim.SetBool("Die", true);
    }

    public Animator GetAnimator()
    {
        return anim;
    }
}