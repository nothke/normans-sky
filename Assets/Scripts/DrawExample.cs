using UnityEngine;
using System.Collections;

public class DrawExample : MonoBehaviour
{

    public Material lineMaterial;

    public int sphereVSegs = 10;
    public int sphereRSegs = 10;

    public float sphereRadius = 1;

    public float orbitEccentricity = 0.5f;
    public float orbitSemiMajorAxis = 100;
    public float orbitAngle = 1;

    [System.Serializable]
    public class Orbit
    {
        public float eccentricity = 0;
        public float semiMajorAxis = 1;
        public Vector3 normal = Vector3.forward;
        public Vector3 forward = Vector3.right;
    }

    public Orbit[] orbits;

    public MeshFilter meshFilter;
    public Vector3[] edgePoints;

    public Color wireFrameColor = Color.blue;

    void Start()
    {
        edgePoints = Draw.GetEdgePointsFromMesh(meshFilter.sharedMesh);
    }

    void OnPostRender()
    {
        lineMaterial.SetPass(0);
        /*
        Draw.color = Color.white;

        // 2D

        // draws vertical line in the middle of the screen
        Draw.Line(new Vector2(0.5f, 0), new Vector2(0.5f, 1));

        Draw.color = Color.red;

        // draws rectangle at 200px,200px, with size of 200px*200px
        Draw.Rect(200, 200, 200, 200);

        Draw.color = Color.green;

        // draws circle at the center of the screen with 100px radius
        Draw.Circle(new Vector2(0.5f, 0.5f), 100);

        Draw.color = Color.white * 0.5f;
        // draws elliplse at the center of the screen
        Draw.Ellipse(new Vector2(0.5f, 0.5f), Draw.PixelToScreen(200, 50));

        // 3D

        // Draws a cube rotating around the scene origin
        Vector3 cubeForward = new Vector3(Mathf.Sin(Time.time), 0, Mathf.Cos(Time.time));
        Draw.Cube(Vector3.zero, Vector3.one, cubeForward, Vector3.up);
        Draw.color.a = 0.8f;
        Draw.Cube(Vector3.zero, Vector3.one * 2, cubeForward, Vector3.up);
        Draw.color.a = 0.6f;
        Draw.Cube(Vector3.zero, Vector3.one * 3, -cubeForward, Vector3.up);
        Draw.color.a = 0.4f;
        Draw.Cube(Vector3.zero, Vector3.one * 4, -cubeForward, Vector3.up);
        Draw.color.a = 0.2f;
        Draw.Cube(Vector3.zero, Vector3.one * 5, -cubeForward, Vector3.up);
        */

        Draw.color = Color.white;
        //Draw.OrbitApses(new Vector2(0.5f, 0.5f), orbitEccentricity, orbitSemiMajorAxis, orbitAngle);
        //Draw.Circle(new Vector2(0.5f, 0.5f), 50);

        foreach (var orbit in orbits)
        {
            Draw.Orbit3DApses(Vector3.zero, orbit.eccentricity, orbit.semiMajorAxis, orbit.normal, orbit.forward);
        }

        Draw.color = Color.yellow;
        Draw.color.a = 0.2f;

        float ringMinRad = 2;
        float ringMaxRad = 3;
        int ringNum = 10;

        for (int i = 0; i < ringNum; i++)
        {
            float rad = Mathf.Lerp(ringMinRad, ringMaxRad, (float)i / ringNum);

            Draw.Circle3D(Vector3.zero, rad, Vector3.up);
        }

        Draw.color = Color.cyan;
        Draw.Sphere(Vector3.zero, sphereRadius, sphereVSegs, sphereRSegs);

        Draw.color = new Color(0, 0.2f, 1);
        Draw.color.a = 0.2f;

        // Draws a grid on the origin
        Draw.Grid(Vector3.zero, 10);

        Draw.color = wireFrameColor;

        // Wireframe
        Draw.Wireframe(meshFilter.transform, edgePoints);
        //Draw.Wireframe(meshFilter.transform, Draw.GetEdgePointsFromMesh(meshFilter.sharedMesh, 0.01f));
    }
}