using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipMovement : MonoBehaviour
{

    private float torque = 10f;
    private float thrust = 10f;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        if (Input.GetKey("space"))
        {
            rb.AddRelativeForce(Vector3.forward * thrust);
        }

        rb.AddRelativeTorque((Input.GetAxis("Vertical")) * 3.15f, 0, 0);
        rb.AddRelativeTorque(0, (Input.GetAxis("Horizontal")) * 1.15f, 0);
    }
}
