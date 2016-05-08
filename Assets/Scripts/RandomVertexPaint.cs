using UnityEngine;
using System.Collections;

public class RandomVertexPaint : MonoBehaviour
{

    // Use this for initialization

    public Color rC1;
    public Color rC2;

    void Start()
    {

        //float hue = Random.value;
        //float cHue = hue + 0.5f;
        //if (cHue > 1) cHue -= 1;

        //rC1 = Utils.HSVToRGB(hue, 1, 1f);
        //rC2 = Utils.HSVToRGB(cHue, 1, 0.4f);

        MeshFilter mf = GetComponent<MeshFilter>();

        Mesh mesh = mf.mesh;

        Color[] colors = new Color[mesh.vertices.Length];
        //Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.Lerp(rC1, rC2, Random.value);

            //colors[i].b = Mathf.Round(Mathf.PerlinNoise(1000 + mesh.vertices[i].x*0.5f, 1000 + mesh.vertices[i].y*0.5f));// Random.Range(0, 0.9999f);
            //colors[i].r = Random.value;

            //vertices[i] += Random.insideUnitSphere;

        }

        //mesh.vertices = vertices;
        mesh.colors = colors;

        mf.mesh = mesh;
    }

}
