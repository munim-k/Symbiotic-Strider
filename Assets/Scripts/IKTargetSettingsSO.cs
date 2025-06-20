using UnityEngine;
[CreateAssetMenu()]
public class IKTargetSettingsSO : ScriptableObject {
    public float stepDistance;
    public float stepHeight;
    public float speed;
    public float rayVerticalOffset;
    public float movementMultiplier = 1;
    public float restRadius;
    public bool drawGizmos;
    public LayerMask raycastLayer;
    public AnimationCurve stepHeightCurve;
}
