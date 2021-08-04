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

    public float firstRadius = 1;
    public float minRadius = 0.01f;
    public float maxRadius = 1;

    RingRenderer ring;

    void Start()
    {
        ring = gameObject.GetComponent<RingRenderer>() ?? gameObject.AddComponent<RingRenderer>();
        ring = gameObject.GetComponent<RingRenderer>();

        Generate();
    }

    void Generate()
    {
        curMat = new Material(material);
        curMat.color = Color.HSVToRGB(Random.value, Random.Range(0, 0.4f), 1);

        int rings = Random.Range(minRings, maxRings);

        ring.segments = new RingRenderer.Segment[rings];

        ring.crossSegments = crossSegments;

        float endRadius = firstRadius;

        ring.material = material;

        Color prevColor = Color.clear;

        for (int i = 0; i < rings; i++)
        {
            float startRadius = endRadius;

            endRadius += Random.Range(minRadius, maxRadius);

            Color c = Random.value < 0.4f ? GetLerpedColor() : Color.clear;

            ring.segments[i] = new RingRenderer.Segment(startRadius, endRadius, prevColor, c);

            prevColor = c;

            //firstRadius = endRadius;
        }

        ring.segments[0].startVertexColor = Color.clear;
        ring.segments[rings - 1].endVertexColor = Color.clear;

        ring.Remesh();
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
