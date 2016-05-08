using UnityEngine;
using System.Collections;

public class TimedTrailRenderer : MonoBehaviour
{

    public bool emit = true;
    public float emitTime = 0.00f;
    public Material material;

    public float lifeTime = 1.00f;

    public Color[] colors;
    public float[] sizes;

    public float uvLengthScale = 0.01f;
    public bool higherQualityUVs = true;

    public int movePixelsForRebuild = 6;
    public float maxRebuildTime = 0.1f;

    public float minVertexDistance = 0.10f;

    public float maxVertexDistance = 10.00f;
    public float maxAngle = 3.00f;

    public bool autoDestruct = false;

    [HideInInspector]
    public ArrayList points = new ArrayList(); // Nothke - made public
    private GameObject o;
    private Vector3 lastPosition;
    private Vector3 lastCameraPosition1;
    private Vector3 lastCameraPosition2;
    private float lastRebuildTime = 0.00f;
    private bool lastFrameEmit = true;

    public class Point
    {
        public float timeCreated = 0.00f;
        public Vector3 position;
        public bool lineBreak = false;
    }

    void Start()
    {
        lastPosition = transform.position;
        o = new GameObject("Trail");
        o.transform.parent = null;
        o.transform.position = Vector3.zero;
        o.transform.rotation = Quaternion.identity;
        o.transform.localScale = Vector3.one;
        o.AddComponent(typeof(MeshFilter));
        o.AddComponent(typeof(MeshRenderer));
        o.GetComponent<Renderer>().sharedMaterial = material;
    }

    void OnEnable()
    {
        lastPosition = transform.position;
        o = new GameObject("Trail");
        o.transform.parent = null;
        o.transform.position = Vector3.zero;
        o.transform.rotation = Quaternion.identity;
        o.transform.localScale = Vector3.one;
        o.AddComponent(typeof(MeshFilter));
        o.AddComponent(typeof(MeshRenderer));
        o.GetComponent<Renderer>().sharedMaterial = material;
    }

    void OnDisable()
    {
        Destroy(o);
    }

