using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpaceshipAttack : MonoBehaviour
{
    public GameObject rocket_prefab;
    public GameObject laserbeam_prefab;
    public ParticleSystem explosion_system_prefab;
    public AISensor sensor;
    public GameObject spaceship;
    private float missile_speed = 5f;
    private float laserbeam_speed = 225f;
    public Canvas missile_crosshair;
    public Canvas laser_gun_crosshair;
    public Image missile_image;
    public Image lasergun_image;
    private RectTransform missile_crosshair_panel;
    private Vector2 initial_crosshair_size;
    private GameObject target_in_focus;
    private float lerp_speed = 0.8f;
    private float rocket_launch_time = 1.5f;
    private float rocket_launch_timer = 1.5f;
    private float lasergun_launch_time = 1.5f;
    private float lasergun_launch_timer = 1.5f;
    private float laserbeam_lifetime = 0.5f;
    private GameObject target;
    private bool missile_launcher_equipped = false;
    private bool laser_gun_equipped = false;
    private float explosion_radius = 15f;
    private float rocket_damage = 25;

    // Start is called before the first frame update
    void Start()
    {
        missile_crosshair.enabled = false;
        laser_gun_crosshair.enabled = false;
        missile_crosshair_panel = missile_crosshair.transform.Find("CrosshairPanel").GetComponent<RectTransform>();
        initial_crosshair_size = missile_crosshair_panel.sizeDelta;
    }
    private GameObject getClosestTarget()
    {
        float min_distance = float.MaxValue;
        GameObject closest_target = null;

        foreach (GameObject g in sensor.Objects)
        {
            //Debug.Log(g.name);
            if (spaceship != null && g != null && g.name.Contains("Tanker"))
            {
                float dist = Vector3.Distance(spaceship.transform.position, g.transform.position);
                if (dist < min_distance)
                {
                    min_distance = dist;
                    closest_target = g;
                }
            }
        }
        if (closest_target != null)
            return closest_target;

        foreach (GameObject g in sensor.Objects)
        {
            if(spaceship != null && g != null)
            {
                float dist = Vector3.Distance(spaceship.transform.position, g.transform.position);
                if (dist < min_distance)
                {
                    min_distance = dist;
                    closest_target = g;
                }
            }
        }

        return closest_target;
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject != null)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                missile_launcher_equipped = true;
                laser_gun_equipped = false;
                missile_image.color = Color.green;
                lasergun_image.color = Color.white;
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                missile_launcher_equipped = false;
                laser_gun_equipped = true;
                missile_image.color = Color.white;
                lasergun_image.color = Color.green;
            }

            if (missile_launcher_equipped)
            {
                laser_gun_crosshair.enabled = false;
                if (sensor.Objects.Count == 0)
                {
                    missile_crosshair.enabled = false;
                }
                else
                {
                    if (target_in_focus != null)
                        missile_crosshair_panel.position = Camera.main.WorldToScreenPoint(target_in_focus.transform.position);

                    missile_crosshair.enabled = true;
                    GameObject closest_target = getClosestTarget();
                    if (target_in_focus == null || target_in_focus != closest_target)
                    {
                        target_in_focus = closest_target;
                        StartCoroutine(FocusOnTarget());
                    }
                }
                if (Input.GetButtonDown("Fire1") && sensor.Objects.Count > 0 && spaceship != null)
                {
                    rocket_launch_timer = 0;
                    GameObject rocket = Instantiate(rocket_prefab, spaceship.transform.position + transform.forward * 3, rocket_prefab.transform.rotation);
                    rocket.transform.localScale /= 3;
                    target = getClosestTarget();
                    rocket.transform.LookAt(target.transform);
                    StartCoroutine(LaunchRocket(rocket));
                }
            }
            else if (laser_gun_equipped)
            {
                laser_gun_crosshair.enabled = true;
                missile_crosshair.enabled = false;

                if (Input.GetButtonDown("Fire1") && spaceship != null)
                {
                    lasergun_launch_timer = 0;
                    GameObject laserbeam_left = Instantiate(laserbeam_prefab, spaceship.transform.position + transform.forward * 3 - transform.right * 2.2f, laserbeam_prefab.transform.rotation);
                    GameObject laserbeam_right = Instantiate(laserbeam_prefab, spaceship.transform.position + transform.forward * 3 + transform.right * 2.2f, laserbeam_prefab.transform.rotation);
                    //rocket.transform.localScale /= 3;
                    //target = getClosestTarget();
                    //laserbeam_left.transform.LookAt(transform.forward);
                    //laserbeam_right.transform.LookAt(transform.forward);
                    laserbeam_left.transform.forward = Quaternion.AngleAxis(0.1f, transform.right) * Quaternion.AngleAxis(1.5f, transform.up) * transform.forward;
                    laserbeam_right.transform.forward = Quaternion.AngleAxis(0.1f, transform.right) * Quaternion.AngleAxis(-1f, transform.up) * transform.forward;
                    StartCoroutine(LaunchLaserBeam(laserbeam_left));
                    StartCoroutine(LaunchLaserBeam(laserbeam_right));
                }
            }
        }        
    }

    private IEnumerator FocusOnTarget()
    {
        missile_crosshair_panel.sizeDelta = initial_crosshair_size;
        float elapsed = 0f;

        while (elapsed < lerp_speed)
        {
            elapsed += Time.deltaTime;
            missile_crosshair_panel.sizeDelta = Vector2.Lerp(initial_crosshair_size, initial_crosshair_size / 1.5f, elapsed / lerp_speed);
            yield return null;
        }

        missile_crosshair_panel.sizeDelta = initial_crosshair_size / 1.5f;
    }
    private IEnumerator LaunchRocket(GameObject rocket)
    {
        while (rocket != null && target != null && Vector3.Distance(target.transform.position, rocket.transform.position) > 3f)
        {
            float real_speed = missile_speed;
            if (Vector3.Distance(target.transform.position, rocket.transform.position) < 8f)
                real_speed *= 2.5f;
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

            if(col.transform.GetComponent<HealthPoints>() != null)
            {
                col.transform.GetComponent<HealthPoints>().takeDamage(rocket_damage);
                //Debug.Log(col.name + " HP IS NOW " + col.transform.GetComponent<HealthPoints>().hp);
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
