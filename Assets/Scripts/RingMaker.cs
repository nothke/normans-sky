using UnityEngine;
using System.Collections;

public class RingMaker : MonoBehaviour
{
    public bool update;

    public Material material;
    Material curMat;

    public Color[] lerpableColors;
    public int minRings = 3;
    public int maxRings = 10;


    public int crossSegments = 64;

    public float startRadius = 1;
    public float minRadius = 0.01f;
    public float maxRadius = 1;

    public Tube tube;

    void Start()
    {
        tube = gameObject.GetComponent<Tube>() ?? gameObject.AddComponent<Tube>();
        tube = gameObject.GetComponent<Tube>();

        Generate();
    }

    void Generate()
    {
        curMat = new Material(material);
        curMat.color = Color.HSVToRGB(Random.value, Random.Range(0, 0.4f), 1);

        int rings = Random.Range(minRings, maxRings);

        tube.segments = new Tube.Segment[rings];

        tube.capEnds = false;
        tube.internalMaterial = curMat;
        tube.crossSegments = crossSegments;

        float radius = startRadius;

        for (int i = 0; i < rings; i++)
        {
            radius += Random.Range(minRadius, maxRadius);

            Color c = Random.value < 0.4f ? GetLerpedColor() : Color.clear;
            tube.segments[i] = new Tube.Segment(radius, 0.002f, curMat, c);
        }

        tube.segments[0].vertexColor = Color.clear;
        tube.segments[rings - 1].vertexColor = Color.clear;

        tube.Remesh();
    }

    Color GetLerpedColor()
    {
        Color c1 = lerpableColors[Random.Range(0, lerpableColors.Length)];
        Color c2;

        do
        {
            c2 = lerpableColors[Random.Range(0, lerpableColors.Length)];
        } while (c2 == c1);

        return Color.Lerp(c1, c2, Random.value);
    }

    void Update()
    {
        if (update)
        {
            Generate();
            update = false;
        }
    }
}
