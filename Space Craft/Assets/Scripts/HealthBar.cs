using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image foreground;
    private float lerp_speed = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpaceshipMovement>().OnHealthChange += changeHealth;
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
/*        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0);*/
    }
}
