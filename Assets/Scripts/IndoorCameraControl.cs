using UnityEngine;

public class IndoorCameraControl : MonoBehaviour
{
    public Transform target;

    [Header("Distance")]
    public float distance = 5.0f;
    public float minDistance = 3.0f;
    public float maxDistance = 6.0f;   // smaller max for indoor use

    [Header("Rotation")]
    public float rotationSpeed = 200.0f;
    public float minY = 10.0f;         // clamp vertical angles tighter
    public float maxY = 60.0f;

    [Header("Smoothing")]
    public float smoothSpeed = 10.0f;  // MUCH higher, used with deltaTime

    [Header("Zoom")]
    public float zoomSpeed = 5.0f;

    [Header("Collision")]
    public float collisionOffset = 0.3f;   // how far from wall to keep camera
    public LayerMask collisionLayers = ~0; // by default, collide with everything

    private float x = 0.0f;
    private float y = 0.0f;
    private Quaternion targetRotation;
    private Vector3 desiredPosition;   // unclamped position
    private Vector3 targetPosition;    // after collision resolution

    void Start()
    {
        if (!target)
        {
            Debug.LogWarning("CameraControl: No target assigned.");
            return;
        }

        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        targetRotation = Quaternion.Euler(y, x, 0);
        desiredPosition = targetRotation * new Vector3(0.0f, 0.0f, -distance) + target.position;
        targetPosition = desiredPosition;
    }

    void Update()
    {
        if (!target) return;

        // Rotate while holding left mouse (change to 1 for RMB if you like)
        if (Input.GetMouseButton(0))
        {
            x += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float yRotation = y - Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            y = Mathf.Clamp(yRotation, minY, maxY);
        }

        // Build rotation from angles
        targetRotation = Quaternion.Euler(y, x, 0);

        // Zoom with scroll wheel
        distance = Mathf.Clamp(
            distance - Input.GetAxis("Mouse ScrollWheel") * zoomSpeed,
            minDistance,
            maxDistance
        );

        // Ideal (unblocked) camera position
        desiredPosition = targetRotation * new Vector3(0.0f, 0.0f, -distance) + target.position;

        // COLLISION CHECK
        Vector3 dir = desiredPosition - target.position;
        float desiredDist = dir.magnitude;
        if (desiredDist > 0.001f)
        {
            dir /= desiredDist; // normalize

            RaycastHit hit;
            // Cast from target towards desired camera position
            if (Physics.Raycast(target.position, dir, out hit, desiredDist, collisionLayers))
            {
                // Place camera in front of the surface, but never closer than minDistance
                float adjustedDist = hit.distance - collisionOffset;

                // Clamp between minDistance and maxDistance so we don't zoom into the ghost
                adjustedDist = Mathf.Clamp(adjustedDist, minDistance, maxDistance);

                targetPosition = target.position + dir * adjustedDist;
            }

            else
            {
                // Nothing in the way, use full desired position
                targetPosition = desiredPosition;
            }
        }
        else
        {
            targetPosition = desiredPosition;
        }
    }

    void LateUpdate()
    {
        if (!target) return;

        // Smooth rotation and position using deltaTime
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            smoothSpeed * Time.deltaTime
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}
