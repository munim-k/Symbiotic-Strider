using UnityEngine;

public class IKTorso : MonoBehaviour {
    [SerializeField] private Transform[] ikTargets;
    [SerializeField] private IKTargetSettingsSO ikTargetSettings;
    [SerializeField] private Vector3 offsetVector;
    private Vector3 originalLocalPos;
    private Quaternion originalRot;
    void Start() {
        originalLocalPos = transform.localPosition;
        originalRot = transform.localRotation;
    }

    void FixedUpdate() {
        //Set the localPosition and localRotation of the torso to the original values + offset based on position of IKTargets
        if (ikTargets.Length > 0) {
            Vector3 averagePosition = Vector3.zero;
            foreach (Transform target in ikTargets) {
                averagePosition += target.position;
            }
            averagePosition /= ikTargets.Length;
            // Offset the torso position based on the average position of the IK targets
            transform.localPosition = originalLocalPos;
            Vector3 newpos = transform.position;
            averagePosition += offsetVector;
            newpos.y += averagePosition.y * ikTargetSettings.torsoTransformMultiplier;
            transform.position = newpos;


        }
    }
}
