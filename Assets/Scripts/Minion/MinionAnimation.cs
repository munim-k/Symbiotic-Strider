using System;
using UnityEngine;

public class MinionAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private MinionAI minionAI;

    public Action OnAttackAnimationComplete;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        minionAI.OnStateChange += HandleStateChange;
    }
    private void OnDestroy()
    {
        minionAI.OnStateChange -= HandleStateChange;
    }

    private void HandleStateChange(MinionAI.MinionState state)
    {
        switch (state)
        {
            case MinionAI.MinionState.Idle:
                animator.SetBool("isRunning", false);
                animator.SetBool("isPunching", false);
                break;
            case MinionAI.MinionState.Moving:
                animator.SetBool("isRunning", true);
                animator.SetBool("isPunching", false);
                break;
            case MinionAI.MinionState.Attacking:
                animator.SetBool("isRunning", false);
                animator.SetBool("isPunching", true);
                break;
        }
    }

    public void OnAttackComplete()
    {
        animator.SetBool("isRunning", false);
        animator.SetBool("isPunching", false);
        OnAttackAnimationComplete?.Invoke();
    }
}
