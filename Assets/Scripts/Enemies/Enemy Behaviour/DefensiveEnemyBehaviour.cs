using System.Runtime.CompilerServices;
using UnityEngine;

public class DefensiveEnemyBehaviour : BaseEnemyBehaviour
{
    private void FixedUpdate()
    {
        //stay at range from the player and do ranged attacks
        if (player != null)
        {
            Vector3 directionToPlayer = player.transform.position - transform.position;
            float distanceToPlayer = directionToPlayer.magnitude;

            //see if the player is within the player range and if it is then start moving away from the player until get out of range
            if (distanceToPlayer < playerRange)
            {
                Vector3 movementVector = -directionToPlayer.normalized * Time.fixedDeltaTime * movementSpeed;
                OnEnemyMove?.Invoke(movementVector);
            }
            else if (distanceToPlayer >= playerRange)
            {
                attackDelayTimer += Time.fixedDeltaTime;
                if (attackDelayTimer >= attackDelay)
                {
                    // Attack the player from a distance
                    OnEnemySupport?.Invoke();
                    attackDelayTimer = 0f; // Reset the attack delay timer
                }
            }
        }
    }
}
