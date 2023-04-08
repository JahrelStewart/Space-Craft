using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteStars : MonoBehaviour
{
    private ParticleSystem.Particle[] points;
    private int stars_max = 1000;
    private float star_size = 0.5f;
    private float star_distance = 100;
    private float star_clip_distance = 25;
    private ParticleSystem particle_system;

    // Start is called before the first frame update
    void Start()
    {
        particle_system = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (points == null)
        {
            points = new ParticleSystem.Particle[stars_max];

            for (int i = 0; i < stars_max; i++)
            {
                points[i].position = Random.insideUnitSphere * star_distance + transform.position;
                points[i].startColor = new Color(1, 1, 1, 1);
                points[i].startSize = star_size;
            }
        }

        for(int i = 0; i < stars_max; i++)
        {
            if((points[i].position - transform.position).sqrMagnitude > Mathf.Pow(star_distance, 2))
            {
                points[i].position = Random.insideUnitSphere * star_distance + transform.position;
            }
            if((points[i].position - transform.position).sqrMagnitude <= Mathf.Pow(star_clip_distance, 2))
            {
                float dist_percentage = (points[i].position - transform.position).sqrMagnitude / Mathf.Pow(star_clip_distance, 2);
                points[i].startColor = new Color(1, 1, 1, dist_percentage);
                points[i].startSize = star_size * dist_percentage;
            }
        }

        particle_system.SetParticles(points);
    }
}
