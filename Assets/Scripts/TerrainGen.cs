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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            Deform();
    }
}
