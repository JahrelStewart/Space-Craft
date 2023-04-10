using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AISensor : MonoBehaviour
{

    public float distance = 10;
    public float angle = 30;
    public float height = 1.0f;
    public Color mesh_color = Color.red;
    public int scan_freq = 120;
    public LayerMask occlusion_layers;
    public LayerMask layers;
    public List<GameObject> Objects = new List<GameObject>();

    Collider[] colliders = new Collider[50];
    Mesh mesh;
    int count;
    float scan_interval;
    float scan_timer;

    // Start is called before the first frame update
    void Start()
    {
        mesh_color.a = 0.1f;
        scan_interval = 1.0f / scan_freq;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        scan_timer -= Time.deltaTime;
        if (scan_timer < 0)
        {
            scan_timer += scan_interval;
            Scan();
        }
    }

    private void Scan()
    {
        count = Physics.OverlapSphereNonAlloc(transform.position, distance, colliders, layers, QueryTriggerInteraction.Collide);

        Objects.Clear();
        for (int i = 0; i < count; i++)
        {
            GameObject obj = colliders[i].gameObject;
            if (IsInSight(obj))
            {
                Objects.Add(obj);
            }
        }
    }

    public bool IsInSight(GameObject obj)
    {
        Vector3 origin = transform.position;
        Vector3 dest = obj.transform.position;
        Vector3 dir = dest - origin;

        //Debug.Log(dir);
        Vector3 up_normalized = new Vector3(Mathf.Abs(Mathf.Round(transform.up.normalized.x)), Mathf.Abs(Mathf.Round(transform.up.normalized.y)), Mathf.Abs(Mathf.Round(transform.up.normalized.z)));
        if(up_normalized.magnitude > 1)
        {
            if(up_normalized.x == 1)
            {
                up_normalized = Vector3.right;
            } else if(up_normalized.y == 1)
            {
                up_normalized = Vector3.up;
            }
            else if (up_normalized.z == 1)
            {
                up_normalized = Vector3.forward;
            }
        }

        //Debug.Log(up_normalized);

        if (up_normalized == Vector3.up)
        {
            if (dir.y < -height || dir.y > height)
            {
                return false;
            }
            else
            {
                dir.y = 0;
            }
        } else if (up_normalized == Vector3.forward)
        {
            if (dir.z < -height || dir.z > height)
            {
                return false;
            }
            else
            {
                dir.z = 0;
            }
        }
        else if (up_normalized == Vector3.right)
        {
            if (dir.x < -height || dir.x > height)
            {
                return false;
            }
            else
            {
                dir.x = 0;
            }
        }


        float delta_angle = Vector3.Angle(dir, transform.forward);
        if (delta_angle > angle)
        {
            return false;
        }

        origin.y += height / 2;
        dest.y = origin.y;
        if (Physics.Linecast(origin, dest, occlusion_layers))
        {
            return false;
        }
        return true;
    }

    Mesh CreateWedgeMesh()
    {
        Mesh mesh = new Mesh();

        int segments = 10;
        int num_of_triangles = (segments * 4) + 4;
        int num_of_vertices = num_of_triangles * 3;

        Vector3[] vertices = new Vector3[num_of_vertices];
        int[] triangles = new int[num_of_vertices];

        Vector3 bottom_center = Vector3.zero - Vector3.up * height / 2;
        Vector3 bottom_left = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
        Vector3 bottom_right = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;

        Vector3 top_center = bottom_center + Vector3.up * height;
        Vector3 top_left = bottom_left + Vector3.up * height;
        Vector3 top_right = bottom_right + Vector3.up * height;

        int vert = 0;

        // left side
        vertices[vert++] = bottom_center;
        vertices[vert++] = bottom_left;
        vertices[vert++] = top_left;

        vertices[vert++] = top_left;
        vertices[vert++] = top_center;
        vertices[vert++] = bottom_center;

        // right side
        vertices[vert++] = bottom_center;
        vertices[vert++] = top_center;
        vertices[vert++] = top_right;

        vertices[vert++] = top_right;
        vertices[vert++] = bottom_right;
        vertices[vert++] = bottom_center;

        float current_angle = -angle;
        float delta_angle = (angle * 2) / segments;
        for (int i = 0; i < segments; i++)
        {
            bottom_left = Quaternion.Euler(0, current_angle, 0) * Vector3.forward * distance;
            bottom_right = Quaternion.Euler(0, current_angle + delta_angle, 0) * Vector3.forward * distance;

            top_left = bottom_left + Vector3.up * height;
            top_right = bottom_right + Vector3.up * height;

            // far side
            vertices[vert++] = bottom_left;
            vertices[vert++] = bottom_right;
            vertices[vert++] = top_right;

            vertices[vert++] = top_right;
            vertices[vert++] = top_left;
            vertices[vert++] = bottom_left;

            // top
            vertices[vert++] = top_center;
            vertices[vert++] = top_left;
            vertices[vert++] = top_right;

            // bottom
            vertices[vert++] = bottom_center;
            vertices[vert++] = bottom_right;
            vertices[vert++] = bottom_left;

            current_angle += delta_angle;
        }


        for (int i = 0; i < num_of_vertices; i++)
        {
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    private void OnValidate()
    {
        mesh = CreateWedgeMesh();
        scan_interval = 1.0f / scan_freq;
    }

    private void OnDrawGizmos()
    {

        if (mesh)
        {
            Gizmos.color = mesh_color;
            Gizmos.DrawMesh(mesh, transform.position, transform.rotation);
        }

        Gizmos.DrawWireSphere(transform.position, distance);
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = colliders[i].gameObject.transform.position;
            pos.y += 0.5f;
            Gizmos.DrawSphere(pos, 1f);
        }

        Gizmos.color = Color.green;
        foreach (var obj in Objects)
        {
            Vector3 pos = obj.transform.position;
            pos.y += 0.5f;
            Gizmos.DrawSphere(pos, 1.2f);
        }
    }
}
