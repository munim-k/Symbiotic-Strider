using UnityEngine;

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

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }
    
    private enum State
    {
        Idle,
        Moving,
        Grabbing,
        Grabbed,
        Throwing
    }

    private State state;
    private void Start() {
        state = State.Idle;
    }
    // Update is called once per frame
    void Update() {
        HandleInput();
        switch (state) {
            case State.Idle:
                break;
            case State.Moving:
                //If we are moving then check if currentMinion is within grabbing distance and if so start grabbing
                if (currentMinion != null) {
                    float distance = Vector3.Distance(transform.position, currentMinion.transform.position);
                    if (distance < minionWalkDetectionRadius) {
                        state = State.Grabbing;
                        if (currentMinion != null) {
                            //Check which ikTarget is closer to the minion and store its index
                            float closestDistance = float.MaxValue;
                            for (int i = 0; i < IKTargets.Length; i++) {
                                distance = Vector3.Distance(currentMinion.transform.position, IKTargets[i].position);
                                if (distance < closestDistance) {
                                    closestDistance = distance;
                                    currentIKTarget = i;
                                }
                            }
                        }
                        currentMinion.Grab(); // Call the Grab method on the minion
                    }
                }
                break;
            case State.Grabbing:
                Grabbing();
                break;
            case State.Grabbed:
                Grabbed();
                break;
            case State.Throwing:
                // If we are throwing then we can reset the state to Idle
                state = State.Idle; // Reset state to Idle after throwing
                break;
            default:
                break;
        }
    }

    private void Grabbing() {
        //Move currentIKTarget to the minion position
        if (currentMinion != null) {
            Vector3 targetPosition = currentMinion.transform.position;
            grablerp += grabSpeed * Time.deltaTime;
            if (grablerp < 1f) {
                IKTargets[currentIKTarget].position = Vector3.Lerp(IKTargets[currentIKTarget].position, targetPosition, grablerp);
            }
            else if (grablerp >= 1f && grablerp <= 2f) {
                IKTargets[currentIKTarget].position = Vector3.Lerp(IKTargets[currentIKTarget].position, IKTargetOriginals[currentIKTarget].position, grablerp - 1f);
                currentMinion.gameObject.transform.position = IKTargets[currentIKTarget].position; // Snap the minion to the IK target position

            }
            else if (grablerp > 2f) {
                grablerp = 0f; // Reset the lerp value
                state = State.Grabbed; // Move to Grabbed state after grabbing
            }
        }
        else {
            state = State.Idle; // If no minion is found, reset to Idle
        }
    }
    private void Grabbed() {
        if (currentMinion != null) {
            currentMinion.gameObject.transform.position = IKTargets[currentIKTarget].position;
        }
    }
    void HandleInput() {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, groundLayer)) {
                HandlePress(hit.point);
            }
        }
#elif UNITY_ANDROID || UNITY_IOS
    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        if (Physics.Raycast(ray, out RaycastHit hit,float.MaxValue, groundLayer))
        {
            HandlePress(hit.point);
        }
    }
#endif
    }

    private void HandlePress(Vector3 point) {
        switch (state) {
            case State.Idle:
                //Only allow player to go after a minion if they are currently Idle 
                if (!playerMovement.IsMoving())
                    CheckForMinion(point);
                break;
            case State.Moving:
                break;
            case State.Grabbing:
                break;
            case State.Grabbed:
                ThrowMinion(point);
                break;
            default:
                break;
        }
    }
    private void CheckForMinion(Vector3 point) {
        Collider[] minions = Physics.OverlapSphere(point, minionTapDetectionRadius, minionLayer, QueryTriggerInteraction.Collide);
        if (minions.Length > 0) {
            // Grab the first minion found
            Minion minion = minions[0].GetComponent<Minion>();
            if (minion != null) {
                state = State.Moving; // Set state to Moving to start the grabbing process
                currentMinion = minion;
            }
        }
    }

    private void ThrowMinion(Vector3 point) {
        if (currentMinion != null) {
            Vector3 gravity = Physics.gravity;
            Vector3 startPos = currentMinion.transform.position;
            Vector3 targetPos = point;
            Vector3 displacement = targetPos - startPos;

            Vector3 velocity = (displacement - 0.5f * gravity * timeToTarget * timeToTarget) / timeToTarget;
            currentMinion.Throw(velocity); // Adjust the throw force as needed
            currentMinion = null; // Clear the current minion after throwing
            state = State.Throwing; // Reset state to Idle after throwing
        }

    }


}
