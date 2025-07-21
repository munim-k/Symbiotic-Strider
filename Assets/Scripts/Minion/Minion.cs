using System;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class Minion : MonoBehaviour {
    private Rigidbody rb;
    public static Action<Minion> OnMinionGrabbedOrAttacked;
    public static Action<Minion> OnMinionThrown;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Disable physics until we need it
    }

    private void Start()
    {
        Enemy.OnEnemyAttackedMinion += HandleEnemyAttackedMinion;
    }

    private void OnDestroy()
    {
        Enemy.OnEnemyAttackedMinion -= HandleEnemyAttackedMinion;
    }
    private void HandleEnemyAttackedMinion(Minion minion)
    {
        if (minion == this)
        {
            OnMinionGrabbedOrAttacked?.Invoke(this);
            Destroy(gameObject); // Destroy the minion when attacked by an enemy
        }
    }

    public void Grab()
    {
        rb.isKinematic = true;
        OnMinionGrabbedOrAttacked?.Invoke(this);
    }

    public void Throw(Vector3 throwDirection)
    {
        rb.isKinematic = false;
        rb.linearVelocity = throwDirection;
        rb.angularVelocity = Vector3.zero; // Reset angular velocity to prevent spinning
        
        OnMinionThrown?.Invoke(this);
    }
}
