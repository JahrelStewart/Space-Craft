using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipMovement : MonoBehaviour
{

    public float torque = 10f; //1.15f
    public float thrust = 30f;
    private Rigidbody rb;
    private float collision_damage = 10;
    private float rocket_damage = 20;
    private float laser_damage = 30;

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

        rb.AddRelativeTorque((Input.GetAxis("Vertical")) * torque, 0, 0);
        rb.AddRelativeTorque(0, (Input.GetAxis("Horizontal")) * torque, 0);
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("Asteroid"))
        {
            //Debug.Log("Asteroid damage");
            GetComponent<HealthPoints>().takeDamage(collision_damage);
        }

    }
}
