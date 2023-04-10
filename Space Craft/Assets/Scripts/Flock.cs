using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public FlockingSettings settings;

    public ComputeShader compute;
    public Transform target;
    public Boid boidPrefab;
    public List<Boid> boids = new();

    const int threadGroupSize = 1024;
    private float spawnRadius = 25f; //10f

    private void Awake()
    {
        //spawn all boids
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        if (boids != null)
        {
            if (boids.Count == 0)
                return;
            int numBoids = boids.Count;
            var boidData = new BoidData[numBoids];

            for (int i = 0; i < boids.Count; i++)
            {
                boidData[i].position = boids[i].position;
                boidData[i].direction = boids[i].forward;
            }

            var boidBuffer = new ComputeBuffer(numBoids, BoidData.Size);
            boidBuffer.SetData(boidData);

            compute.SetBuffer(0, "boids", boidBuffer);
            compute.SetInt("numBoids", boids.Count);
            compute.SetFloat("viewRadius", settings.perceptionRadius);
            compute.SetFloat("avoidRadius", settings.avoidanceRadius);

            int threadGroups = Mathf.CeilToInt(numBoids / (float)threadGroupSize);
            compute.Dispatch(0, threadGroups, 1, 1);

            boidBuffer.GetData(boidData);

            for (int i = 0; i < boids.Count; i++)
            {
                boids[i].avgFlockHeading = boidData[i].flockHeading;
                boids[i].centreOfFlockmates = boidData[i].flockCentre;
                boids[i].avgAvoidanceHeading = boidData[i].avoidanceHeading;
                boids[i].numPerceivedFlockmates = boidData[i].numFlockmates;

                boids[i].CalculateMovement();
            }

            boidBuffer.Release();
        }
    }

    private void Spawn()
    {
        for (int i = 0; i < boids.Count; i++)
        {
            Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
            Boid boid = Instantiate(boidPrefab, pos, Quaternion.identity).gameObject.GetComponent<Boid>();
            boid.transform.forward = Random.insideUnitSphere;
            boid.Initialize(settings, target);
            boids[i] = boid;
        }
    }
    public void SpawnAnAllie()
    {
        Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
        Boid boid = Instantiate(boidPrefab, pos, Quaternion.identity).gameObject.GetComponent<Boid>();
        boid.transform.forward = Random.insideUnitSphere;
        boid.Initialize(settings, target);
        boids.Add(boid);
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
