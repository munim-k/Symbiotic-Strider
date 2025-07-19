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
    [SerializeField] private EnemyAnimation enemyAnimation;

    public enum EnemyState
    {
        Idle,
        Moving,
        Attacking,
        Supporting
    }
    public Action<EnemyState> OnEnemyStateChange;

    private NavMeshAgent agent;
    private bool isAttackingOrSupporting = false;
    private BaseEnemyBehaviour baseBehaviour;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component is missing on the enemy.");
        }

        baseBehaviour = (BaseEnemyBehaviour)gameObject.AddComponent(EnemyBehaviourType.behaviourClasses[behaviourType]);
        
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

        enemyAnimation.OnAnimationComplete += () => { isAttackingOrSupporting = false; };
    }

    private void Update()
    {
        //check if the agent has reached the destination
        if (agent.remainingDistance <= agent.stoppingDistance && !isAttackingOrSupporting)
        {
            OnEnemyStateChange?.Invoke(EnemyState.Idle);
        }
    }

    private void HandleEnemySupport()
    {
        isAttackingOrSupporting = true;
        agent.isStopped = true;

        OnEnemyStateChange?.Invoke(EnemyState.Supporting);
    }

    private void HandleEnemyMove(Vector3 movementVector)
    {
        if (!isAttackingOrSupporting)
        {
            agent.SetDestination(transform.position + movementVector);
            agent.isStopped = false;

            OnEnemyStateChange?.Invoke(EnemyState.Moving);
        }
    }

    private void HandleEnemyAttack(Vector3 playerPos)
    {
        isAttackingOrSupporting = true;
        agent.isStopped = true;

        transform.LookAt(new Vector3(playerPos.x, transform.position.y, playerPos.z));

        OnEnemyStateChange?.Invoke(EnemyState.Attacking);
    }
}
