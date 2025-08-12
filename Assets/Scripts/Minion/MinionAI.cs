using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections;

public class MinionAI : MonoBehaviour
{
    [SerializeField] private Minion minion;
    [Header("Minion Movement Variables")]
    [SerializeField] private float detectionRadius = 3f;
    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private MinionAnimation minionAnimation;
    public float damage = 10f;
    [SerializeField] private float waitTimeBetweenAttacks = 1f;
    private BoxCollider boxCollider;
    private Enemy closestEnemy;
    private float attackRange = 1.0f;
    private bool isGrabbed = false;
    private bool isAttacking = false;
    private bool isInAir = false;

    // Player following variables
    [Header("Following Player")]
    [SerializeField] private float followDistance = 2f;
    [SerializeField] private float followBuffer = 0.5f; // Stop before reaching exact follow distance
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.2f;
    private bool isGrounded = true;
    private Transform player; 

    public enum MinionState
    {
        Idle,
        Moving,
        Attacking,
    }

    public Action<MinionState> OnStateChange;
    private MinionState currentState = MinionState.Idle;

    public static Action<Enemy, float> OnEnemyAttacked;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        float scale = transform.localScale.x;
        detectionRadius *= scale;
        speed *= scale;
        rotationSpeed *= scale;
        attackRange *= scale;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogWarning("Player with tag 'Player' not found!");

        minion.OnThisMinionGrabbed += HandleMinionGrabbedOrAttacked;
        minion.OnThisMinionThrown += HandleMinionThrown;

        minionAnimation.OnAttackAnimationComplete += HandleAttackAnimationComplete;
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
    }

    private void HandleMinionThrown()
    {
        isGrabbed = false;
        isInAir = true;
        StartCoroutine(CheckLanding());
    }

    private IEnumerator CheckLanding()
    {
        while (isInAir)
        {
            Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
            isGrounded = Physics.Raycast(rayOrigin, Vector3.down, groundCheckDistance, groundLayer);
            
            if (isGrounded)
            {
                isInAir = false;
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    private void HandleMinionGrabbedOrAttacked()
    {
        isGrabbed = true;
        currentState = MinionState.Idle;
        OnStateChange?.Invoke(currentState);
        closestEnemy = null;
    }

    private void FixedUpdate()
    {
        if (isGrabbed || isInAir)
            return;

        CheckGrounded();
        if (!isGrounded)
        {
            currentState = MinionState.Idle;
            OnStateChange?.Invoke(currentState);
            return;
        }

        UpdateClosestEnemy();
        HandleMinionMovement();
    }

    private void CheckGrounded()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, groundCheckDistance, groundLayer);
        Debug.DrawRay(rayOrigin, Vector3.down * groundCheckDistance, isGrounded ? Color.green : Color.red);
    }

    private void HandleMinionMovement()
    {
        // Priority 1: Fight enemy
        if (closestEnemy != null)
        {
            float distanceToEnemy = Vector3.Distance(closestEnemy.transform.position, transform.position);
            
            if (distanceToEnemy <= attackRange)
            {
                if (!isAttacking)
                {
                    currentState = MinionState.Attacking;
                    OnStateChange?.Invoke(currentState);
                    isAttacking = true;
                }
            }
            else
            {
                if (isAttacking) return;

                currentState = MinionState.Moving;
                OnStateChange?.Invoke(currentState);
                MoveTowards(closestEnemy.transform.position);
            }
            return;
        }

        // Priority 2: Follow player if too far
        if (player != null)
        {
            float distToPlayer = Vector3.Distance(player.position, transform.position);
            if (distToPlayer > followDistance + followBuffer)
            {
                currentState = MinionState.Moving;
                OnStateChange?.Invoke(currentState);
                MoveTowards(player.position);
                return;
            }
        }

        // Priority 3: Idle
        if (currentState != MinionState.Idle)
        {
            currentState = MinionState.Idle;
            OnStateChange?.Invoke(currentState);
        }
    }

    private void MoveTowards(Vector3 target)
    {
        // Only move on the XZ plane
        Vector3 targetPosition = new Vector3(target.x, transform.position.y, target.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.fixedDeltaTime);
        
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void UpdateClosestEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);

        Enemy newClosestEnemy = null;
        float closestDistance = float.MaxValue;
        
        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent(out Enemy enemy) && enemy.gameObject.activeInHierarchy)
            {
                float dist = Vector3.Distance(enemy.transform.position, transform.position);
                if (dist < closestDistance)
                {
                    newClosestEnemy = enemy;
                    closestDistance = dist;
                }
            }
        }
        
        closestEnemy = newClosestEnemy;
    }
}