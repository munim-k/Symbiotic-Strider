using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMinionGrab : MonoBehaviour {
    //Singleton instance
    public static PlayerMinionGrab Instance { get; private set; }
    
    [SerializeField] private float minionTapDetectionRadius;
    [SerializeField] private float minionWalkDetectionRadius;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask minionLayer;
    [SerializeField] private Transform[] IKTargets;
    [SerializeField] private Transform[] IKTargetOriginals;
    [SerializeField] private float grabSpeed = 1f;
    [SerializeField] private float timeToTarget = 1f;
    [SerializeField] private PlayerMovement playerMovement;
    private float grablerp = 0f;
    private int currentIKTarget;
    private Minion currentMinion;
    private Vector3 grabTargetPosition;
    private Vector3 startGrabPosition;

    private enum State {
        Idle,
        Moving,
        Grabbing,
        Grabbed,
        Throwing
    }

    private State state;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        state = State.Idle;
    }

    private void Update() {
        HandleInput();

        switch (state) {
            case State.Idle:
                break;

            case State.Moving:
                if (currentMinion != null) {
                   Collider[] hits = Physics.OverlapSphere(transform.position, minionWalkDetectionRadius, minionLayer);
                    foreach (Collider hit in hits) {
                        if (hit.gameObject == currentMinion.gameObject) {
                            StartGrabbing();
                            break;
                        }
                    }
                }
                break;

            case State.Grabbing:
                PerformGrabbing();
                break;

            case State.Grabbed:
                KeepMinionHeld();
                break;

            case State.Throwing:
                state = State.Idle;
                break;
        }
    }

    private void HandleInput() {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, groundLayer)) {
                HandlePress(hit.point);
            }
        }
#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
            int fingerId = Input.GetTouch(0).fingerId;
            if(EventSystem.current.IsPointerOverGameObject(fingerId))
                return;
            
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, groundLayer)) {
                HandlePress(hit.point);
            }
        }
#endif
    }

    private void HandlePress(Vector3 point) {
        switch (state) {
            case State.Idle:
                if (!playerMovement.IsMoving()) {
                    CheckForMinion(point);
                }
                break;

            case State.Grabbed:
                ThrowMinion(point);
                break;
        }
    }

    private void CheckForMinion(Vector3 point) {
        Collider[] minions = Physics.OverlapSphere(point, minionTapDetectionRadius, minionLayer, QueryTriggerInteraction.Collide);
        if (minions.Length > 0) {
            Minion minion = minions[0].GetComponent<Minion>();
            if (minion != null) {
                currentMinion = minion;
                playerMovement.agent.SetDestination(minion.transform.position);
                state = State.Moving;
            }
        }
    }

    private void StartGrabbing() {
        state = State.Grabbing;
        grablerp = 0f;

        grabTargetPosition = currentMinion.transform.position;

        float closestDistance = float.MaxValue;
        for (int i = 0; i < IKTargets.Length; i++) {
            float dist = Vector3.Distance(grabTargetPosition, IKTargets[i].position);
            if (dist < closestDistance) {
                closestDistance = dist;
                currentIKTarget = i;
            }
        }

        startGrabPosition = IKTargets[currentIKTarget].position;

        currentMinion.Grab(); // Notify minion it's being grabbed
    }

    private void PerformGrabbing() {
        grablerp += grabSpeed * Time.deltaTime;

        if (grablerp < 1f) {
            IKTargets[currentIKTarget].position = Vector3.Lerp(startGrabPosition, grabTargetPosition, grablerp);
        } else if (grablerp < 2f) {
            float returnLerp = grablerp - 1f;
            IKTargets[currentIKTarget].position = Vector3.Lerp(grabTargetPosition, IKTargetOriginals[currentIKTarget].position, returnLerp);
            currentMinion.transform.position = IKTargets[currentIKTarget].position;
        } else {
            grablerp = 0f;
            state = State.Grabbed;
        }
    }

    private void KeepMinionHeld() {
        if (currentMinion != null) {
            currentMinion.transform.position = IKTargets[currentIKTarget].position;
        }
    }

    private void ThrowMinion(Vector3 point) {
        if (currentMinion != null) {
            Vector3 gravity = Physics.gravity;
            Vector3 startPos = currentMinion.transform.position;
            Vector3 displacement = point - startPos;

            Vector3 velocity = (displacement - 0.5f * gravity * timeToTarget * timeToTarget) / timeToTarget;

            currentMinion.Throw(velocity);

            currentMinion = null;
            state = State.Throwing;
        }
    }
}
