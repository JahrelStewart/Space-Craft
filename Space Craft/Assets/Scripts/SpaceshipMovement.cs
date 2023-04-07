using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipMovement : MonoBehaviour
{

    private float torque = 1.15f;
    private float thrust = 20f;
    private Rigidbody rb;
    private float hp = 100;
    private float max_hp = 100;
    private float collision_damage = 20;
    private float rocket_damage = 40;

    public event Action<float> OnHealthChange = delegate { };

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

    public void takeDamage(float damage)
    {
        hp -= damage;
        OnHealthChange(hp / max_hp);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("Rocket"))
        {
            Debug.Log("Rocket damage");
            takeDamage(rocket_damage);
        } else if (collision.gameObject.name.Contains("Asteroid"))
        {
            Debug.Log("Asteroid damage");
            takeDamage(collision_damage);
        }
    }
}
