using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteAsteroids : MonoBehaviour
{
    public List<GameObject> asteroids;
    private GameObject[] points;
    private int asteroids_max = 600;
    private float asteroids_size = 0.5f;
    private float asteroids_distance = 450;
    private float asteroids_clip_distance = 75;
    private Vector3 initial_scale = new Vector3(1, 1, 1);

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (points == null)
        {
            points = new GameObject[asteroids_max];

            for (int i = 0; i < asteroids_max; i++)
            {
                points[i] = Instantiate(asteroids[Random.Range(0, asteroids.Count)], transform.position, transform.rotation);
                points[i].transform.position = Random.insideUnitSphere * asteroids_distance + transform.position;
                points[i].transform.Find("HPCanvas").GetComponent<Canvas>().enabled = false;
            }
        }

        for (int i = 0; i < asteroids_max; i++)
        {
            if (points[i] != null)
            {
                if ((points[i].transform.position - transform.position).sqrMagnitude > Mathf.Pow(asteroids_distance, 2))
                {
                    points[i].transform.position = Random.insideUnitSphere * asteroids_distance + transform.position;
                }

                points[i].transform.localScale = Vector3.Min(initial_scale, initial_scale * (asteroids_clip_distance / Vector3.Distance(points[i].transform.position, transform.position)));
            }
        }
    }
}
