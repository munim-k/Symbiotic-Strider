using UnityEngine;
using Unity.Cinemachine;
using System;

public class CinemachineTouchCamera : MonoBehaviour
{
    public CinemachineCamera cineCam;
    public Transform cameraFollowTarget;
   // public Transform PlayerPosition;
    public float panSpeed = 0.5f;
    public float zoomSpeed = 1f;
    public float minZoom = 5f;
    public float maxZoom = 20f;

    public Vector2 panLimitMin = new Vector2(-50, -50);
    public Vector2 panLimitMax = new Vector2(50, 50);

    public bool IsCameraInteracting { get; private set; }
    public Transform player; // assign this in the Inspector

    private Vector2 lastPanPosition;
    private int panFingerId;
    private float lastTapTime;
    private bool isPanning;
    private const float doubleTapThreshold = 0.3f;
    private Vector3 defaultPosition;

    void Start()
    {
        if (cineCam == null) cineCam = GetComponent<CinemachineCamera>();
        defaultPosition = cameraFollowTarget.position;

        PlayerStats.Instance.OnPlayerUpgraded += PlayerStats_OnPlayerUpgraded;
    }

    private void OnDestroy()
    {
        PlayerStats.Instance.OnPlayerUpgraded -= PlayerStats_OnPlayerUpgraded;
    }

    private void PlayerStats_OnPlayerUpgraded(float scale)
    {
        if (minZoom > 15)
            return;
        minZoom *= scale;
        maxZoom *= scale;

        panLimitMin *= scale;
        panLimitMax *= scale;
    }

    void Update()
    {
        IsCameraInteracting = false;

#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouse();
#else
        HandleTouch();
#endif
    }

    void HandleMouse()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            Zoom(scroll * 100f);
            IsCameraInteracting = true;
        }

        if (Input.GetMouseButtonDown(2)) // Middle mouse button
        {
            lastPanPosition = Input.mousePosition;
            isPanning = true;
            IsCameraInteracting = true;
        }
        else if (Input.GetMouseButton(2) && isPanning)
        {
            Vector2 delta = (Vector2)Input.mousePosition - lastPanPosition;
            Pan(delta);
            lastPanPosition = Input.mousePosition;
            IsCameraInteracting = true;
        }
        else if (Input.GetMouseButtonUp(2))
        {
            isPanning = false;
        }
    }

    void HandleTouch()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (Time.time - lastTapTime < doubleTapThreshold)
                {
                    ResetCamera();
                }
                lastTapTime = Time.time;

                lastPanPosition = touch.position;
                panFingerId = touch.fingerId;
                isPanning = true;
            }
            else if (touch.fingerId == panFingerId && touch.phase == TouchPhase.Moved && isPanning)
            {
                Vector2 delta = touch.position - lastPanPosition;
                Pan(delta);
                lastPanPosition = touch.position;
                IsCameraInteracting = true;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isPanning = false;
            }
        }
        else if (Input.touchCount == 2)
        {
            isPanning = false;
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            float prevMag = ((t0.position - t0.deltaPosition) - (t1.position - t1.deltaPosition)).magnitude;
            float currentMag = (t0.position - t1.position).magnitude;
            float delta = currentMag - prevMag;

            Zoom(delta * 0.02f);
            IsCameraInteracting = true;
        }
    }

    void Pan(Vector2 delta)
    {
        Vector3 move = new Vector3(-delta.x * panSpeed * Time.deltaTime, 0f, -delta.y * panSpeed * Time.deltaTime);
        Vector3 newPosition = cameraFollowTarget.position + move;

        // Define pan limits relative to the player's position
        float minX = player.position.x + panLimitMin.x;
        float maxX = player.position.x + panLimitMax.x;
        float minZ = player.position.z + panLimitMin.y;
        float maxZ = player.position.z + panLimitMax.y;

        // Clamp to player-relative bounds
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);

        cameraFollowTarget.position = newPosition;
    }



    void Zoom(float amount)
    {
        var lens = cineCam.Lens;
        if (lens.Orthographic)
        {
            lens.OrthographicSize -= amount * zoomSpeed * Time.deltaTime;
            lens.OrthographicSize = Mathf.Clamp(lens.OrthographicSize, minZoom, maxZoom);
            cineCam.Lens = lens;
        }
        else
        {
            lens.FieldOfView -= amount * zoomSpeed * Time.deltaTime;
            lens.FieldOfView = Mathf.Clamp(lens.FieldOfView, minZoom, maxZoom);
            cineCam.Lens = lens;
        }
    }

    void ResetCamera()
    {
        cameraFollowTarget.position = defaultPosition;
    }
}
