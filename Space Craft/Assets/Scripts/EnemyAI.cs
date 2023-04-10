using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyAI : MonoBehaviour
{
    FlockingSettings settings;
    public GameObject rocket_prefab;
    public ParticleSystem explosion_system_prefab;
    private float rotation_speed = 50.0f;
    private float attack_radius = 120f;      
    private float timer = 10f;
    private float time = 0;    
    private float rocket_speed = 5f;
    private float rocket_damage = 2;
    private float laser_damage = 30;
    private Vector3 randomSpot;
    private int offset = 80; // origin of radius which is 50 away from target position
    private int radius = 50; // Enemy will move anywhere within the given radius of the offset position
    private float explosion_radius = 5f;

    // To update:    
    [HideInInspector]
    public Vector3 avgFlockHeading;
    [HideInInspector]
    public Vector3 avgAvoidanceHeading;
    [HideInInspector]
    public Vector3 centreOfFlockmates;
    [HideInInspector]
    public int numPerceivedFlockmates;

    //For Obstacle Avoidance:
    private const int numViewDirections = 300;
    private Vector3[] directions;

    private Transform cachedTransform;
    private Transform target; //target that follows player    
    [HideInInspector]
    public Vector3 position;
    [HideInInspector]
    public Vector3 forward;
    private Vector3 velocity;

    void Awake()
    {
        cachedTransform = transform;
    }

    // Start is called before the first frame update
    void Start()
    {        
        randomSpot = Random.insideUnitSphere;
        GeneratePerceptionPoints();
    }

    public Transform getClosesTarget()
    {
        float min_distance = float.MaxValue;
        GameObject closest_target = null;

        foreach (GameObject g in GameObject.Find("EnemiesSystem").GetComponent<EnemiesAI>().allies)
        {
            //Debug.Log(g.name);
            if (g != null)
            {
                float dist = Vector3.Distance(transform.position, g.transform.position);
                if (dist < min_distance)
                {
                    min_distance = dist;
                    closest_target = g;
                }
            }
        }

        return closest_target.transform;
    }

    public void Initialize(FlockingSettings settings, Transform player)
    {
        this.target = player;
        this.settings = settings;

        position = cachedTransform.position;
        forward = cachedTransform.forward;

        float startSpeed = (settings.minSpeed + settings.maxSpeed) / 2;
        velocity = transform.forward * startSpeed;
    }

    // Update is called once per frame
    public void CalculateMovement()
    {
        time += Time.deltaTime;
        target = getClosesTarget();

        float distance = Vector3.Distance(transform.position, target.transform.position);
        Quaternion dirToTarget = Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up);

        if (time > timer && distance < attack_radius)
        {
            time = 0;
            Attack();
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation, dirToTarget, rotation_speed * Time.deltaTime);        

        Vector3 acceleration = Vector3.zero;        

        if (target != null)
        {
            float aheadBy = (int)(distance / 10) + offset;
            Vector3 futurePositionOfTarget = target.transform.position + (randomSpot * radius) + target.transform.forward * aheadBy;
            Vector3 offsetToTarget = (futurePositionOfTarget - position);
            acceleration = SteerTowards(offsetToTarget) * settings.targetWeight;
        }

        if (numPerceivedFlockmates != 0)
        {
            centreOfFlockmates /= numPerceivedFlockmates;

            Vector3 offsetToFlockmatesCentre = (centreOfFlockmates - position);

            var alignmentForce = SteerTowards(avgFlockHeading) * settings.alignWeight;
            var cohesionForce = SteerTowards(offsetToFlockmatesCentre) * settings.cohesionWeight;
            var seperationForce = SteerTowards(avgAvoidanceHeading) * settings.seperateWeight;

            acceleration += alignmentForce;
            acceleration += cohesionForce;
            acceleration += seperationForce;
        }

        if (IsGoingToCollide())
        {
            Vector3 collisionAvoidDir = GetWhiskers();
            Vector3 collisionAvoidForce = SteerTowards(collisionAvoidDir) * settings.avoidCollisionWeight;
            acceleration += collisionAvoidForce;
        }

        velocity += acceleration * Time.deltaTime;
        float speed = velocity.magnitude;
        Vector3 dir = velocity / speed;
        speed = Mathf.Clamp(speed, settings.minSpeed, settings.maxSpeed);
        velocity = dir * speed;        

        cachedTransform.position += velocity * Time.deltaTime;
        cachedTransform.forward = transform.forward;
        position = cachedTransform.position;
        forward = transform.forward;

    }

    public void Attack()
    {
        //Debug.Log("Attacking");
        //Debug.DrawRay(transform.position + 0.5f * transform.forward, target.transform.position - transform.position);        
        GameObject rocket = Instantiate(rocket_prefab, transform.position, rocket_prefab.transform.rotation);
        rocket.transform.localScale /= 3;
        rocket.transform.LookAt(target.transform.position);

        StartCoroutine(LaunchRocket(rocket, target.transform.position));
    }
    private IEnumerator LaunchRocket(GameObject rocket, Vector3 target)
    {
        while (Vector3.Distance(target, rocket.transform.position) > 3f)
        {
            float real_speed = rocket_speed;
            if (Vector3.Distance(target, rocket.transform.position) < 8f)
                real_speed *= 1.5f;
            rocket.transform.position += (target - rocket.transform.position) * real_speed * Time.deltaTime;
            rocket.transform.LookAt(target);
            yield return null;
        }
        // Deal some damage to target
        Collider[] colliders = Physics.OverlapSphere(rocket.transform.position, explosion_radius);
        foreach(Collider col in colliders)
        {
            if(col.transform.parent != null)
                col.transform.parent.GetComponent<HealthPoints>().takeDamage(rocket_damage);
            else if(col.transform.GetComponent<HealthPoints>() != null)
                col.transform.GetComponent<HealthPoints>().takeDamage(rocket_damage);
        }
/*        if(Vector3.Distance(rocket.transform.position, GameObject.Find("Spaceship").transform.Find("Cylinder").transform.position) < explosion_radius){
            GameObject.Find("Spaceship").GetComponent<HealthPoints>().takeDamage(rocket_damage);
        }*/
        ParticleSystem exp = Instantiate(explosion_system_prefab, rocket.transform.position, explosion_system_prefab.transform.rotation);
        exp.Play();
        Destroy(rocket);
        Destroy(exp, exp.main.duration);
    }

    private void GeneratePerceptionPoints()
    {
        directions = new Vector3[numViewDirections];

        float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
        float angleIncrement = Mathf.PI * 2 * goldenRatio;

        for (int i = 0; i < numViewDirections; i++)
        {
            float t = (float)i / numViewDirections;
            float inclination = Mathf.Acos(1 - 2 * t);
            float azimuth = angleIncrement * i;

            float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
            float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
            float z = Mathf.Cos(inclination);
            directions[i] = new Vector3(x, y, z);
        }
    }

    bool IsGoingToCollide()
    {
        Vector3[] rayDirections = directions;

        for (int i = 0; i < rayDirections.Length; i++)
        {
            Vector3 dir = cachedTransform.TransformDirection(rayDirections[i]);

            RaycastHit hit;
            if (Physics.SphereCast(position, settings.boundsRadius, dir, out hit, settings.collisionAvoidDst, settings.obstacleMask))
            {
                return true;
            }
        }

        return false;
    }

    Vector3 GetWhiskers()
    {
        Vector3[] rayDirections = directions;

        for (int i = 0; i < rayDirections.Length; i++)
        {
            Vector3 dir = cachedTransform.TransformDirection(rayDirections[i]);
            Ray ray = new Ray(position, dir);
            if (!Physics.SphereCast(ray, settings.boundsRadius, settings.collisionAvoidDst, settings.obstacleMask))
            {
                return dir;
            }
        }

        return forward;
    }
    Vector3 SteerTowards(Vector3 vector)
    {
        Vector3 v = vector.normalized * settings.maxSpeed - velocity;
        return Vector3.ClampMagnitude(v, settings.maxSteerForce);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.name.Contains("Rocket"))
        {
            Debug.Log("Rocket damage From Player");
            GetComponent<HealthPoints>().takeDamage(rocket_damage);
        }
        else if (collider.gameObject.name.Contains("Laser"))
        {
            Debug.Log("Laser damage From Player");
            GetComponent<HealthPoints>().takeDamage(laser_damage);
        }
    }

    private void OnDrawGizmos()
    {
        /* Gizmos.color = Color.magenta;
         Gizmos.DrawLine(transform.position, player.transform.position);*/

        /*if (directions.Length > 0)
        {
            for (int i = 0; i < directions.Length; i++)
            {
                Vector3 dir = transform.TransformDirection(directions[i]);
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(transform.position, dir * settings.collisionAvoidDst);
            }
        }*/
    }
}