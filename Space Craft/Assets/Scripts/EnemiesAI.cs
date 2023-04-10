using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Flock;
using static UnityEngine.GraphicsBuffer;

public class EnemiesAI : MonoBehaviour
{
    public FlockingSettings settings;
    public ComputeShader compute;

    public Transform player;
    public List<EnemyAI> enemies = new();
    private List<GameObject> allies = new();    
    public EnemyAI enemyPrefab;

    const int threadGroupSize = 1024;
    private float spawnRadius = 50f;

    private void Awake()
    {
        //spawn all enemeies
        Spawn();
    }

    private void Start()
    {
        List<Boid> allies = GameObject.FindObjectsOfType<Boid>().ToList();
        foreach (Boid boid in allies)
        {
            this.allies.Add(boid.gameObject);
        }
        this.allies.Add(player.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (enemies != null)
        {
            if (enemies.Count == 0)
                return;
            int numEnemy = enemies.Count;
            var enemyData = new EnemyData[numEnemy];

            for (int i = 0; i < enemies.Count; i++)
            {
                enemyData[i].position = enemies[i].position;
                enemyData[i].direction = enemies[i].forward;
            }

            var enemyBuffer = new ComputeBuffer(numEnemy, EnemyData.Size);
            enemyBuffer.SetData(enemyData);

            compute.SetBuffer(0, "boids", enemyBuffer);
            compute.SetInt("numBoids", enemies.Count);
            compute.SetFloat("viewRadius", settings.perceptionRadius);
            compute.SetFloat("avoidRadius", settings.avoidanceRadius);

            int threadGroups = Mathf.CeilToInt(numEnemy / (float)threadGroupSize);
            compute.Dispatch(0, threadGroups, 1, 1);

            enemyBuffer.GetData(enemyData);

            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].avgFlockHeading = enemyData[i].flockHeading;
                enemies[i].centreOfFlockmates = enemyData[i].flockCentre;
                enemies[i].avgAvoidanceHeading = enemyData[i].avoidanceHeading;
                enemies[i].numPerceivedFlockmates = enemyData[i].numFlockmates;

                enemies[i].CalculateMovement();
            }

            enemyBuffer.Release();
        }
    }

    private void Spawn()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
            EnemyAI enemy = Instantiate(enemyPrefab, pos, Quaternion.identity).gameObject.GetComponent<EnemyAI>();
            enemy.transform.forward = Random.insideUnitSphere;
            enemy.Initialize(settings, player);
            enemies[i] = enemy;
        }
    }
    public void SpawnAnEnemy()
    {
        Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
        EnemyAI enemy = Instantiate(enemyPrefab, pos, Quaternion.identity).gameObject.GetComponent<EnemyAI>();
        enemy.transform.forward = Random.insideUnitSphere;
        enemy.Initialize(settings, player);
        enemies.Add(enemy);
    }
    public struct EnemyData
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
