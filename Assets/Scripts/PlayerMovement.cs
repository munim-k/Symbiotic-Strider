using MagicPigGames;
using System;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour {
    public float maxStamina = 100f;
    public float staminaDrainRate = 10f; // per second while moving
    public float staminaRegenRate = 5f;  // per second while idle

    [SerializeField]
    private float currentStamina;
    private NavMeshAgent agent;
    private bool isTouching = false;
    [SerializeField]
    private GameObject touchIndicatorObject;

    [SerializeField]
    private float MaxSpeed = 5f; // Maximum speed of the player
    [SerializeField]
    private float MinSpeed = 1f; // Minimum speed of the player when stamina is low
    [SerializeField]
    private ProgressBar staminaBar; // Reference to the stamina bar UI

    private bool moving = false;
    public Action<bool> OnMove;
    void Start() {
        agent = GetComponent<NavMeshAgent>();
        currentStamina = maxStamina;
    }

    void Update() {
        HandleInput();

        // Rotate toward movement direction
        if (agent.velocity.magnitude > 0.1f) {
            Vector3 forward = agent.velocity.normalized;
            if (forward != Vector3.zero) {
                Quaternion lookRotation = Quaternion.LookRotation(forward) * Quaternion.Euler(0f, 180f, 0f);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
            }

        }

        // Stamina logic
        if (agent.hasPath && agent.remainingDistance > agent.stoppingDistance) {
            if (!moving) {
                OnMove?.Invoke(true);
            }
            moving = true;
            currentStamina -= staminaDrainRate * Time.deltaTime;
            currentStamina = Mathf.Max(currentStamina, 0f);

            // Scale agent speed based on stamina level (minimum 20% speed)
            float staminaRatio = Mathf.Clamp01(currentStamina / maxStamina);
            agent.speed = Mathf.Lerp(1f, 5f, staminaRatio); // 1 = min speed, 5 = max speed
        }
        else {
            // Regenerate stamina when idle
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
            agent.speed = 5f; // reset to max speed
            if(moving)
            {
                OnMove?.Invoke(false);
            }
            moving = false;

        }

        if (touchIndicatorObject != null) {
            touchIndicatorObject.SetActive(true);
            touchIndicatorObject.transform.position = agent.steeringTarget;
        }
        if (agent.hasPath && touchIndicatorObject != null) {
            touchIndicatorObject.transform.position = agent.steeringTarget;
        }
        if (touchIndicatorObject != null && !agent.hasPath) {
            Turn_Indicator_Off();
        }
        staminaBar.SetProgress(currentStamina / maxStamina);
    }

    void HandleInput() {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0) && currentStamina > 0f) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                agent.SetDestination(hit.point);
            }
        }
#elif UNITY_ANDROID || UNITY_IOS
    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && currentStamina > 0f)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            agent.SetDestination(hit.point);
        }
    }
#endif
    }

    public float GetStaminaPercentage() {
        return currentStamina / maxStamina;
    }

    private void Turn_Indicator_Off() {
        touchIndicatorObject.SetActive(false);
    }

    public bool IsMoving() {
        return moving;
    }

}