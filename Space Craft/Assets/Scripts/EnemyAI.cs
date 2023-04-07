using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public GameObject player;
    public AISensor sensor;
    public GameObject rocket_prefab;
    public ParticleSystem explosion_system_prefab;
    private float see_radius = 125f;
    private float attack_radius = 55f;
    private float arrive_radius = 10f;
    private float torque = 1.15f;
    private float thrust = 20f;
    private Rigidbody rb;
    private Vector3 destination;
    private float max_velocity = 10f;
    private float timer = 6f;
    private float time = 0;
    private float rocket_launch_time = 1.5f;
    private float rocket_launch_timer = 1.5f;
    private Vector3 target;
    private float rocket_speed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        destination = Vector3.zero;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if(destination == Vector3.zero || time > timer || sensor.Objects.Count > 0)
        {
            time = 0;
            float dist = Vector3.Distance(transform.position, player.transform.position);
            //Debug.Log("Distance between player and enemy is: " + dist);
            if (dist > see_radius)
                Wander();
            else if (dist < attack_radius && sensor.Objects.Count > 0)
                Attack();
            else
                Seek();
        }
        else
        {
            if (rb.velocity.magnitude < max_velocity)
            {
                rb.AddRelativeForce((-destination + transform.position).normalized * thrust);
            }
        }
        //Debug.Log(destination);
        //Debug.DrawRay(transform.position, transform.TransformDirection(destination - transform.position), Color.black);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation((-destination + transform.position).normalized), 0.003f);

    }

    public void Wander()
    {
        rb.velocity *= 0.3f;
        rb.angularVelocity *= 0.3f;
        Debug.Log("Wandering");
        Vector3 rotated_fwd = Quaternion.AngleAxis(Random.Range(-180, 180), Vector3.up) * transform.forward;
        rotated_fwd = Quaternion.AngleAxis(Random.Range(-180, 180), Vector3.forward) * transform.forward;
        rotated_fwd = Quaternion.AngleAxis(Random.Range(-180, 180), Vector3.right) * transform.forward;
        rotated_fwd.Normalize();

        destination = transform.position + rotated_fwd * 15f;
        //Debug.Log("Destination pos is: " + destination);
    }

    public void Attack()
    {
        Debug.Log("Attacking");
        Debug.DrawRay(transform.position + 0.5f * transform.forward, sensor.Objects[0].transform.position - transform.position);
        rocket_launch_timer = 0;
        GameObject rocket = Instantiate(rocket_prefab, transform.position, rocket_prefab.transform.rotation);
        rocket.transform.localScale /= 3;
        target = sensor.Objects[0].transform.position;
        rocket.transform.LookAt(target);

        rb.velocity *= 0.3f;
        rb.angularVelocity *= 0.3f;
        Vector3 rotated_fwd = Quaternion.AngleAxis(Random.Range(-25, 25), Vector3.up) * transform.forward;
        rotated_fwd = Quaternion.AngleAxis(Random.Range(-25, 25), Vector3.forward) * transform.forward;
        rotated_fwd = Quaternion.AngleAxis(Random.Range(-25, 25), Vector3.right) * transform.forward;
        rotated_fwd.Normalize();
        destination = transform.position + rotated_fwd * 15f;

        StartCoroutine(LaunchRocket(rocket));
    }
    private IEnumerator LaunchRocket(GameObject rocket)
    {
        while (Vector3.Distance(target, rocket.transform.position) > 1f)
        {
            float real_speed = rocket_speed;
            if (Vector3.Distance(target, rocket.transform.position) < 8f)
                real_speed *= 1.5f;
            rocket.transform.position += (target - rocket.transform.position) * real_speed * Time.deltaTime;
            rocket.transform.LookAt(target);
            yield return null;
        }
        // Deal some damage to target
        ParticleSystem exp = Instantiate(explosion_system_prefab, rocket.transform.position, explosion_system_prefab.transform.rotation); ;
        exp.Play();
        Destroy(rocket);
        Destroy(exp, exp.main.duration);
    }

    public void Seek()
    {
        Debug.Log("Seeking");
        rb.velocity *= 0.3f;
        rb.angularVelocity *= 0.3f;
        destination = player.gameObject.transform.position;
    }
}