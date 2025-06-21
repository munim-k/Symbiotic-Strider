using UnityEngine;

public class IKBoneTarget : MonoBehaviour {
    [SerializeField] private IKBoneTarget[] oppositeLegs;
    [SerializeField] private Transform body;
    [SerializeField] private IKTargetSettingsSO ikTargetSettings;
    private float lerp = 1f;
    //Current position of the leg
    private Vector3 currentPosition;
    //Offset vector from the body to the leg
    [SerializeField] private Vector3 offsetVector;
    private Vector3 originalOffsetVector;
    //New position of the leg
    private Vector3 targetPosition;
    //Old position of the leg
    private Vector3 oldPosition;
    private Vector3 bodyVelocity;
    private Vector3 maxBodyVelocityPerMove;
    private Vector3 bodyOldPos;
    private bool move = false;
    private bool turntoMove = false;

    private void Start() {
        offsetVector += transform.position - body.position;
        originalOffsetVector = offsetVector;
        originalOffsetVector = offsetVector;
        currentPosition = transform.position;
        targetPosition = transform.position;
        oldPosition = transform.position;
        bodyOldPos = body.transform.position;
    }

    // Update is called once per frame
    private void FixedUpdate() {
        transform.position = currentPosition;
        //Rotate offsetVector around the body position
        offsetVector = body.rotation * originalOffsetVector;

        //Calculate current target position
        bodyVelocity = (body.position - bodyOldPos) / Time.fixedDeltaTime;
        bodyVelocity *= ikTargetSettings.movementMultiplier;
        if (bodyVelocity.magnitude > maxBodyVelocityPerMove.magnitude) {
            maxBodyVelocityPerMove = bodyVelocity;
        }
        Vector3 rayOrigin = body.position + offsetVector + maxBodyVelocityPerMove;
        rayOrigin.y += ikTargetSettings.rayVerticalOffset;
        Ray ray = new Ray(rayOrigin, Vector3.down);

        //Find target position
        if (Physics.Raycast(ray, out RaycastHit info, ikTargetSettings.rayVerticalOffset + 3, ikTargetSettings.raycastLayer)) {
            targetPosition = info.point;
        }
        if ((!move) && turntoMove) {
            if (Vector3.Distance(transform.position, targetPosition) > ikTargetSettings.stepDistance) {
                lerp = 0f;
                move = true;
            }
            else {
                //Dont need to move this frame so set turntoMove to false
                turntoMove = false;
            }
        }
        //Animation of leg
        if (move) {
            if (lerp < 1f && turntoMove) {
                Vector3 footPosition = Vector3.Lerp(oldPosition, targetPosition, lerp);
                //Leg should lift less if step is smaller
                footPosition.y += ikTargetSettings.stepHeightCurve.Evaluate(lerp) * ikTargetSettings.stepHeight;
                currentPosition = footPosition;
                //The bigger the distance the faster the leg moves
                lerp += Time.fixedDeltaTime * ikTargetSettings.speed;
            }
            if (lerp >= 1f) {
                move = false;
                maxBodyVelocityPerMove = Vector3.zero;
                currentPosition = targetPosition;
                oldPosition = targetPosition;
                turntoMove = false;
            }
        }
        bodyOldPos = body.position;
    }

    Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rot) {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = rot * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }


    public void SetTurn(bool turn) {
        turntoMove = turn;
    }
    public bool GetTurn() {
        return turntoMove;
    }

    public bool IsGrounded() {
        return !move;
    }




    private void OnDrawGizmos() {
        if (ikTargetSettings.drawGizmos) {

            Gizmos.color = UnityEngine.Color.red;
            Gizmos.DrawSphere(oldPosition, ikTargetSettings.stepDistance);
            Gizmos.color = UnityEngine.Color.green;
            Gizmos.DrawSphere(targetPosition, ikTargetSettings.stepDistance);
        }

    }
}
