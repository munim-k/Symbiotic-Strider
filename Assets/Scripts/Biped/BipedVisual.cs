using UnityEngine;

public class BipedVisual : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement; // Reference to the PlayerMovement script
    [SerializeField] private Animator bipedAnimator;
    private readonly string MOVE_BOOL = "Moving";

    private void Start() {
        playerMovement.OnMove+= HandleMovement;
    }

    void HandleMovement(bool move) {
        if (bipedAnimator == null) {
            Debug.LogError("BipedAnimator is not assigned in the inspector.");
            return;
        }

        bipedAnimator.SetBool(MOVE_BOOL, move);

    }
}
