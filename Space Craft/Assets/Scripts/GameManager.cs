using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI points_text;
    public TextMeshProUGUI allies_text;
    private static int num_of_spawned_allies = 0;
    private static int points = 0;
    private static int asteroid_gain = 5;
    private static int enemy_gain = 15;
    private int num_of_points_to_spawn = 30;
    private int max_num_of_allies = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        points_text.text = points.ToString();
        allies_text.text = num_of_spawned_allies.ToString();

        if(points >= num_of_points_to_spawn && num_of_spawned_allies < max_num_of_allies)
        {
            num_of_points_to_spawn += 30;
            GameObject.Find("FlockingSystem").GetComponent<Flock>().SpawnAnAllie();
            num_of_spawned_allies++;
        }
    }


    public static int getNumOfAllies()
    {
        return num_of_spawned_allies;
    }

    public static void asteroidDestroyed()
    {
        points += asteroid_gain;
    }
    public static void enemyDestroyed()
    {
        points += enemy_gain;
    }
}
