using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthPoints : MonoBehaviour
{
    public float hp = 100;
    public float max_hp = 100;

    public event Action<float> OnHealthChange = delegate { };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void takeDamage(float damage)
    {
        hp -= damage;
        OnHealthChange(hp / max_hp);

        if(transform.name.Contains("Spaceship") && hp <= 0f)
        {
            GameManager.resetGame();
            Destroy(gameObject.transform.Find("PrototypeZero").gameObject);
            Destroy(gameObject.transform.Find("Cylinder").gameObject);
            Destroy(gameObject.transform.Find("Sphere").gameObject);

            Invoke("LoadEndScene", 2f);
        }
        else if (transform.name.Contains("Tanker") && hp <= 0f)
        {
            GameObject.Find("EnemiesSystem").GetComponent<EnemiesAI>().enemies.Remove(transform.GetComponent<EnemyAI>());
            Destroy(transform.gameObject);
            GameManager.enemyDestroyed();
        }
        else if (transform.name.Contains("BlueBull") && hp <= 0f)
        {
            GameObject.Find("EnemiesSystem").GetComponent<EnemiesAI>().allies.Remove(transform.gameObject);
            Destroy(transform.gameObject);
            GameManager.allieDestroyed();
        }
    }

    void LoadEndScene()
    {
        SceneManager.LoadScene(2);
    }
}
