using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidFlotation : MonoBehaviour
{
    private Vector3 flotation_dir;
    private float flotation_speed;
    private float delta_rotation_angle;
    private Vector3 rotation_axis;
    // Start is called before the first frame update
    void Start()
    {
        rotation_axis = new Vector3[] { Vector3.up, Vector3.forward, Vector3.right} [Random.Range(0, 2)];
        flotation_dir = Quaternion.AngleAxis(Random.Range(-180, 180), rotation_axis) * transform.forward;
        flotation_speed = Random.Range(1, 5);
        delta_rotation_angle = Random.Range(5, 15);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += flotation_dir * flotation_speed * Time.deltaTime;
        transform.Rotate(rotation_axis, delta_rotation_angle * Time.deltaTime);
    }

    public Vector3 getFlotationDir()
    {
        return flotation_dir;
    }

    public float getFlotationSpeed()
    {
        return flotation_speed;
    }
}
