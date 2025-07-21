using UnityEngine;

public class AggressiveEnemyBehaviour : BaseEnemyBehaviour
{
    private void FixedUpdate()
    {
        // move towrads the player and attack if close enough
        if (player != null)
        {
            Vector3 directionToClosest = GetClosestAggroObject(out Transform closestMinion);
            float distanceToClosest = directionToClosest.magnitude;

            //move towrads the player and stop at melee range and then attack
            if (distanceToClosest > meleeAttackRange)
            {
                Vector3 movementVector = directionToClosest.normalized * Time.fixedDeltaTime * movementSpeed;
                OnEnemyMove?.Invoke(movementVector);
            }
            else if (distanceToClosest <= meleeAttackRange)
            {
                attackDelayTimer += Time.fixedDeltaTime;
                if (attackDelayTimer >= attackDelay)
                {
                    OnEnemyAttack?.Invoke(closestMinion ? closestMinion.position : player.transform.position, EnemyBehaviourType.AttackType.Melee, closestMinion ? closestMinion.GetComponent<Minion>() : null);
                    attackDelayTimer = 0f; // Reset the attack delay timer
                }
            }
        }
    }
}