using UnityEngine;

public class IKBoneTarget : MonoBehaviour {
    [SerializeField] private IKBoneTarget[] oppositeLegs;
    [SerializeField] private Transform body;
    [SerializeField] private IKTargetSettingsSO ikTargetSettings;
    private float lerp = 0f;
    //Current position of the leg
    private Vector3 currentPosition;
    //Offset vector from the body to the leg
    [SerializeField] private Vector3 offsetVector;
    private Vector3 originalOffsetVector;
    //New position of the leg
    private Vector3 newPosition;
    //Old position of the leg
    private Vector3 oldPosition;
    private Vector3 bodyPosAtRayHit;
    private Vector3 bodyOldPos;
    private Vector3 bodyVelocity;
    private bool isMoving = false;
    private bool turntoMove = true;

    private void Start() {
        offsetVector += transform.position - body.position;
        originalOffsetVector = offsetVector;
        originalOffsetVector = offsetVector;
        currentPosition = transform.position;
        newPosition = transform.position;
        oldPosition = transform.position;
        bodyOldPos = body.transform.position;
        if (turntoMove) {
            foreach (var leg in oppositeLegs) {
                leg.turntoMove = false;
            }
        }
    }

    // Update is called once per frame
    private void FixedUpdate() {
        transform.position = currentPosition;
        bodyVelocity = (body.position - bodyOldPos) / Time.fixedDeltaTime;
        //Rotate offsetVector around the body position
        offsetVector = body.rotation * originalOffsetVector;
        if (!isMoving && turntoMove) {
            bodyVelocity *= ikTargetSettings.movementMultiplier;
            Vector3 rayOrigin = body.position + offsetVector + bodyVelocity;
            rayOrigin.y += ikTargetSettings.rayVerticalOffset;
            Ray ray = new Ray(rayOrigin, Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit info, ikTargetSettings.rayVerticalOffset + 3, ikTargetSettings.raycastLayer)) {
                //If the player has moved a certain distance from the old position, the leg will move to the new position
                if (Vector3.Distance(newPosition, info.point) > ikTargetSettings.stepDistance) {
                    newPosition = info.point;
                    lerp = 0f;
                }

            }
        }
        //Animation of leg
        if (lerp < 1f && turntoMove && !AreOppositeLegsMoving()) {
            isMoving = true;
            Vector3 footPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
            footPosition.y += ikTargetSettings.stepHeightCurve.Evaluate(lerp) * ikTargetSettings.stepHeight;
            currentPosition = footPosition;
            float bodyVelAdjust = 1f + bodyVelocity.magnitude * ikTargetSettings.velocitySpeedAdjust;
            lerp += Time.fixedDeltaTime * ikTargetSettings.speed * bodyVelAdjust;
        }
        if (lerp > 1f) {
            isMoving = false;
            currentPosition = newPosition;
            oldPosition = newPosition;
            foreach (var leg in oppositeLegs) {
                leg.turntoMove = true;
            }
            turntoMove = false;
        }
        bodyOldPos = body.position;
    }

    Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rot) {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = rot * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }


    private bool AreOppositeLegsMoving() {
        foreach (IKBoneTarget leg in oppositeLegs) {
            if (leg.isMoving) {
                return true;
            }
        }
        return false;
    }

    public bool IsGrounded() {
        return !isMoving;
    }




    private void OnDrawGizmos() {
        if (ikTargetSettings.drawGizmos) {

            Gizmos.color = UnityEngine.Color.red;
            Gizmos.DrawSphere(oldPosition, ikTargetSettings.stepDistance);
            Gizmos.color = UnityEngine.Color.green;
            Gizmos.DrawSphere(newPosition, ikTargetSettings.stepDistance);
        }

    }
}
