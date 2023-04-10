using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image foreground;
    public Canvas hpcanvas;
    private float lerp_speed = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<HealthPoints>().OnHealthChange += changeHealth;
    }

    public void changeHealth(float percentage)
    {
        StartCoroutine(lerpHealth(percentage));
    }

    private IEnumerator lerpHealth(float percentage)
    {
        float initial_percentage = foreground.fillAmount;
        float elapsed = 0f;

        while(elapsed < lerp_speed)
        {
            elapsed += Time.deltaTime;
            foreground.fillAmount = Mathf.Lerp(initial_percentage, percentage, elapsed / lerp_speed);
            yield return null;
        }

        foreground.fillAmount = percentage;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        if (!transform.name.Contains("Spaceship"))
        {
            AsteroidFlotation af = transform.parent.Find("Asteroid").GetComponent<AsteroidFlotation>();
            hpcanvas.transform.position += af.getFlotationDir() * af.getFlotationSpeed() * Time.deltaTime;
            hpcanvas.transform.LookAt(Camera.main.transform);
/*            Vector3 init_scale = new Vector3(15, 15, 15);
            hpcanvas.transform.localScale = transform.parent.Find("Asteroid").localScale.magnitude / init_scale.magnitude * Vector3.one;*/
        }
        /*       
                transform.Rotate(0, 180, 0);*/
    }
}
