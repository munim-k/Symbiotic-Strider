using System;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour {
    public static PlayerMovement Instance { get; private set; }
    private NavMeshAgent agent;
    [SerializeField] private GameObject touchIndicatorObject;

    private float originalMaxSpeed = 5f;
    private float originalMinSpeed = 1f;

    private float maxSpeed = 5f; // Maximum speed of the player
    private float minSpeed = 1f; // Minimum speed of the player when stamina is low
    private bool moving = false;
    public Action<bool> OnMove;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        maxSpeed = originalMaxSpeed;
        minSpeed = originalMinSpeed;

        agent = GetComponent<NavMeshAgent>();

        PlayerStats.Instance.OnFrostProcced += HandleFrostProcced;
        PlayerStats.Instance.OnStunProcced += HandleStunProcced;
        PlayerStats.Instance.OnPlayerUpgraded += HandlePlayerUpgrade;

        UpgradeUI.Instance.OnMoveSpeedUpgraded += MoveSpeedUpgraded;
        GlyphsUI.Instance.OnCometButtonClicked += MoveSpeedUpgradedBetter;
    }

    private void MoveSpeedUpgradedBetter()
    {
        minSpeed *= 2f;
        maxSpeed *= 2f;
    }

    private void MoveSpeedUpgraded()
    {
        minSpeed *= 1.2f;
        maxSpeed *= 1.2f;
    }

    private void OnDestroy()
    {
        PlayerStats.Instance.OnFrostProcced -= HandleFrostProcced;
        PlayerStats.Instance.OnStunProcced -= HandleStunProcced;
        PlayerStats.Instance.OnPlayerUpgraded -= HandlePlayerUpgrade;

        UpgradeUI.Instance.OnMoveSpeedUpgraded -= MoveSpeedUpgraded;
        GlyphsUI.Instance.OnCometButtonClicked += MoveSpeedUpgradedBetter;
    }

    private void HandlePlayerUpgrade(float scale)
    {
        maxSpeed = originalMaxSpeed * scale;
        minSpeed = originalMinSpeed * scale;
    }

    private void HandleStunProcced(bool obj)
    {
        if (obj)
        {
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;
        }
    }

    private void HandleFrostProcced(bool obj)
    {
        if (obj)
        {
            minSpeed /= 2;
            maxSpeed /= 2;
        }
        else
        {
            minSpeed *= 2;
            maxSpeed *= 2;
        }
    }

    void Update()
    {
        HandleInput();

        // Rotate toward movement direction
        if (agent.velocity.magnitude > 0.1f)
        {
            Vector3 forward = agent.velocity.normalized;
            if (forward != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(forward) * Quaternion.Euler(0f, 180f, 0f);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
            }
        }

        if (agent.hasPath && agent.remainingDistance > agent.stoppingDistance)
        {
            if (!moving)
            {
                OnMove?.Invoke(true);
            }
            moving = true;
            agent.speed = Mathf.Lerp(minSpeed, maxSpeed, PlayerStats.Instance.GetStaminaRatio());
        }
        else
        {
            agent.speed = maxSpeed; // reset to max speed
            if (moving)
            {
                OnMove?.Invoke(false);
            }
            moving = false;
        }

        if (touchIndicatorObject != null)
        {
            touchIndicatorObject.SetActive(true);
            touchIndicatorObject.transform.position = agent.steeringTarget;
        }
        if (agent.hasPath && touchIndicatorObject != null)
        {
            touchIndicatorObject.transform.position = agent.steeringTarget;
        }
        if (touchIndicatorObject != null && !agent.hasPath)
        {
            Turn_Indicator_Off();
        }
    }

    void HandleInput() {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0) && PlayerStats.Instance.GetCurrentStamina() > 0f) {
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

    private void Turn_Indicator_Off() {
        touchIndicatorObject.SetActive(false);
    }

    public bool IsMoving() {
        return moving;
    }

}