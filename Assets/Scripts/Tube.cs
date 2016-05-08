using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tube : MonoBehaviour
{
    public bool update;

    public bool doMeshCollider;

    //public Material externalMaterial;
    public Material internalMaterial;

    [Range(3, 64)]
    public int crossSegments = 12;

    public float thickness = 0.1f;

    public bool capEnds = true;
    public bool doInterior = true;

    public Vector2 stdUvScale = new Vector2(0.5f, 3);

    [System.Serializable]
    public class Segment
    {
        public float radius = 2;
        public float height = 3;

        public Material material;

        public int verticalSubtiles = 1;
        public int horizontalSubtiles = 1;

        public Vector2 uvScale;

        public Color vertexColor;

        public Segment(float radius, float height)
        {
            this.radius = radius;
            this.height = height;
        }

        public Segment(float radius, float height, Material externalMaterial)
        {
            this.radius = radius;
            this.height = height;
            this.material = externalMaterial;
        }

        public Segment(float radius, float height, Material mat, Color vertexColor)
        {
            this.radius = radius;
            this.height = height;
            this.material = mat;
            this.vertexColor = vertexColor;
        }



        public Segment(float radius, float height, Material externalMaterial, int vSt, int hSt, Vector2 uvScale)
        {
            this.radius = radius;
            this.height = height;
            this.material = externalMaterial;
            this.verticalSubtiles = vSt;
            this.horizontalSubtiles = hSt;
            this.uvScale = uvScale;
        }
    }

    public Segment[] segments;

    //Mesh mesh;

    void Start()
    {
        Remesh();

        first = true;
    }

    /*
    void Update()
    {
        if (update)
            Remesh();
    }*/

    bool first;

    void OnValidate()
    {
        if (first)
            Remesh();
    }

    private Vector3[] crossPoints;

    List<GameObject> cylinders = new List<GameObject>();

    [HideInInspector]
    public float totalHeight = 0;

    public void Remesh()
    {
        if (segments.Length == 0)
            return;

        if (cylinders.Count > 0)
        {
            for (int i = 0; i < cylinders.Count; i++)
            {
                Destroy(cylinders[i].GetComponent<MeshFilter>().sharedMesh);

                if (cylinders[i].GetComponent<MeshCollider>())
                    Destroy(cylinders[i].GetComponent<MeshCollider>().sharedMesh);

                Destroy(cylinders[i]);
            }

            cylinders.Clear();
        }

        if (segments.Length <= 1)
            return;

        totalHeight = 0;

        for (int i = 0; i < segments.Length - 1; i++)
        {
            var startPoint = transform.position + transform.up * totalHeight;
            var endPoint = transform.position + transform.up * (totalHeight + segments[i].height);

            DoObjectBand(startPoint, endPoint, segments[i].radius, segments[i + 1].radius, false, segments[i].material, segments[i].verticalSubtiles, segments[i].horizontalSubtiles, segments[i].uvScale, segments[i].vertexColor, segments[i + 1].vertexColor);
            if (doInterior) DoObjectBand(startPoint, endPoint, segments[i].radius, segments[i + 1].radius, true, internalMaterial, 1, 1, stdUvScale, segments[i].vertexColor, segments[i + 1].vertexColor);

            totalHeight += segments[i].height;
        }

        /*
        if (capEnds && thickness != 0)
        {
            var firstStartPoint = transform.position + Vector3.up * -0.0001f;
            var firstEndPoint = transform.position;

            var lastStartPoint = transform.position + Vector3.up * (totalHeight + 0.0001f);
            var lastEndPoint = transform.position + Vector3.up * totalHeight;

            DoObjectBand(firstStartPoint, firstEndPoint, segments[0].radius - thickness, segments[0].radius, false, internalMaterial, 1, 1, stdUvScale);
            DoObjectBand(lastStartPoint, lastEndPoint, segments[segments.Length - 1].radius - thickness, segments[segments.Length - 1].radius, false, internalMaterial, 1, 1, stdUvScale);
        }*/
    }

    public bool doConvexTriggers;

    //Vector3[] vertices;

    void DoObjectBand(Vector3 startPoint, Vector3 endPoint, float startRadius, float endRadius, bool interior, Material material, int verticalSubtiles, int HorizontalSubtiles, Vector2 uvScale, Color startColor, Color endColor)
    {
        GameObject go = new GameObject("CylinderSegment");
        go.name += interior ? "_interior" : "";

        go.AddComponent<MeshFilter>();
        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        mr.sharedMaterial = material;
        //mr.sharedMaterial = interior ? internalMaterial : externalMaterial;

        go.transform.parent = transform;

        cylinders.Add(go);

        DoBand(go, startPoint, endPoint, startRadius, endRadius, interior, verticalSubtiles, HorizontalSubtiles, uvScale, startColor, endColor);

        //go.AddComponent<NumberVertices>();
        //go.GetComponent<NumberVertices>().showNormals = true;

        if (!doMeshCollider)
            return;

        MeshCollider mc;

        if (go.GetComponent<MeshCollider>())
            mc = go.GetComponent<MeshCollider>();
        else
            mc = go.AddComponent<MeshCollider>();

        //MeshCollider mc = go.GetComponent<MeshCollider>() ?? go.AddComponent<MeshCollider>();

        mc.sharedMesh = go.GetComponent<MeshFilter>().mesh;
        mc.enabled = false;

        if (doConvexTriggers && interior)
            CreateConvexTrigger(go.GetComponent<MeshFilter>());
    }

    void DoBand(GameObject go, Vector3 startPoint, Vector3 endPoint, float startRadius, float endRadius, bool interior,
        int verticalSubtiles, int radialSubtiles, Vector2 uvScale, Color startColor, Color endColor)
    {
        float radDiff = startRadius - endRadius;

        float height = Vector3.Distance(startPoint, endPoint);
        float angle = (radDiff - height) / height + 1;

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

        //Color[] colors = new Color[vertices.Length * crossSegments];
        int[] tris = new int[2 * crossSegments * 6];
        int[] lastVertices = new int[crossSegments + 1];
        int[] theseVertices = new int[crossSegments + 1];
        Quaternion rotation = new Quaternion();

        float offset = interior ? -thickness : 0;

        for (int p = 0; p < 2; p++)
        {
            Vector3 point = p == 0 ? startPoint : endPoint;

            //if (p < 2 - 1)
            rotation = Quaternion.FromToRotation(Vector3.forward, endPoint - startPoint);

            float radius = p == 0 ? startRadius : endRadius;

            //float radialUVMult = / crossSegments;

            // create vertices at cross segments
            for (int c = 0; c < crossSegments + 1; c++)
            {
                int vertexIndex = p * (crossSegments + 1) + c;

                // #VERTICES
                vertices[vertexIndex] = point + rotation * crossPoints[c] * (radius + offset);

                // #UVs


                float meanRadius = (startRadius + endRadius) / 2;
                float circ = 2 * Mathf.PI * meanRadius;

                float uvHeight = RtF(p * height * uvScale.y, verticalSubtiles);

                float circRatio = circ / (2 * Mathf.PI);
                float uvRadial = (float)c * RtF((circRatio * uvScale.x), radialSubtiles) / (float)crossSegments; /// crossSegments; 

                if (!interior)
                    uvs[vertexIndex] = new Vector2(uvRadial, uvHeight); // /vertices.length
                else
                    uvs[vertexIndex] = new Vector2(uvRadial, (0.0f + p));


                if (c == crossSegments && p == 0)
                {
                    //uvs[vertexIndex] = new Vector2(((0.0f + 0) * uvHorzMult * circ) / crossSegments, (0.0f + p) * height * uvVertMult); // /vertices.length

                }

                if (c == crossSegments && p == 1)
                {
                    //uvs[vertexIndex] = new Vector2(((0.0f + 0) * uvHorzMult * circ) / crossSegments, (0.0f + p) * height * uvVertMult); // /vertices.length

                }

                //colors[vertexIndex] = vertices[p].color;

                lastVertices[c] = theseVertices[c];
                theseVertices[c] = p * (crossSegments + 1) + c;

                // #NORMALS

                normals[vertexIndex] = (Vector3.ProjectOnPlane(vertices[vertexIndex] - transform.position, transform.up).normalized + transform.up * angle).normalized;
                if (interior) normals[vertexIndex] *= -1;

                // COLORS
                if (p == 0)
                    colors[vertexIndex] = startColor;
                else
                    colors[vertexIndex] = endColor;
            }

            // TRIANGLES

            if (p > 0) // if not bottom vertices
            {
                for (int c = 0; c < crossSegments; c++)
                {
                    int start = (p * (crossSegments) + c) * 6;

                    tris[start] = lastVertices[c];
                    tris[start + 1] = interior ? theseVertices[c] : lastVertices[(c + 1)];
                    tris[start + 2] = interior ? lastVertices[(c + 1)] : theseVertices[c];

                    tris[start + 3] = tris[start + 2];
                    tris[start + 4] = tris[start + 1];
                    tris[start + 5] = theseVertices[(c + 1)];
                }
            }
        }

        //Clear mesh for new build  (jf)	
        Mesh mesh = new Mesh();
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.triangles = tris;

        mesh.normals = normals;
        //mesh.RecalculateNormals();

        //if (usingBumpmap)
        mesh.tangents = CalculateTangents(vertices);

        mesh.uv = uvs;

        /*
        if (useMeshCollision)
            if (colliderExists)
            {
                gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;
            }
            else
            {
                gameObject.AddComponent<MeshCollider>();
                colliderExists = true;
            }
            */

        go.GetComponent<MeshFilter>().mesh = mesh;
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

    public float CalculateVolume() // TODO: proly incorrect
    {
        // V=πr2h

        float V = 0;

        for (int i = 0; i < segments.Length - 1; i++) // skip last segment
        {
            V += 2 * Mathf.PI * segments[i].radius * segments[i].height;
        }

        return V;
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

    public float GetTotalLength()
    {
        float l = 0;

        foreach (var seg in segments)
            if (seg != null)
                l += seg.height;

        return l;
    }

    public static float RtF(float f, int divisions)
    {
        if (divisions == 0) divisions = 1; // overrides divisions

        float sf = f;

        // 4.2    (4.2 * 4) = 17            17 /4 = 4.25
        // 0.1    (0.1 * 4) = 0.4 = 0         

        sf *= divisions;

        //sf = Mathf.Ceil(sf);


        if (sf <= 0.5f && sf != 0)
            sf = Mathf.Ceil(sf);
        else
        {
            sf = Mathf.Round(sf);
        }

        return sf / divisions;
    }

}
