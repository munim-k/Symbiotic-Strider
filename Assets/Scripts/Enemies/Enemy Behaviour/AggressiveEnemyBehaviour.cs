using UnityEngine;

public class AggressiveEnemyBehaviour : BaseEnemyBehaviour
{
    private void FixedUpdate()
    {
        // move towrads the player and attack if close enough
        if (player != null)
        {
            Vector3 directionToPlayer = player.transform.position - transform.position;
            float distanceToPlayer = directionToPlayer.magnitude;

            //move towrads the player and stop at melee range and then attack
            if (distanceToPlayer > meleeAttackRange)
            {
                Vector3 movementVector = directionToPlayer.normalized * Time.fixedDeltaTime * movementSpeed;
                OnEnemyMove?.Invoke(movementVector);
            }
            else if (distanceToPlayer <= meleeAttackRange)
            {
                attackDelayTimer += Time.fixedDeltaTime;
                if (attackDelayTimer >= attackDelay)
                {
                    OnEnemyAttack?.Invoke(player.transform.position);
                    attackDelayTimer = 0f; // Reset the attack delay timer
                }
            }
        }
    }
}