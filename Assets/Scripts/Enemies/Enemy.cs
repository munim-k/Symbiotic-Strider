using System;
using System.Collections.Generic;
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
    [SerializeField] private List<GameObject> projectilePrefabs;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float rangeAttackBuildUp = 5f;
    private Vector3 closestHIttablePosition;
    private Minion closestMinion;

    public enum EnemyState
    {
        Idle,
        Moving,
        Attacking,
        RangedAttacking,
        Supporting
    }
    public Action<EnemyState> OnEnemyStateChange;
    public static Action<float> OnEnemyAttackedPlayer;
    public static Action<Minion> OnEnemyAttackedMinion;

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

        if (baseBehaviour != null)
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
        enemyAnimation.OnMeleeAttackDamage += () =>
        {
            //check if the player is in melee range
            if (Vector3.Distance(transform.position, closestHIttablePosition) <= meleeAttackRange)
            {
                if (closestMinion != null)
                {
                    OnEnemyAttackedMinion?.Invoke(closestMinion);
                }
                else
                {
                    OnEnemyAttackedPlayer?.Invoke(damage);
                }
            }
        };
    }
    private void OnDestroy()
    {
        baseBehaviour.OnEnemyMove -= HandleEnemyMove;
        baseBehaviour.OnEnemyAttack -= HandleEnemyAttack;
        baseBehaviour.OnEnemySupport -= HandleEnemySupport;
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

        closestHIttablePosition = transform.position + movementVector;
    }

    private void HandleEnemyAttack(Vector3 playerPos, EnemyBehaviourType.AttackType attackType, Minion closestMinion)
    {
        isAttackingOrSupporting = true;
        agent.isStopped = true;
        this.closestMinion = closestMinion;

        transform.LookAt(new Vector3(playerPos.x, transform.position.y, playerPos.z));

        OnEnemyStateChange?.Invoke(attackType == EnemyBehaviourType.AttackType.Melee ? EnemyState.Attacking : EnemyState.RangedAttacking);
        //spawn the projectile and throw towards the player
        if (attackType == EnemyBehaviourType.AttackType.Ranged)
        {
            int random = UnityEngine.Random.Range(0, projectilePrefabs.Count);
            GameObject projectile = Instantiate(projectilePrefabs[random], transform.position, transform.rotation);
            projectile.GetComponent<ProjectileType>().procBuildUp = rangeAttackBuildUp;
            
            projectile.transform.localScale = transform.localScale;
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            if (rb != null)
            {
                float timeToDestroy = LaunchProjectile(rb, playerPos, 45f); // Launch at a 45-degree angle
                Destroy(projectile, timeToDestroy + 2f); // Destroy after some time
            }
            else
            {
                Debug.LogError("Rigidbody component is missing on the projectile.");
            }

        }
    }

    float LaunchProjectile(Rigidbody rb, Vector3 target, float angleDegrees)
    {
        Vector3 startPos = transform.position;
        Vector3 direction = target - startPos;

        // Horizontal distance
        Vector3 directionXZ = new Vector3(direction.x, 0, direction.z);
        float distance = directionXZ.magnitude;

        // Vertical difference
        float heightDifference = direction.y;

        // Convert angle to radians
        float angleRad = angleDegrees * Mathf.Deg2Rad;

        // Calculate initial velocity using physics
        float gravity = Physics.gravity.y;
        float velocitySquared = (gravity * distance * distance) /
                                (2 * (heightDifference - Mathf.Tan(angleRad) * distance) * Mathf.Pow(Mathf.Cos(angleRad), 2));

        if (velocitySquared <= 0) return 0f; // Target unreachable

        float velocity = Mathf.Sqrt(velocitySquared);

        // Final velocity vector
        Vector3 launchVelocity = directionXZ.normalized * velocity * Mathf.Cos(angleRad);
        launchVelocity.y = velocity * Mathf.Sin(angleRad);

        rb.linearVelocity = launchVelocity; // Directly set velocity for better control

        float timeToTarget = distance / (velocity * Mathf.Cos(angleRad));
        return timeToTarget; // Return the time it will take to reach the target
    }
}
