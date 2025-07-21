using UnityEngine;

public class CopyScale : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;

    private void Start()
    {
        if (targetTransform == null)
        {
            Debug.LogError("Target Transform is not set in CopyScale.");
            return;
        }
        // Copy the scale from the target transform
        transform.localScale = targetTransform.localScale;
    }
}
