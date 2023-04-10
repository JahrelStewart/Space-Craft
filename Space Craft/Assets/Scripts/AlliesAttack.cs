using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlliesAttack : MonoBehaviour
{
    private AISensor lead_sensor;
    private AISensor this_sensor;
    public GameObject rocket_prefab;
    public GameObject laserbeam_prefab;
    private GameObject target;
    private float rocket_launch_time = 5.5f;
    private float rocket_launch_timer = 5.5f;
    private float laser_launch_time = 5.5f;
    private float laser_launch_timer = 5.5f;
    private float laserbeam_lifetime = 1.6f;
    private float missile_speed = 5f;
    private float laserbeam_speed = 45f;
    private float laser_distance_threshold = 80f;
    public ParticleSystem explosion_system_prefab;
    private float explosion_radius = 5f;
    private float rocket_damage = 25;

    // Start is called before the first frame update
    void Start()
    {
        lead_sensor = GameObject.Find("Spaceship").transform.Find("Sphere").GetComponent<AISensor>();
        this_sensor = GetComponent<AISensor>();
    }

    // Update is called once per frame
    void Update()
    {
        rocket_launch_time += Time.deltaTime;
        laser_launch_time += Time.deltaTime;

        if (this_sensor.Objects.Count > 0)
        {
            GameObject closest_target = getClosestTarget(this_sensor);
            if (closest_target != null)
            {
                if (closest_target.transform.GetComponent<EnemyAI>() != null)
                {
                    //Debug.Log(Vector3.Distance(transform.position, closest_target.transform.position));
                    // If the distance is out of laser's reach, shoot an auto missile
                    if (Vector3.Distance(transform.position, closest_target.transform.position) > laser_distance_threshold)
                    {
                        if (rocket_launch_time >= rocket_launch_timer)
                        {
                            rocket_launch_time = 0;
                            InstantiateAndLaunchRocket(closest_target);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        // If target is within laser's range but can be killed with a rocket, shoot an auto missile
                        // since it has 100% accuracy
                        if (rocket_launch_time >= rocket_launch_timer && closest_target.GetComponent<HealthPoints>().hp < 20)
                        {
                            rocket_launch_time = 0;
                            InstantiateAndLaunchRocket(closest_target);
                        }
                        else
                        {
                            if (laser_launch_time > laser_launch_timer)
                            {
                                laser_launch_time = 0;
                                StartCoroutine(LaunchNLasers(7));
                            }
                        }
                    }
                } else if (closest_target.name.Contains("Asteroid")) {
                    if (Vector3.Distance(transform.position, closest_target.transform.position) > laser_distance_threshold)
                    {
                        if (rocket_launch_time >= rocket_launch_timer)
                        {
                            rocket_launch_time = 0;
                            InstantiateAndLaunchRocket(closest_target);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        // If target is within laser's range but can be killed with a rocket, shoot an auto missile
                        // since it has 100% accuracy
                        if (rocket_launch_time >= rocket_launch_timer && closest_target.GetComponent<HealthPoints>().hp < 20)
                        {
                            rocket_launch_time = 0;
                            InstantiateAndLaunchRocket(closest_target);
                        }
                        else
                        {
                            if (laser_launch_time > laser_launch_timer)
                            {
                                laser_launch_time = 0;
                                StartCoroutine(LaunchNLasers(7));
                            }
                        }
                    }
                }
            }
            return;
        }

        if (lead_sensor.Objects.Count > 0)
        {
            GameObject closest_target = getClosestTarget(lead_sensor);
            if (closest_target != null)
            {
                if (closest_target.transform.GetComponent<EnemyAI>() != null)
                {
                    //Debug.Log(Vector3.Distance(transform.position, closest_target.transform.position));
                    // If the distance is out of laser's reach, shoot an auto missile
                    if (Vector3.Distance(transform.position, closest_target.transform.position) > laser_distance_threshold) {
                        if (rocket_launch_time >= rocket_launch_timer)
                        {
                            rocket_launch_time = 0;
                            InstantiateAndLaunchRocket(closest_target);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        // If target is within laser's range but can be killed with a rocket, shoot an auto missile
                        // since it has 100% accuracy
                        if (rocket_launch_time >= rocket_launch_timer && closest_target.GetComponent<HealthPoints>().hp < 20 * GameManager.getNumOfAllies())
                        {
                            rocket_launch_time = 0;
                            InstantiateAndLaunchRocket(closest_target);
                        }
                    }
                }
            }
        }
    }

    private IEnumerator LaunchNLasers(int n)
    {
        int counter = 0;
        while(counter < n)
        {
            counter++;
            InstantiateAndLaunchLaser();
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void InstantiateAndLaunchRocket(GameObject closest_target)
    {
        GameObject rocket = Instantiate(rocket_prefab, transform.position + transform.forward * 3, rocket_prefab.transform.rotation);
        rocket.transform.localScale /= 3;
        target = closest_target;
        rocket.transform.LookAt(target.transform);
        StartCoroutine(LaunchRocket(rocket));
    }

    public void InstantiateAndLaunchLaser()
    {
        GameObject laserbeam_left = Instantiate(laserbeam_prefab, transform.position + transform.forward * 3 - transform.right * 2.2f, laserbeam_prefab.transform.rotation);
        GameObject laserbeam_right = Instantiate(laserbeam_prefab, transform.position + transform.forward * 3 + transform.right * 2.2f, laserbeam_prefab.transform.rotation);
        laserbeam_left.transform.forward = Quaternion.AngleAxis(-1, transform.right) * Quaternion.AngleAxis(4f, transform.up) * transform.forward;
        laserbeam_right.transform.forward = Quaternion.AngleAxis(-1, transform.right) * Quaternion.AngleAxis(-3f, transform.up) * transform.forward;
        StartCoroutine(LaunchLaserBeam(laserbeam_left));
        StartCoroutine(LaunchLaserBeam(laserbeam_right));
    }

    private GameObject getClosestTarget(AISensor sensor)
    {
        float min_distance = float.MaxValue;
        GameObject closest_target = null;

        foreach (GameObject g in sensor.Objects)
        {
            if(g != null)
            {
                float dist = Vector3.Distance(transform.position, g.transform.position);
                if (dist < min_distance)
                {
                    min_distance = dist;
                    closest_target = g;
                }
            }
        }

        return closest_target;
    }

    private IEnumerator LaunchRocket(GameObject rocket)
    {
        while (target != null && rocket != null && Vector3.Distance(target.transform.position, rocket.transform.position) > 3f)
        {
            float real_speed = missile_speed;
            if (Vector3.Distance(target.transform.position, rocket.transform.position) < 8f)
                real_speed *= 1.5f;
            rocket.transform.position += (target.transform.position - rocket.transform.position) * real_speed * Time.deltaTime;
            rocket.transform.LookAt(target.transform);
            yield return null;
        }
        if (rocket == null)
            yield break;

        Collider[] colliders = Physics.OverlapSphere(rocket.transform.position, explosion_radius);
        foreach (Collider col in colliders)
        {
            if (col.transform.parent != null && col.transform.parent.name.Contains("Spaceship"))
                continue;
            else if (col.transform.name.Contains("Asteroid"))
                continue;
            else if (col.transform.name.Contains("BlueBull"))
                continue;

            if (col.transform.GetComponent<HealthPoints>() != null)
            {
                col.transform.GetComponent<HealthPoints>().takeDamage(rocket_damage);
                Debug.Log(col.name + " HP IS NOW " + col.transform.GetComponent<HealthPoints>().hp);
            }
        }

        ParticleSystem exp = Instantiate(explosion_system_prefab, rocket.transform.position, explosion_system_prefab.transform.rotation); ;
        exp.Play();
        Destroy(rocket);
        Destroy(exp, exp.main.duration);
    }
    private IEnumerator LaunchLaserBeam(GameObject laserbeam)
    {
        float elapsed = 0;
        while (elapsed < laserbeam_lifetime)
        {
            if (laserbeam == null)
                break;
            elapsed += Time.deltaTime;
            laserbeam.transform.position += laserbeam.transform.forward * laserbeam_speed * Time.deltaTime;
            //laserbeam.transform.LookAt(target.transform);
            yield return null;
        }
        //ParticleSystem exp = Instantiate(explosion_system_prefab, rocket.transform.position, explosion_system_prefab.transform.rotation); ;
        //exp.Play();
        Destroy(laserbeam);
        //Destroy(exp, exp.main.duration);
    }
}
