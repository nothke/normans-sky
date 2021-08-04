using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RingRenderer : MonoBehaviour
{
    public bool update;

    public Material material;

    [Range(3, 64)]
    public int crossSegments = 12;

    [System.Serializable]
    public class Segment
    {
        public float startRadius = 2;
        public float endRadius = 3;

        public Color startVertexColor;
        public Color endVertexColor;

        public Segment(float startRadius, float endRadius)
        {
            this.startRadius = startRadius;
            this.endRadius = endRadius;
        }


        public Segment(float startRadius, float endRadius, Color startColor, Color endColor)
        {
            this.startRadius = startRadius;
            this.endRadius = endRadius;
            this.startVertexColor = startColor;
            this.endVertexColor = endColor;
        }
    }

    public Segment[] segments;

    void Start()
    {
        //Remesh();

        first = true;
    }

    bool first;

    void OnValidate()
    {
        if (first)
            Remesh();
    }



    [HideInInspector]
    public float totalHeight = 0;

    MeshRenderer mr;
    MeshFilter mf;

    public void Remesh()
    {
        if (segments == null) return;
        if (segments.Length == 0) return;

        lastVert = 0;
        meshVertices.Clear();
        meshNormals.Clear();
        meshColors.Clear();
        meshTriangles.Clear();
        meshUVs.Clear();

        if (meshGO == null)
        {
            meshGO = new GameObject("RING");
            mr = meshGO.AddComponent<MeshRenderer>();
            mf = meshGO.AddComponent<MeshFilter>();
            mr.material = material;
        }
        else
            Destroy(meshGO.GetComponent<MeshFilter>().sharedMesh);

        for (int i = 0; i < segments.Length; i++)
        {
            DoBand(segments[i].startRadius, segments[i].endRadius, segments[i].startVertexColor, segments[i].endVertexColor);
        }

        CreateMesh();

        meshGO.transform.parent = transform;
        meshGO.transform.localPosition = Vector3.zero;
        meshGO.transform.localRotation = Quaternion.identity;
    }

    public List<Vector3> meshVertices = new List<Vector3>();
    public List<Vector3> meshNormals = new List<Vector3>();
    public List<Vector2> meshUVs = new List<Vector2>();
    public List<Color> meshColors = new List<Color>();
    public List<int> meshTriangles = new List<int>();

    GameObject meshGO;

    int lastVert;

    void DoBand(float startRadius, float endRadius, Color startColor, Color endColor)
    {
        float radDiff = startRadius - endRadius;

        Vector3 startPoint = Vector3.zero;
        Vector3 endPoint = Vector3.up * .00001f;

        Vector3[] crossPoints;

        //float height = Vector3.Distance(startPoint, endPoint);
        //float angle = (radDiff - height) / height + 1;

        // cross segments
        crossSegments = Mathf.Clamp(crossSegments, 3, 200); // Makes sure segments don't overflow

        crossPoints = new Vector3[crossSegments + 2];
        float theta = 2.0f * Mathf.PI / crossSegments;

        for (int c = 0; c < crossSegments + 1; c++)
        {
            crossPoints[c] = new Vector3(Mathf.Cos(theta * c), Mathf.Sin(theta * c), 0);
        }

        // create arrays
        Vector3[] vertices = new Vector3[2 * crossSegments + 2]; // + 2
        Color[] colors = new Color[2 * crossSegments + 2];
        Vector3[] normals = new Vector3[2 * crossSegments + 2]; // + 2
        Vector2[] uvs = new Vector2[2 * crossSegments + 2]; // + 2

        int[] tris = new int[2 * crossSegments * 6];

        int[] lastVertices = new int[crossSegments + 1];
        int[] theseVertices = new int[crossSegments + 1];
        Quaternion rotation = new Quaternion();

        for (int p = 0; p < 2; p++)
        {
            Vector3 point = p == 0 ? startPoint : endPoint;

            rotation = Quaternion.FromToRotation(Vector3.forward, endPoint - startPoint);

            float radius = p == 0 ? startRadius : endRadius;

            // create vertices at cross segments
            for (int c = 0; c < crossSegments + 1; c++)
            {
                int vertexIndex = p * (crossSegments + 1) + c;

                // #VERTICES
                vertices[vertexIndex] = point + rotation * crossPoints[c] * (radius);

                // #UVs
                uvs[vertexIndex] = Vector2.zero;

                // #TRIS SETUP
                lastVertices[c] = theseVertices[c];
                theseVertices[c] = lastVert + p * (crossSegments + 1) + c;

                // #NORMALS
                normals[vertexIndex] = meshGO.transform.up;

                // COLORS
                if (p == 0)
                    colors[vertexIndex] = startColor;
                else
                    colors[vertexIndex] = endColor;
            }

            // TRIANGLES

            /*
            if (p > 0) // if not bottom vertices
            {

            }*/
        }

        for (int c = 0; c < crossSegments; c++)
        {
            //int start = (p * (crossSegments) + c) * 6;
            int start = (crossSegments + c) * 6;

            tris[start] = lastVertices[c];
            tris[start + 1] = lastVertices[(c + 1)];
            tris[start + 2] = theseVertices[c];

            tris[start + 3] = tris[start + 2];
            tris[start + 4] = tris[start + 1];
            tris[start + 5] = theseVertices[(c + 1)];
        }

        List<int> trisCorrect = new List<int>();
        trisCorrect.AddRange(tris);
        trisCorrect.RemoveRange(0, crossSegments * 6);

        meshVertices.AddRange(vertices);
        meshNormals.AddRange(normals);
        meshUVs.AddRange(uvs);
        meshColors.AddRange(colors);
        meshTriangles.AddRange(trisCorrect);

        lastVert += vertices.Length;

    }

    void CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.Clear();
        mesh.vertices = meshVertices.ToArray();
        mesh.normals = meshNormals.ToArray();
        mesh.uv = meshUVs.ToArray();
        mesh.colors = meshColors.ToArray();
        mesh.triangles = meshTriangles.ToArray();

        meshGO.GetComponent<MeshFilter>().mesh = mesh;
    }

    Vector4[] CalculateTangents(Vector3[] verts)
    {
        Vector4[] tangents = new Vector4[verts.Length];

        for (int i = 0; i < tangents.Length; i++)
        {
            var vertex1 = i > 0 ? verts[i - 1] : verts[i];
            var vertex2 = i < tangents.Length - 1 ? verts[i + 1] : verts[i];
            var tan = (vertex1 - vertex2).normalized;
            tangents[i] = new Vector4(tan.x, tan.y, tan.z, 1.0f);
        }
        return tangents;
    }

    void CreateConvexTrigger(MeshFilter mf)
    {
        GameObject go = new GameObject("ModuleTrigger");
        go.transform.parent = mf.transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;

        go.layer = 2;

        MeshCollider mc = go.AddComponent<MeshCollider>();
        mc.sharedMesh = mf.mesh;
        mc.convex = true;

        mc.isTrigger = true;
        mc.enabled = false;
    }

    public static float RtF(float f, int divisions)
    {
        if (divisions == 0) divisions = 1; // overrides divisions

        float sf = f;

        sf *= divisions;


        if (sf <= 0.5f && sf != 0)
            sf = Mathf.Ceil(sf);
        else
        {
            sf = Mathf.Round(sf);
        }

        return sf / divisions;
    }

}
