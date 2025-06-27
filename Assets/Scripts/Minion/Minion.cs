using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class Minion : MonoBehaviour {

    private Rigidbody rb;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Disable physics until we need it
    }

    public void Grab() {
        rb.isKinematic = true;
    }

    public void Throw(Vector3 throwDirection) {
        rb.isKinematic = false;
        rb.linearVelocity = throwDirection;
        rb.angularVelocity = Vector3.zero; // Reset angular velocity to prevent spinning
    }
}
