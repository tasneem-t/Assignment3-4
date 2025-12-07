using UnityEngine;

/// <summary>
/// Attach to the player root. Assign the thirdPersonCamera and firstPersonCamera references.
/// Press V to toggle between views.
/// </summary>
public class CameraSwitcher : MonoBehaviour
{
    public Camera Camera;      // drag your existing 3rd person camera here
    public Camera FirstPersonCamera;      // drag the FirstPersonCamera here
    public FirstPersonLook fpLook;        // drag the FirstPersonLook (component on firstPersonCamera)
    public GameObject playerVisuals;      // optional: root of player visual meshes (to enable/disable when switching)
    public KeyCode toggleKey = KeyCode.V;

    bool isFirstPerson = false;

    void Start()
    {
        // ensure initial state: third person active, first person disabled
        if (Camera != null) Camera.enabled = true;
        if (FirstPersonCamera != null) FirstPersonCamera.enabled = false;
        if (fpLook != null) fpLook.enabled = false;
        // ensure player visuals are enabled in 3rd person (so body visible)
        if (playerVisuals != null) playerVisuals.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleView();
        }
    }

    public void ToggleView()
    {
        isFirstPerson = !isFirstPerson;

        if (Camera != null) Camera.enabled = !isFirstPerson;
        if (FirstPersonCamera != null) FirstPersonCamera.enabled = isFirstPerson;

        if (fpLook != null) fpLook.enabled = isFirstPerson;
        if (fpLook != null) fpLook.EnableLook(isFirstPerson);

        // Option A: disable player visuals when in first person (so you don't see model)
        if (playerVisuals != null) playerVisuals.SetActive(!isFirstPerson);

        // If you used layers instead for body hiding, you can skip disabling visuals and rely on culling mask
    }
}
