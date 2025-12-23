using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleAI : MonoBehaviour
{
    public enum State { Idle, Patrol, Chase, Attack }
    public State state = State.Patrol;
    public Transform player;
    public float detectRange = 12f;
    public float loseRange = 18f;
    public float attackRange = 2f;
    public Transform[] patrolPoints;
    public float patrolWaitTime = 1.5f;
    public float attackCooldown = 1.0f;
    public int attackDamage = 10;
    public float faceTargetSpeed = 10f;

    public NavMeshAgent agent;
    public int patrolIndex = 0;
    public float waitTimer = 0f;
    public float attackTimer = 0f;

    public void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
        }
    }

    public void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        switch (state)
        {
            case State.Idle:
                if (dist <= detectRange)
                {
                    SetState(State.Chase);
                } 
                else if (patrolPoints != null && patrolPoints.Length > 0)
                {
                    SetState(State.Patrol);
                } 
                break;

            case State.Patrol:
                if (dist <= detectRange)
                {
                    SetState(State.Chase);
                } 
                break;

            case State.Chase:
                if (dist <= attackRange)
                {
                    SetState(State.Attack);
                } 
                else if (dist >= loseRange)
                {
                    SetState((patrolPoints != null && patrolPoints.Length > 0) ? State.Patrol : State.Idle);
                } 
                break;

            case State.Attack:
                if (dist > attackRange)
                {
                    SetState(State.Chase);
                } 
                break;
        }

        switch (state)
        {
            case State.Idle:   
                TickIdle(); 
                break;
            case State.Patrol: 
                TickPatrol(); 
                break;

            case State.Chase:  
                TickChase(); 
                break;

            case State.Attack: 
                TickAttack(); 
                break;
        }
    }

    void SetState(State newState)
    {
        if (state == newState) return;
        state = newState;

        switch (state)
        {
            case State.Idle:
                agent.isStopped = true;
                break;

            case State.Patrol:
                agent.isStopped = false;
                GoToNextPatrolPoint();
                break;

            case State.Chase:
                agent.isStopped = false;
                break;

            case State.Attack:
                agent.isStopped = true;
                attackTimer = 0f;
                break;
        }
    }

    void TickIdle()
    {
        //Nothing for now
    }

    void TickPatrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            SetState(State.Idle);
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= patrolWaitTime)
            {
                waitTimer = 0f;
                patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
                GoToNextPatrolPoint();
            }
        }
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;
        if (patrolPoints[patrolIndex] == null) return;

        agent.SetDestination(patrolPoints[patrolIndex].position);
    }

    void TickChase()
    {
        agent.SetDestination(player.position);
    }

    void TickAttack()
    {
        // Face the player
        Vector3 toPlayer = (player.position - transform.position);
        toPlayer.y = 0f;
        if (toPlayer.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(toPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, faceTargetSpeed * Time.deltaTime);
        }

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            attackTimer = attackCooldown;
            Debug.Log($"{name} attacked {player.name} for {attackDamage} damage");
        }
    }
}
