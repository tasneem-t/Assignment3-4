using UnityEngine;

public class move : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 10f;
    public float inputDeadzone = 0.15f;
    public bool smoothRotation = true;

    public Vector3 minBounds = new Vector3(-15f, 5f, -15f);
    public Vector3 maxBounds = new Vector3(15f, 5f, 15f);


    Camera cam;

    void Start()
    {
        cam = Camera.main;
        if (cam == null) Debug.LogWarning("MoveGhost4Dir: No Camera.main found. Tag a camera 'MainCamera' or assign one.");
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal"); // left/right arrows
        float vertical = Input.GetAxis("Vertical");     // up/down arrows

        // Movement based on ghost's facing direction
        Vector3 input = new Vector3(horizontal, 0, vertical);
        if (input.magnitude < inputDeadzone)
            return;
        Vector3 camForward = (cam != null) ? cam.transform.forward : Vector3.forward;
        Vector3 camRight = (cam != null) ? cam.transform.right : Vector3.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        // camera-relative move direction (world space)
        Vector3 moveDir = camForward * vertical + camRight * horizontal;
        if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();

        // Convert to local movement (relative to ghost’s rotation)
     

        transform.position += moveDir * speed * Time.deltaTime;
        Vector2 input2D = new Vector2(horizontal, vertical);
        if (Mathf.Abs(input2D.x) >= Mathf.Abs(input2D.y))
        {
            // horizontal dominates -> face left or right relative to camera
            if (input2D.x > 0f)
                FaceDirection(camRight);   // right
            else
                FaceDirection(-camRight);  // left
        }
        else
        {
            // vertical dominates -> face forward or backward relative to camera
            if (input2D.y > 0f)
                FaceDirection(camForward);   // forward (away from camera)
            else
                FaceDirection(-camForward);  // backward (toward camera)
        }
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
        pos.z = Mathf.Clamp(pos.z, minBounds.z, maxBounds.z);
        transform.position = pos;

    }

    void FaceDirection(Vector3 worldDir)
    {
        worldDir.y = 0f;
        if (worldDir.sqrMagnitude < 0.0001f) return;
        Quaternion targetRot = Quaternion.LookRotation(worldDir.normalized, Vector3.up);
        if (smoothRotation)
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        else
            transform.rotation = targetRot;
    }
}