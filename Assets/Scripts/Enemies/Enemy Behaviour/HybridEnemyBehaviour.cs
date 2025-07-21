using System.Security.Cryptography;
using UnityEngine;

public class HybridEnemyBehaviour : BaseEnemyBehaviour
{
    private float randomNumber;
    private bool hasSetRandomNumber = false;
    private void FixedUpdate()
    {
        // Hybrid behavior: switch between aggressive and defensive based on distance to player
        if (player != null)
        {
            Vector3 directionToClosest = GetClosestAggroObject(out Transform closestMinion);
            float distanceToClosest = directionToClosest.magnitude;
            
            // check if the player is within melee attack range
            if (distanceToClosest <= meleeAttackRange)
            {
                hasSetRandomNumber = false;
                attackDelayTimer += Time.fixedDeltaTime;
                if (attackDelayTimer >= attackDelay)
                {
                    OnEnemyAttack?.Invoke(closestMinion ? closestMinion.position: player.transform.position, EnemyBehaviourType.AttackType.Melee, closestMinion ? closestMinion.GetComponent<Minion>() : null);
                    attackDelayTimer = 0f; // Reset the attack delay timer
                }
            }
            else if (distanceToClosest < playerRange)
            {
                if (!hasSetRandomNumber)
                {
                    randomNumber = Random.Range(0f, 1f);
                    hasSetRandomNumber = true;
                }
                //randomly behave as aggressive or defensive
                if (randomNumber < 0.5f)
                {
                    // Aggressive behavior: move towards the player
                    Vector3 movementVector = directionToClosest.normalized * Time.fixedDeltaTime * movementSpeed;
                    OnEnemyMove?.Invoke(movementVector);
                }
                else
                {
                    // Defensive behavior: move away from the player
                    Vector3 movementVector = -directionToClosest.normalized * Time.fixedDeltaTime * movementSpeed;
                    OnEnemyMove?.Invoke(movementVector);
                }
            }
            else
            {
                hasSetRandomNumber = false;
                attackDelayTimer += Time.fixedDeltaTime;
                if (attackDelayTimer >= attackDelay)
                {
                    if (Random.value < 0.5f)
                    {
                        // Randomly choose to either attack or support
                        OnEnemyAttack?.Invoke(player.transform.position, EnemyBehaviourType.AttackType.Ranged, null);
                    }
                    else
                    {
                        // Support from a distance
                        OnEnemySupport?.Invoke();
                    }
                    attackDelayTimer = 0f; // Reset the attack delay timer
                }
            }
        }
    }
}
