using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

public class Boid : MonoBehaviour
{
    private Transform target; //target that follows player
    private Vector3 position;    
    private Vector3 forward;
    Vector3 velocity;

    // To update:
    Vector3 acceleration;
    [HideInInspector]
    public Vector3 avgFlockHeading;
    [HideInInspector]
    public Vector3 avgAvoidanceHeading;
    [HideInInspector]
    public Vector3 centreOfFlockmates;
    [HideInInspector]
    public int numPerceivedFlockmates;

    private const int numViewDirections = 300;
    private Vector3[] directions;

    // Start is called before the first frame update
    void Start()
    {
        GeneratePerceptionPoints();        
    }

    public void Initialize(Transform target)
    {
        this.target = target;       

        /*position = cachedTransform.position;
        forward = cachedTransform.forward;*/

        float startSpeed = (Flock.BoidSettings.minSpeed + Flock.BoidSettings.maxSpeed) / 2;
        velocity = transform.forward * startSpeed;
    }

    public void CalculateMovement()
    {
        Vector3 acceleration = Vector3.zero;

        if (target != null)
        {
            Vector3 offsetToTarget = (target.position - position);
            acceleration = SteerTowards(offsetToTarget) * Flock.BoidSettings.targetWeight;
        }

        if (numPerceivedFlockmates != 0)
        {
            centreOfFlockmates /= numPerceivedFlockmates;

            Vector3 offsetToFlockmatesCentre = (centreOfFlockmates - position);

            var alignmentForce = SteerTowards(avgFlockHeading) * Flock.BoidSettings.alignWeight;
            var cohesionForce = SteerTowards(offsetToFlockmatesCentre) * Flock.BoidSettings.cohesionWeight;
            var seperationForce = SteerTowards(avgAvoidanceHeading) * Flock.BoidSettings.seperateWeight;

            acceleration += alignmentForce;
            acceleration += cohesionForce;
            acceleration += seperationForce;
        }

        if (IsGoingToCollide())
        {
            Vector3 collisionAvoidDir = GetWhiskers();
            Vector3 collisionAvoidForce = SteerTowards(collisionAvoidDir) * Flock.BoidSettings.avoidCollisionWeight;
            acceleration += collisionAvoidForce;
        }

        velocity += acceleration * Time.deltaTime;
        float speed = velocity.magnitude;
        Vector3 dir = velocity / speed;
        speed = Mathf.Clamp(speed, Flock.BoidSettings.minSpeed, Flock.BoidSettings.maxSpeed);
        velocity = dir * speed;

        transform.position += velocity * Time.deltaTime;

        /*cachedTransform.position += velocity * Time.deltaTime;
        cachedTransform.forward = dir;
        position = cachedTransform.position;
        forward = dir;*/

    }

    bool IsGoingToCollide()
    {
        RaycastHit hit;
        if (Physics.SphereCast(position, Flock.BoidSettings.boundsRadius, forward, out hit, Flock.BoidSettings.collisionAvoidDst, Flock.BoidSettings.obstacleMask))
        {
            return true;
        }
        
        return false;
    }

    Vector3 GetWhiskers()
    {
        Vector3[] rayDirections = directions;

        for (int i = 0; i < rayDirections.Length; i++)
        {
            //Vector3 dir = cachedTransform.TransformDirection(rayDirections[i]);
            Vector3 dir = transform.TransformDirection(rayDirections[i]);
            Ray ray = new Ray(position, dir);
            if (!Physics.SphereCast(ray, Flock.BoidSettings.boundsRadius, Flock.BoidSettings.collisionAvoidDst, Flock.BoidSettings.obstacleMask))
            {
                return dir;
            }
        }

        return forward;
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

    Vector3 SteerTowards(Vector3 vector)
    {
        Vector3 v = vector.normalized * Flock.BoidSettings.maxSpeed - velocity;
        return Vector3.ClampMagnitude(v, Flock.BoidSettings.maxSteerForce);
    }
}
