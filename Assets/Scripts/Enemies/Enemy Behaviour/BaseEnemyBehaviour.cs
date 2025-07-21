using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyBehaviour : MonoBehaviour
{
    protected GameObject player;
    protected float meleeAttackRange;
    protected float playerRange;
    protected float movementSpeed;
    protected float attackDelay;
    protected float attackDelayTimer;

    protected List<Minion> minions = new List<Minion>();

    public void SetAttackRanges(float meleeAttackRange, float playerRange, float movementSpeed, float attackDelay)
    {
        this.meleeAttackRange = meleeAttackRange;
        this.playerRange = playerRange;
        this.movementSpeed = movementSpeed;
        this.attackDelay = attackDelay;
    }

    public Action<Vector3> OnEnemyMove;
    public Action<Vector3, EnemyBehaviourType.AttackType, Minion> OnEnemyAttack;
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

        MinionHandler.Instance.OnMinionSpawned += HandleMinionSpawned;
        Minion.OnMinionGrabbedOrAttacked += HandleMinionGrabbedOrAttacked;
        Minion.OnMinionThrown += HandleMinionThrown;
    }

    private void OnDestroy()
    {
        MinionHandler.Instance.OnMinionSpawned -= HandleMinionSpawned;
        Minion.OnMinionGrabbedOrAttacked -= HandleMinionGrabbedOrAttacked;
        Minion.OnMinionThrown -= HandleMinionThrown;
    }

    private void HandleMinionThrown(Minion minion)
    {
        //add this minion to the list of minions
        if (!minions.Contains(minion))
        {
            minions.Add(minion);
        }
    }

    private void HandleMinionGrabbedOrAttacked(Minion minion)
    {
        //remove this minion from the list of minions   
        if (minions.Contains(minion))
        {
            minions.Remove(minion);
        }
    }

    private void HandleMinionSpawned(Minion minion)
    {
        minions.Add(minion);
    }

    protected Vector3 GetClosestAggroObject(out Transform closestMinion)
    {
        Vector3 directionToClosest = player.transform.position - transform.position;
        float distanceToClosest = directionToClosest.magnitude;
        Transform newClosestMinion = null;
        //check distance to all the minions and the player
        foreach (Minion minion in minions)
        {
            if (minion != null)
            {
                Vector3 directionToMinion = minion.transform.position - transform.position;
                float distanceToMinion = directionToMinion.magnitude;
                if (distanceToMinion < distanceToClosest)
                {
                    directionToClosest = directionToMinion;
                    distanceToClosest = distanceToMinion;
                    newClosestMinion = minion.transform;
                }
            }
        }
        closestMinion = newClosestMinion;
        return directionToClosest;
    }
}
