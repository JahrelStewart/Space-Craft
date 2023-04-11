using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI points_text;
    public TextMeshProUGUI allies_text;
    private static int num_of_spawned_allies = 0;
    private static int num_of_spawned_enemies = 0;
    private static int points = 0;
    private static int asteroid_gain = 5;
    private static int enemy_gain = 15;
    private int num_of_points_to_spawn = 30;
    private int max_num_of_allies = 5;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        num_of_spawned_allies = 0;
        num_of_spawned_enemies = 0;
        points = 0;
        SpawnEnemies();
    }

    public static void resetGame()
    {
        num_of_spawned_allies = 0;
        points = 0;
        num_of_spawned_enemies = 0;
    }

    public void SpawnEnemies()
    {
        int spawn_counter = num_of_spawned_enemies;

        while (spawn_counter < (num_of_spawned_allies + 1) * 2)
        {
            spawn_counter++;
            GameObject.Find("EnemiesSystem").GetComponent<EnemiesAI>().SpawnAnEnemy();
/*            foreach(EnemyAI enemy in GameObject.Find("EnemiesSystem").GetComponent<EnemiesAI>().enemies){
                enemy.GetComponent<HealthPoints>().max_hp = 100 * (num_of_spawned_allies + 1);
                enemy.GetComponent<HealthPoints>().hp = 100 * (num_of_spawned_allies + 1);
            }*/
        }
        num_of_spawned_enemies = spawn_counter;

    }

    // Update is called once per frame
    void Update()
    {
        if (num_of_spawned_enemies != (num_of_spawned_allies + 1) * 2)
        {
            SpawnEnemies();
        }

        if(points >= 100)
        {
            points_text.fontSize = 35;
            Vector3 pos = points_text.transform.localPosition;
            pos.x = -3;
            pos.y = -10;
            points_text.transform.localPosition = pos;
        }

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
    public static void allieDestroyed()
    {
        num_of_spawned_allies--;
    }
    public static void enemyDestroyed()
    {
        points += enemy_gain;
        num_of_spawned_enemies--;
    }

    public static int getPoints()
    {
        return points;
    }
}
