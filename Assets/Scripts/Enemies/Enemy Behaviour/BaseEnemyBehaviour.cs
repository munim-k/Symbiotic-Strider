using System;
using UnityEngine;

public class BaseEnemyBehaviour : MonoBehaviour
{
    protected GameObject player;
    protected float meleeAttackRange;
    protected float playerRange;
    protected float movementSpeed;
    protected float attackDelay;

    protected float attackDelayTimer;

    public void SetAttackRanges(float meleeAttackRange, float playerRange, float movementSpeed, float attackDelay)
    {
        this.meleeAttackRange = meleeAttackRange;
        this.playerRange = playerRange;
        this.movementSpeed = movementSpeed;
        this.attackDelay = attackDelay;
    }

    public Action<Vector3> OnEnemyMove;
    public Action OnEnemyAttack;
    public Action OnEnemySupport;

    private void Start()
    {
        attackDelayTimer = attackDelay;
        PlayerMovement playerMovement = FindAnyObjectByType<PlayerMovement>();
        if (playerMovement != null)
        {
            player = playerMovement.gameObject;
        }
        else
        {
            Debug.LogError("PlayerMovement not found in the scene.");
        }
    }
}
