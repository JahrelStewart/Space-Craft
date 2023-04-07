using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{     
    private float spawnRadius = 10f;

    public Boid boidPrefab;
    public List<Boid> boids = new();

    private void Awake()
    {
        //spawn all boids
        Spawn();

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Spawn()
    {
        for (int i = 0; i < boids.Count; i++)
        {
            Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
            Boid boid = Instantiate(boidPrefab);
            boid.transform.position = pos;
            boid.transform.forward = Random.insideUnitSphere;            
        }
    }

    // Flock Settings that all boids mist adhere to:
    public struct BoidSettings
    {
        public static float minSpeed = 2;
        public static float maxSpeed = 5;
        public static float perceptionRadius = 2.5f;
        public static float avoidanceRadius = 1;
        public static float maxSteerForce = 3;

        public static float alignWeight = 1;
        public static float cohesionWeight = 1;
        public static float seperateWeight = 1;

        public static float targetWeight = 1;

        public static LayerMask obstacleMask;
        public static float boundsRadius = .27f;
        public static float avoidCollisionWeight = 10;
        public static float collisionAvoidDst = 5;
    }

    public struct BoidData
    {
        public Vector3 position;
        public Vector3 direction;

        public Vector3 flockHeading;
        public Vector3 flockCentre;
        public Vector3 avoidanceHeading;
        public int numFlockmates;

        public static int Size
        {
            get
            {
                return sizeof(float) * 3 * 5 + sizeof(int);
            }
        }
    }
}
