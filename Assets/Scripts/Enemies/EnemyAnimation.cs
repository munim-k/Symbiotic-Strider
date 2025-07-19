using System;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    private Animator animator;
    public Action OnAnimationComplete;

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
        if (enemy != null)
        {
            enemy.OnEnemyStateChange += HandleEnemyStateChange;
        }
        else
        {
            Debug.LogError("Enemy reference is not set.");
        }
    }

    private void HandleEnemyStateChange(Enemy.EnemyState state)
    {
        switch (state)
        {
            case Enemy.EnemyState.Idle:
                animator.SetBool("isMoving", false);
                animator.SetBool("isAttacking", false);
                animator.SetBool("isSupporting", false);
                break;
            case Enemy.EnemyState.Moving:
                animator.SetBool("isMoving", true);
                animator.SetBool("isAttacking", false);
                animator.SetBool("isSupporting", false);
                break;
            case Enemy.EnemyState.Attacking:
                animator.SetBool("isMoving", false);
                animator.SetBool("isAttacking", true);
                animator.SetBool("isSupporting", false);
                break;
            case Enemy.EnemyState.Supporting:
                animator.SetBool("isMoving", false);
                animator.SetBool("isAttacking", false);
                animator.SetBool("isSupporting", true);
                break;
        }
    }
    public void AnimationComplete()
    {
        OnAnimationComplete?.Invoke();
        animator.SetBool("isMoving", false);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isSupporting", false);
    }

}
