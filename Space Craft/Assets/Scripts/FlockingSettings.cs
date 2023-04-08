using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Flocking Settings", menuName = "Flocking Settings")]
public class FlockingSettings : ScriptableObject
{
    // Flock Settings that all boids must adhere to:

    public float minSpeed = 20; //2
    public  float maxSpeed = 30; //5
    public  float perceptionRadius = 50; //2.5 | Controls how big the flock groups are
    public  float avoidanceRadius = 4; //1 | Controls how far away each boid must be from their flockmates within a group
    public float maxSteerForce = 3; //3
    public  float alignWeight = 1; //1
    public  float cohesionWeight = 1; //1
    public  float seperateWeight = 1; //1

    public LayerMask obstacleMask;
    public  float targetWeight = 20; //1

    public  float boundsRadius = 1; //.27f
    public  float avoidCollisionWeight = 70; //10
    public  float collisionAvoidDst = 10; //5 | Controls the distance at whcih a boid must divert to avoid collision
}
