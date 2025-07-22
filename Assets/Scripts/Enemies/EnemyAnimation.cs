using System;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    [SerializeField] private EnemyHealth enemyHealth;
    private Animator animator;
    public Action OnAnimationComplete;
    public Action OnMeleeAttackDamage;
    public Action OnSupportComplete;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component is missing on the enemy.");
        }
    }

    private void Start()
    {
        enemy.OnEnemyStateChange += HandleEnemyStateChange;
        enemyHealth.OnEnemyDeath += HandleEnemyDeath;
    }

    private void HandleEnemyDeath()
    {
        animator.SetBool("isDead", true);
    }

    private void OnDestroy()
    {
        enemy.OnEnemyStateChange -= HandleEnemyStateChange;
        enemyHealth.OnEnemyDeath -= HandleEnemyDeath;
    }

    private void HandleEnemyStateChange(Enemy.EnemyState state)
    {
        switch (state)
        {
            case Enemy.EnemyState.Idle:
                animator.SetBool("isMoving", false);
                animator.SetBool("isAttacking", false);
                animator.SetBool("isSupporting", false);
                animator.SetBool("isRangeAttacking", false);
                break;
            case Enemy.EnemyState.Moving:
                animator.SetBool("isMoving", true);
                animator.SetBool("isAttacking", false);
                animator.SetBool("isSupporting", false);
                animator.SetBool("isRangeAttacking", false);
                break;
            case Enemy.EnemyState.Attacking:
                animator.SetBool("isMoving", false);
                animator.SetBool("isAttacking", true);
                animator.SetBool("isSupporting", false);
                animator.SetBool("isRangeAttacking", false);
                break;
            case Enemy.EnemyState.Supporting:
                animator.SetBool("isMoving", false);
                animator.SetBool("isAttacking", false);
                animator.SetBool("isSupporting", true);
                animator.SetBool("isRangeAttacking", false);
                break;
            case Enemy.EnemyState.RangedAttacking:
                animator.SetBool("isMoving", false);
                animator.SetBool("isAttacking", false);
                animator.SetBool("isSupporting", false);
                animator.SetBool("isRangeAttacking", true);
                break;
        }
    }
    public void AnimationComplete()
    {
        OnAnimationComplete?.Invoke();
        animator.SetBool("isMoving", false);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isSupporting", false);
        animator.SetBool("isRangeAttacking", false);
    }

    public void MeleeAttackDamage()
    {
        OnMeleeAttackDamage?.Invoke();
    }

    public void SupportComplete()
    {
        OnSupportComplete?.Invoke();
    }

    public void DeathAnimationComplete()
    {
        Destroy(enemy.gameObject);
    }
}
