using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pickupable : MonoBehaviour
{
    [Header("Offsets when held (local to player)")]
    public float forwardOffset = 1.2f;
    public float upOffset = 0.2f;
    public bool setColliderTriggerWhenHeld = true;

    Rigidbody rb;
    Collider col;

    bool isHeld = false;
    Transform holdParent;

    // debug
    int debugFrames = 0;

    public float pushDistance = 0.05f; // small gentle nudge

    void OnCollisionEnter(Collision collision)
    {
        if (!isHeld && collision.gameObject.CompareTag("Player"))
        {
            Vector3 pushDir = collision.contacts[0].point - transform.position;
            pushDir.y = 0f; // only horizontal
            if (pushDir.sqrMagnitude > 0.001f)
            {
                pushDir = pushDir.normalized * pushDistance;
                rb.MovePosition(transform.position + pushDir);
            }
        }
    }
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        if (rb != null)
        {
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.useGravity = true;     // enable gravity so it sits naturally
            rb.isKinematic = false;   // let physics detect collisions
            rb.mass = 1f;
            rb.linearDamping = 5f;             // slows it down after a push
            rb.angularDamping = 5f;
            rb.linearDamping = 5f;             // slows it down after a push
            rb.angularDamping = 5f;
        }
    }

    void Update()
    {
        // While held, force the local position/rotation each Update to avoid any lag or interference
        if (isHeld && holdParent != null)
        {
            // Ensure parent relationship exists
            if (transform.parent != holdParent)
            {
                transform.SetParent(holdParent, worldPositionStays: false);
            }

            // Force exact local transform
            Vector3 desiredLocalPos = new Vector3(0f, upOffset, forwardOffset);
            transform.localPosition = desiredLocalPos;
            transform.localRotation = Quaternion.identity;

            // Safety: keep rb kinematic while held
            if (rb != null && !rb.isKinematic)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            // Debug a few frames to ensure correct state
            if (debugFrames < 4)
            {
                Debug.Log($"[Pickupable] Held: {name} parent={holdParent.name} localPos={transform.localPosition} worldPos={transform.position}");
                debugFrames++;
            }
        }
    }

    public void OnPickedUp(Transform parent)
    {
        if (isHeld || parent == null) return;

        isHeld = true;
        holdParent = parent;

        // Parent to player WITHOUT preserving world position so localPosition works predictably
        transform.SetParent(holdParent, worldPositionStays: false);

        // Snap to exact local offset immediately
        transform.localPosition = new Vector3(0f, upOffset, forwardOffset);
        transform.localRotation = Quaternion.identity;

        // turn off physics while attached
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (col != null && setColliderTriggerWhenHeld)
            col.isTrigger = true;

        debugFrames = 0;
        Debug.Log($"[Pickupable] OnPickedUp: {name} parent={parent.name}");
    }

    public void OnDropped()
    {
        if (!isHeld) return;

        // Drop in front of ghost/player
        Vector3 dropPosition = transform.position;
        if (holdParent != null)
            dropPosition = holdParent.position + holdParent.forward * 1f + Vector3.up * 0.2f; // adjust forward/up offsets

        transform.SetParent(null, true);
        transform.position = dropPosition; // ensure proper world position

        // re-enable physics
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;      // reset linear velocity
            rb.angularVelocity = Vector3.zero; // reset angular velocity
        }

        if (col != null && setColliderTriggerWhenHeld)
            col.isTrigger = false;

        isHeld = false;
        holdParent = null;

        Debug.Log($"[Pickupable] OnDropped: {name} worldPos={transform.position}");
    }
    public void RemoveFromScene()
    {
        // If held, unparent cleanly
        if (isHeld)
        {
            // put it where the holder currently is (avoid sudden transforms)
            transform.SetParent(null, true);
            isHeld = false;
            holdParent = null;
        }

        // disable physics immediately to avoid any physics visual glitches
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // disable colliders so nothing interacts with it during removal
        if (col != null) col.enabled = false;
        Collider[] childCols = GetComponentsInChildren<Collider>(true);
        foreach (var c in childCols) c.enabled = false;

        // disable renderers so it visually disappears without waiting for Destroy
        Renderer[] rends = GetComponentsInChildren<Renderer>(true);
        foreach (var r in rends) r.enabled = false;

        // finally destroy the GameObject (immediate or with tiny delay)
        Destroy(gameObject);
    }
}
