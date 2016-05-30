using UnityEngine;
using System.Collections;

public class TerrainGen : MonoBehaviour
{

    SimplexNoiseGenerator simplex;

    public string seed = "42";

    public int mult = 25;
    public float amplitude = 0.5f;
    public int octaves = 1;

    void Start()
    {
        simplex = new SimplexNoiseGenerator(seed);

        Deform();
    }

    void TimeDeform()
    {
        float tTest = Time.realtimeSinceStartup; // TEST TIMER

        Deform();

        Debug.Log("Mesh gen: " + (Time.realtimeSinceStartup - tTest)); // TEST TIMER

        float tTest1 = Time.realtimeSinceStartup; // TEST TIMER

        AddCollider();

        Debug.Log("Collider making: " + (Time.realtimeSinceStartup - tTest1)); // TEST TIMER


    }

    void Deform()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        Mesh m = mf.mesh;

        Vector3[] vertices = m.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldPoint = transform.TransformPoint(vertices[i]);

            float noise = simplex.coherentNoise(worldPoint.x, worldPoint.y, worldPoint.z, octaves, mult, amplitude, 2, 0.9f);

            if (noise > 0.4f)
                vertices[i] += vertices[i].normalized * noise;
        }



        mf.mesh.vertices = vertices;
        mf.mesh.RecalculateNormals();
    }

    void AddCollider()
    {
        MeshFilter mf = GetComponent<MeshFilter>();

        MeshCollider col = GetComponent<MeshCollider>();
        if (col == null) col = gameObject.AddComponent<MeshCollider>();


        col.sharedMesh = mf.mesh;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            TimeDeform();
    }
}
