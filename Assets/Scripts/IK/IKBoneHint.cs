using UnityEngine;

public class IKBoneHint : MonoBehaviour {
    [SerializeField] private Transform body;
    [SerializeField] private IKTargetSettingsSO ikTargetSettings;
    private float lerp = 0f;
    private Vector3 oldPosition;
    private Vector3 newPosition;
    private Vector3 offsetVector;
    private Vector3 originalOffsetVector;
    private bool rotating = false;
    private void Start() {
        offsetVector = transform.position - body.position;
    }

    private void Update() {

        transform.position = body.position + offsetVector;

    }
}