    void Update()
    {
        if (emit && emitTime != 0)
        {
            emitTime -= Time.deltaTime;
            if (emitTime == 0) emitTime = -1;
            if (emitTime < 0) emit = false;
        }

        if (!emit && points.Count == 0 && autoDestruct)
        {
            Destroy(o);
            Destroy(gameObject);
        }

        // early out if there is no camera
        if (!Camera.main) return;

        bool re = false;

        // if we have moved enough, create a new vertex and make sure we rebuild the mesh
        float theDistance = (lastPosition - transform.position).magnitude;
        if (emit)
        {
            if (theDistance > minVertexDistance)
            {
                bool make = false;
                if (points.Count < 3)
                {
                    make = true;
                }
                else
                {
                    Vector3 l1 = ((Point)points[points.Count - 2]).position - ((Point)points[points.Count - 3]).position;
                    Vector3 l2 = ((Point)points[points.Count - 1]).position - ((Point)points[points.Count - 2]).position;
                    if (Vector3.Angle(l1, l2) > maxAngle || theDistance > maxVertexDistance) make = true;
                }

                if (make)
                {
                    Point p = new Point();
                    p.position = transform.position;
                    p.timeCreated = Time.time;
                    points.Add(p);
                    lastPosition = transform.position;
                }
                else
                {
                    ((Point)points[points.Count - 1]).position = transform.position;
                    ((Point)points[points.Count - 1]).timeCreated = Time.time;
                }
            }
            else if (points.Count > 0)
            {
                ((Point)points[points.Count - 1]).position = transform.position;
                ((Point)points[points.Count - 1]).timeCreated = Time.time;
            }
        }

        if (!emit && lastFrameEmit && points.Count > 0) ((Point)points[points.Count - 1]).lineBreak = true;
        lastFrameEmit = emit;

        // approximate if we should rebuild the mesh or not
        if (points.Count > 1)
        {
            Vector3 cur1 = Camera.main.WorldToScreenPoint(((Point)points[0]).position);
            lastCameraPosition1.z = 0;
            Vector3 cur2 = Camera.main.WorldToScreenPoint(((Point)points[points.Count - 1]).position);
            lastCameraPosition2.z = 0;

            float distance = (lastCameraPosition1 - cur1).magnitude;
            distance += (lastCameraPosition2 - cur2).magnitude;

            if (distance > movePixelsForRebuild || Time.time - lastRebuildTime > maxRebuildTime)
            {
                re = true;
                lastCameraPosition1 = cur1;
                lastCameraPosition2 = cur2;
            }
        }
        else
        {
            re = true;
        }


        if (re)
        {
            lastRebuildTime = Time.time;

            ArrayList remove = new ArrayList();
            int i = 0;
            foreach (Point p in points)
            {
                // cull old points first
                if (Time.time - p.timeCreated > lifeTime) remove.Add(p);
                i++;
            }

            foreach (Point p in remove) points.Remove(p);
            remove.Clear();

            if (points.Count > 1)
            {
                Vector3[] newVertices = new Vector3[points.Count * 2];
                Vector2[] newUV = new Vector2[points.Count * 2];
                int[] newTriangles = new int[(points.Count - 1) * 6];
                Color[] newColors = new Color[points.Count * 2];

                i = 0;
                float curDistance = 0.00f;

                foreach (Point p in points)
                {
                    float time = (Time.time - p.timeCreated) / lifeTime;

                    Color color = Color.Lerp(Color.white, Color.clear, time);
                    if (colors != null && colors.Length > 0)
                    {
                        float colorTime = time * (colors.Length - 1);
                        float min = Mathf.Floor(colorTime);
                        float max = Mathf.Clamp(Mathf.Ceil(colorTime), 1, colors.Length - 1);
                        float lerp = Mathf.InverseLerp(min, max, colorTime);
                        if (min >= colors.Length) min = colors.Length - 1; if (min < 0) min = 0;
                        if (max >= colors.Length) max = colors.Length - 1; if (max < 0) max = 0;
                        color = Color.Lerp(colors[(int)min], colors[(int)max], lerp);
                    }

                    float size = 1f;
                    if (sizes != null && sizes.Length > 0)
                    {
                        float sizeTime = time * (sizes.Length - 1);
                        float min = Mathf.Floor(sizeTime);
                        float max = Mathf.Clamp(Mathf.Ceil(sizeTime), 1, sizes.Length - 1);
                        float lerp = Mathf.InverseLerp(min, max, sizeTime);
                        if (min >= sizes.Length) min = sizes.Length - 1; if (min < 0) min = 0;
                        if (max >= sizes.Length) max = sizes.Length - 1; if (max < 0) max = 0;
                        size = Mathf.Lerp(sizes[(int)min], sizes[(int)max], lerp);
                    }

                    Vector3 lineDirection = Vector3.zero;
                    if (i == 0) lineDirection = p.position - ((Point)points[i + 1]).position;
                    else lineDirection = ((Point)points[i - 1]).position - p.position;

                    Vector3 vectorToCamera = Camera.main.transform.position - p.position;
                    Vector3 perpendicular = Vector3.Cross(lineDirection, vectorToCamera).normalized;

                    newVertices[i * 2] = p.position + (perpendicular * (size * 0.5f));
                    newVertices[(i * 2) + 1] = p.position + (-perpendicular * (size * 0.5f));

                    newColors[i * 2] = newColors[(i * 2) + 1] = color;

                    newUV[i * 2] = new Vector2(curDistance * uvLengthScale, 0);
                    newUV[(i * 2) + 1] = new Vector2(curDistance * uvLengthScale, 1);

                    if (i > 0 && !((Point)points[i - 1]).lineBreak)
                    {
                        if (higherQualityUVs) curDistance += (p.position - ((Point)points[i - 1]).position).magnitude;
                        else curDistance += (p.position - ((Point)points[i - 1]).position).sqrMagnitude;

                        newTriangles[(i - 1) * 6] = (i * 2) - 2;
                        newTriangles[((i - 1) * 6) + 1] = (i * 2) - 1;
                        newTriangles[((i - 1) * 6) + 2] = i * 2;

                        newTriangles[((i - 1) * 6) + 3] = (i * 2) + 1;
                        newTriangles[((i - 1) * 6) + 4] = i * 2;
                        newTriangles[((i - 1) * 6) + 5] = (i * 2) - 1;
                    }

                    i++;
                }

                Mesh mesh = (o.GetComponent(typeof(MeshFilter)) as MeshFilter).mesh;
                mesh.Clear();
                mesh.vertices = newVertices;
                mesh.colors = newColors;
                mesh.uv = newUV;
                mesh.triangles = newTriangles;
            }
        }
    }
}