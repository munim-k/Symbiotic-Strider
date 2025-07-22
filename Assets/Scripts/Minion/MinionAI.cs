using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using System;
using System.Linq;
using System.Collections;
public class MinionAI : MonoBehaviour
{
    [SerializeField] private Minion minion; //get minion component from the Minion script

    [Header("Minion Movement Variables")]
    [SerializeField] private float detectionRadius = 3f;
    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private MinionAnimation minionAnimation;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float waitTimeBetweenAttacks = 1f;
    private Enemy closestEnemy;
    private List<Enemy> enemies;
    private float attackRange = 1.0f;
    private bool isGrabbed = false;
    private bool isAttacking = false;

    public enum MinionState
    {
        Idle,
        Moving,
        Attacking
    }

    public Action<MinionState> OnStateChange;
    private MinionState currentState = MinionState.Idle;
    private MinionState newState = MinionState.Idle;

    public static Action<Enemy, float> OnEnemyAttacked;


    private void Awake()
    {
        enemies = new List<Enemy>();
    }

    private void Start()
    {
        float scale = transform.localScale.x;
        detectionRadius *= scale;
        speed *= scale;
        rotationSpeed *= scale;
        damage *= scale;

        minion.OnThisMinionGrabbed += HandleMinionGrabbedOrAttacked;
        minion.OnThisMinionThrown += HandleMinionThrown;

        minionAnimation.OnAttackAnimationComplete += HandleAttackAnimationComplete;
        EnemyHealth.OnEnemyDied += EnemyHealth_OnEnemyDeath;
    }

    private void EnemyHealth_OnEnemyDeath(Enemy enemy)
    {
        if (enemies.Contains(enemy))
        {
            Debug.Log("Removed");
            enemies.Remove(enemy);
        }
    }

    private void HandleAttackAnimationComplete()
    {
        StartCoroutine(WaitForDelay());
        currentState = MinionState.Idle;
        OnEnemyAttacked?.Invoke(closestEnemy, damage);
    }

    private IEnumerator WaitForDelay()
    {
        yield return new WaitForSeconds(waitTimeBetweenAttacks);
        isAttacking = false;
    }

    private void OnDestroy()
    {
        minion.OnThisMinionGrabbed -= HandleMinionGrabbedOrAttacked;
        minion.OnThisMinionThrown -= HandleMinionThrown;

        minionAnimation.OnAttackAnimationComplete -= HandleAttackAnimationComplete;
        EnemyHealth.OnEnemyDied -= EnemyHealth_OnEnemyDeath;
    }

    private void HandleMinionThrown()
    {
        isGrabbed = false;
    }

    private void HandleMinionGrabbedOrAttacked()
    {
        isGrabbed = true;
        Debug.Log("Idle Now");
        SetState(MinionState.Idle);
        if (enemies != null)
        {
            enemies.Clear(); // Clear the list of enemies when the minion is grabbed or attacked
        }
    }

    private void FixedUpdate()
    {
        if (isGrabbed)
            return;
        HandleCollisionsWithEnemies();

        HandleMinionMovement();
    }

    private void HandleMinionMovement()
    {
        //check which enemy is the closest
        Enemy closestEnemy = null;
        float closestDistance = float.MaxValue;
        foreach (Enemy enemy in enemies)
        {
            if (enemy == null || !enemy.gameObject.activeInHierarchy)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy == null)
        {
            SetState(MinionState.Idle);
            return;
        }

        this.closestEnemy = closestEnemy;

        // Check if the closest enemy is within attack range
        if (closestDistance <= attackRange && !isAttacking)
        {
            SetState(MinionState.Attacking);
            Debug.Log("Setting state");
            isAttacking = true;
        }
        else
        {
            if (isAttacking)
                return;
            SetState(MinionState.Moving);
            //start moving towards the enemy while rotating towards it
            transform.position = Vector3.MoveTowards(transform.position, closestEnemy.transform.position, speed * Time.fixedDeltaTime);
            Vector3 direction = (closestEnemy.transform.position - transform.position).normalized;
            direction.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void HandleCollisionsWithEnemies()
    {
        //do a sphere cast to look for enemies
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        foreach (Collider hit in hits)
        {
            //check if the enemy is not already in the enemies list
            hit.TryGetComponent(out Enemy enemy);

            if (!enemies.Contains(enemy))
            {
                enemies.Add(enemy);
            }
        }
    }

    private void SetState(MinionState state)
    {
        newState = state;
        if (currentState != newState)
        {
            currentState = newState;
            OnStateChange?.Invoke(currentState);
        }
    }
}
