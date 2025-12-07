using UnityEngine;

public class MoveOrb : MonoBehaviour
{
    [Header("Movement")]
    public Vector3 targetPosition = new Vector3(4f, 1f, 4f);
    public float speed = 3f;

    [Header("Floating (vertical bob)")]
    public bool enableFloat = true;
    public float floatAmplitude = 0.25f;   // how high it bobs up/down
    public float floatFrequency = 1.5f;    // speed of bobbing (Hz)

    // internal
    private Vector3 _basePosition;   // used as offset when bobbing
    private float _floatStartY;

    void Start()
    {
        // record initial base position (used for floating offset)
        _basePosition = transform.position;
        _floatStartY = transform.position.y;
    }


    void Update()
    {
        // Move towards target in world space
        Vector3 nextPos = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // If floating is enabled, compute bob offset around a base Y.
        if (enableFloat)
        {
            // Use time-based sine for smooth bobbing
            float bob = Mathf.Sin(Time.time * Mathf.PI * 2f * floatFrequency) * floatAmplitude;

            // We want horizontal movement towards target but vertical = baseY + bob
            // Use the horizontal/forward position from nextPos but set Y separately.
            nextPos.y = _floatStartY + bob;
        }

        transform.position = nextPos;
    }

    // Helpful visual in Scene view: shows target and path
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(targetPosition, 0.12f);
        Gizmos.DrawLine(transform.position, targetPosition);
    }
}
