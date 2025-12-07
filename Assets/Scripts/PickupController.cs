using System.Collections;
using UnityEngine;

public class PickupController : MonoBehaviour
{
    public Camera playerCamera;           // optional; used for aiming/raycast
    public float pickupRange = 3f;
    public LayerMask pickupLayer = ~0;    // set to your Pickupable layer
    public float defaultHoldDuration = 5f;

    [HideInInspector] public float holdTimeRemaining = 0f;
    [HideInInspector] public float holdTimeMax = 0f;
    [HideInInspector] public bool canCarry = false;

    public HoldBarUI holdBarUI; // drag the UI Manager or the GameObject that has HoldBarUI_LeanTween


    Pickupable heldItem;
    Coroutine holdTimerCoroutine;

    void Start()
    {
        if (playerCamera == null) playerCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldItem == null)
                TryPickup();
            else
                ManualDrop();
        }
    }

    void TryPickup()
    {
        if (!canCarry) return; // <-- Only pick up if powerup is active

        if (playerCamera == null)
        {
            Debug.LogWarning("PickupController: playerCamera is null.");
            return;
        }

        // Raycast from camera center
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        RaycastHit hit;
        if (Physics.SphereCast(ray, 0.5f, out hit, pickupRange, pickupLayer))
        {
            Pickupable p = hit.collider.GetComponent<Pickupable>();
            if (p != null)
            {
                Pickup(p);
                return;
            }
        }

        // fallback: nearest overlap in radius
        Collider[] cols = Physics.OverlapSphere(transform.position, pickupRange, pickupLayer);
        float best = Mathf.Infinity;
        Pickupable bestP = null;
        foreach (Collider c in cols)
        {
            Pickupable p = c.GetComponent<Pickupable>();
            if (p == null) continue;
            float d = Vector3.Distance(transform.position, p.transform.position);
            if (d < best) { best = d; bestP = p; }
        }
        if (bestP != null) Pickup(bestP);
    }

    void Pickup(Pickupable p)
    {
        if (p == null) return;

        heldItem = p;
        // pass this player's transform as the parent
        heldItem.OnPickedUp(this.transform);

        // start timer and expose values for UI
        holdTimeMax = defaultHoldDuration;
        holdTimeRemaining = defaultHoldDuration;

        if (holdBarUI != null)
            holdBarUI.StartHold(defaultHoldDuration);
    }

    IEnumerator HoldTimer(float duration)
    {
        float t = 0f;
        // set initial remain for UI
        holdTimeRemaining = duration;
        while (t < duration)
        {
            t += Time.deltaTime;
            holdTimeRemaining = Mathf.Max(0f, duration - t); // <-- important: update exposed value
            yield return null;
        }
        // ensure 0
        holdTimeRemaining = 0f;
        AutoDrop();
    }

    void AutoDrop()
    {
        if (heldItem != null)
        {
            heldItem.OnDropped();
            heldItem = null;
        }
        if (holdBarUI != null)
            holdBarUI.StopHold();

    }

    public void ManualDrop()
    {
        if (heldItem != null)
        {
            heldItem.OnDropped();
            heldItem = null;
        }
        if (holdBarUI != null)
            holdBarUI.StopHold();
    }
}
