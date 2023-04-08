using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidCollisionExplosion : MonoBehaviour
{
    public GameObject asteroid_prefab;
    private float cube_size = 1.5f;
    private int cubes_in_row = 5;
    private float cubes_pivot_distance;
    private Vector3 cubes_pivot;
    private float explosion_radius = 35;
    private float explosion_force = 500;
    private float explosion_upward = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        cubes_pivot_distance = cube_size * cubes_in_row / 2;
        cubes_pivot = new Vector3(cubes_pivot_distance, cubes_pivot_distance, cubes_pivot_distance);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name.Contains("Spaceship"))
            explode();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("here");
        if (other.gameObject.name.Contains("Rocket") || other.gameObject.name.Contains("LaserBeam"))
            explode();
    }

    public void explode()
    {
        gameObject.SetActive(false);

        for(int x = 0; x < cubes_in_row; x++)
        {
            for (int y = 0; y < cubes_in_row; y++)
            {
                for (int z = 0; z < cubes_in_row; z++)
                {
                    createPiece(x, y, z);
                }
            }
        }

        Vector3 explosion_pos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosion_pos, explosion_radius);
        foreach(Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.AddExplosionForce(explosion_force, transform.position, explosion_radius, explosion_upward);
            }
        }
    }

    public void createPiece(int x, int y, int z)
    {
        GameObject piece;
        piece = Instantiate(asteroid_prefab, transform.position, transform.rotation);
        //piece = GameObject.CreatePrimitive(PrimitiveType.Cube);

        piece.transform.position = transform.position + new Vector3(cube_size * x, cube_size * y, cube_size * z);
        piece.transform.localScale = new Vector3(cube_size, cube_size, cube_size);

        piece.AddComponent<Rigidbody>();
        //piece.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 2);
        piece.GetComponent<Rigidbody>().mass = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
