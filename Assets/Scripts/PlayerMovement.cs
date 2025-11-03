using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 10f;
    Rigidbody rb;
    Vector3 input;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.None; // allow rotation
        rb.maxAngularVelocity = 20f; // allow fast spinning
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        input = new Vector3(h, 0f, v);
    }

    void FixedUpdate()
    {
        // Stop small leftover spin for sharper control
        if (input.magnitude < 0.1f)
        {
            rb.angularVelocity *= 0.95f; // gradually slow when no input
            return;
        }

        // Reduce old spin when changing direction (makes turning faster)
        rb.angularVelocity *= 0.8f;

        // Apply torque for rolling movement
        Vector3 torque = new Vector3(input.z, 0, -input.x) * speed;
        rb.AddTorque(torque, ForceMode.Force);
    }
}