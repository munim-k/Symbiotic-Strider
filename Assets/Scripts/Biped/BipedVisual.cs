using System;
using UnityEngine;

public class BipedVisual : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement; // Reference to the PlayerMovement script
    [SerializeField] private Animator bipedAnimator;
    private readonly string MOVE_BOOL = "Moving";
    private readonly string PICKUP_TRIGGER = "PickUp";
    private readonly string THROW_TRIGGER = "Throw";

    private void Start()
    {
        playerMovement.OnMove += HandleMovement;
        playerMovement.OnGrabbed += HandleGrab;
    }

    private void HandleGrab(bool grabbed)
    {
        if (grabbed)
        {
            bipedAnimator.SetTrigger(PICKUP_TRIGGER);
        }
        else
        {
            bipedAnimator.SetTrigger(THROW_TRIGGER);
        }
    }

    void HandleMovement(bool move) {
        if (bipedAnimator == null) {
            Debug.LogError("BipedAnimator is not assigned in the inspector.");
            return;
        }

        bipedAnimator.SetBool(MOVE_BOOL, move);

    }
}
