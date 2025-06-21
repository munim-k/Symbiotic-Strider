using UnityEngine;

public class IKTorso : MonoBehaviour {
    [SerializeField] private Transform[] ikTargets;
    [SerializeField] private IKTargetSettingsSO ikTargetSettings;
    private Vector3 originalLocalPos;
    private Quaternion originalRot;
    void Start() {
        originalLocalPos = transform.localPosition;
        originalRot = transform.localRotation;
    }

    void FixedUpdate() {
        //Set the localPosition and localRotation of the torso to the original values + offset based on position of IKTargets
        transform.localPosition = originalLocalPos;
        transform.localRotation = originalRot;
        if (ikTargets.Length > 0) {
            Vector3 averagePosition = Vector3.zero;
            foreach (Transform target in ikTargets) {
                averagePosition += target.position;
            }
            averagePosition /= ikTargets.Length;
            // Offset the torso position based on the average position of the IK targets
            averagePosition -= transform.position;
            averagePosition.x = 0;
            averagePosition.y = 0;
            transform.localPosition += averagePosition * ikTargetSettings.torsoTransformMultiplier;
        }

    }
}
