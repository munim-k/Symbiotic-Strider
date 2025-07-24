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
    public float damage = 10f;
    [SerializeField] private float waitTimeBetweenAttacks = 1f;
    private Enemy closestEnemy;
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


    private void Start()
    {
        float scale = transform.localScale.x;
        detectionRadius *= scale;
        speed *= scale;
        rotationSpeed *= scale;

        minion.OnThisMinionGrabbed += HandleMinionGrabbedOrAttacked;
        minion.OnThisMinionThrown += HandleMinionThrown;

        minionAnimation.OnAttackAnimationComplete += HandleAttackAnimationComplete;
        EnemyHealth.OnEnemyDied += EnemyHealth_OnEnemyDeath;
    }

    private void EnemyHealth_OnEnemyDeath(Enemy enemy)
    {
        closestEnemy = null;
        StartCoroutine(DelayToAggro());
    }

    private IEnumerator DelayToAggro()
    {
        yield return new WaitForSeconds(1f);
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
        SetState(MinionState.Idle);
        closestEnemy = null;
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
        if (closestEnemy == null)
        {
            SetState(MinionState.Idle);
            return;
        }

        // Check if the closest enemy is within attack range
        if (Vector3.Distance(closestEnemy.transform.position, transform.position) <= attackRange && !isAttacking)
        {
            SetState(MinionState.Attacking);
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
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);

        Enemy cEnemy = null;
        float cDistance = float.MaxValue;
        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent(out Enemy enemy) && enemy.gameObject.activeInHierarchy && cDistance > Vector3.Distance(enemy.transform.position, transform.position))
            {
                cEnemy = enemy;
            }
        }
        if(!(cEnemy == null && closestEnemy != null))
            closestEnemy = cEnemy;
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
