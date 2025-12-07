using UnityEngine;

public class move : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal"); // left/right arrows
        float vertical = Input.GetAxis("Vertical");     // up/down arrows

        // Movement based on ghost's facing direction
        Vector3 move = new Vector3(horizontal, 0, vertical);

        // Convert to local movement (relative to ghost’s rotation)
        move = transform.TransformDirection(move);

        transform.position += move * speed * Time.deltaTime;
    }
}
