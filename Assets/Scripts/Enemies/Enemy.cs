using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyBehaviourType.BehaviourType behaviourType;
    [SerializeField] private float movementSpeed = 3f;
    [SerializeField] private float playerRange = 6f;
    [SerializeField] private float meleeAttackRange = 2f;
    [SerializeField] private float attackDelay = 2f;
    private NavMeshAgent agent;
    private BaseEnemyBehaviour baseBehaviour;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component is missing on the enemy.");
        }

        //add the appropriate behaviour component based on the behaviour type
        switch (behaviourType)
        {
            case EnemyBehaviourType.BehaviourType.Aggressive:
                baseBehaviour = gameObject.AddComponent<AggressiveEnemyBehaviour>();
                break;
            case EnemyBehaviourType.BehaviourType.Defensive:
                baseBehaviour = gameObject.AddComponent<DefensiveEnemyBehaviour>();
                break;
            case EnemyBehaviourType.BehaviourType.Hybrid:
                baseBehaviour = gameObject.AddComponent<HybridEnemyBehaviour>();
                break;
            default:
                Debug.LogError("Unknown enemy behaviour type.");
                break;
        }
        
        if(baseBehaviour != null)
        {
            baseBehaviour.SetAttackRanges(meleeAttackRange, playerRange, movementSpeed, attackDelay);
        }
        else
        {
            Debug.LogError("BaseEnemyBehaviour component could not be created.");
        }
    }

    private void Start()
    {
        agent.speed = movementSpeed;
        if (gameObject.TryGetComponent(out BaseEnemyBehaviour baseBehaviour))
        {
            baseBehaviour.OnEnemyMove += HandleEnemyMove;
            baseBehaviour.OnEnemyAttack += HandleEnemyAttack;
            baseBehaviour.OnEnemySupport += HandleEnemySupport;
        }
        else
        {
            Debug.LogError("BaseEnemyBehaviour component is missing on the enemy.");
        }
    }

    private void HandleEnemySupport()
    {
        Debug.Log("Enemy is supporting from a distance.");
    }

    private void HandleEnemyMove(Vector3 movementVector)
    {
        agent.SetDestination(transform.position + movementVector);
        agent.isStopped = false;
    }

    private void HandleEnemyAttack()
    {
        agent.isStopped = true;

        Debug.Log("Melee attack executed on player.");
    }
}
