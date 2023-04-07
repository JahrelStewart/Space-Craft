using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipAttack : MonoBehaviour
{
    public GameObject rocket_prefab;
    public ParticleSystem explosion_system_prefab;
    public AISensor sensor;
    public GameObject spaceship;
    public float speed = 1f;
    public Canvas crosshair;
    private float rocket_launch_time = 1.5f;
    private float rocket_launch_timer = 1.5f;
    private GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        crosshair.enabled = false;
    }
    private GameObject getClosestTarget()
    {
        float min_distance = float.MaxValue;
        GameObject closest_target = null;

        foreach(GameObject g in sensor.Objects)
        {
            float dist = Vector3.Distance(spaceship.transform.position, g.transform.position);
            if (dist < min_distance)
            {
                min_distance = dist;
                closest_target = g;
            }
        }

        return closest_target;
    }

    // Update is called once per frame
    void Update()
    {
        if(sensor.Objects.Count == 0)
        {
            crosshair.enabled = false;
        }
        else
        {
            crosshair.enabled = true;
            crosshair.GetComponent<RectTransform>().anchoredPosition3D = Camera.main.WorldToScreenPoint(getClosestTarget().transform.position);
        }
        if (Input.GetButtonDown("Fire1") && sensor.Objects.Count > 0)
        {
            rocket_launch_timer = 0;
            GameObject rocket = Instantiate(rocket_prefab, spaceship.transform.position + transform.forward * 3, rocket_prefab.transform.rotation);
            rocket.transform.localScale /= 3;
            target = getClosestTarget();
            rocket.transform.LookAt(target.transform);
            StartCoroutine(LaunchRocket(rocket));
        }
    }
    private IEnumerator LaunchRocket(GameObject rocket)
    {
        while (Vector3.Distance(target.transform.position, rocket.transform.position) > 1f)
        {
            float real_speed = speed;
            if (Vector3.Distance(target.transform.position, rocket.transform.position) < 8f)
                real_speed *= 1.5f;
            rocket.transform.position += (target.transform.position - rocket.transform.position) * real_speed * Time.deltaTime;
            rocket.transform.LookAt(target.transform);
            yield return null;
        }
        if (target.gameObject.GetComponent<AsteroidCollisionExplosion>())
        {
            target.gameObject.GetComponent<AsteroidCollisionExplosion>().explode();
        }
        ParticleSystem exp = Instantiate(explosion_system_prefab, rocket.transform.position, explosion_system_prefab.transform.rotation); ;
        exp.Play();
        Destroy(rocket);
        Destroy(exp, exp.main.duration);
    }
}
