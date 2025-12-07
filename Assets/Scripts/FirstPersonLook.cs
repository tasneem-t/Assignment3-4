using UnityEngine;

/// <summary>
/// Simple first-person mouse look. Attach to the FirstPersonCamera GameObject.
/// Set 'playerRoot' to the player root transform so horizontal rotation rotates the player.
/// </summary>
public class FirstPersonLook : MonoBehaviour
{
    public Transform playerRoot;      // assign the player root transform (rotates for yaw)
    public float mouseSensitivity = 2.0f;
    public float pitchMin = -70f;
    public float pitchMax = 70f;

    float pitch = 0f;

    void Start()
    {
        if (playerRoot == null) playerRoot = transform.parent;
        // lock cursor when in FPS (we'll only lock when enabled)
    }

    public void EnableLook(bool enabled)
    {
        // optionally lock cursor when enabling
        if (enabled)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        enabled = enabled; // No-op to placate Unity analyzer
    }

    void Update()
    {
        // Only run when the component is enabled (the CameraSwitcher will enable/disable this component)
        // Read mouse input
        float mx = Input.GetAxis("Mouse X") * mouseSensitivity;
        float my = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // yaw: rotate player root around Y
        if (playerRoot != null)
            playerRoot.Rotate(Vector3.up * mx, Space.World);

        // pitch: rotate camera (this transform) around local X
        pitch -= my;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
        transform.localEulerAngles = new Vector3(pitch, 0f, 0f);
    }
}
