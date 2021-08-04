#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using UnityEditor;


public class NumberVertices : MonoBehaviour
{
    public bool showVertexNumbers = true;
    public bool showNormals = true;
    public float normalGizmoLength = 0.2f;
    public bool drawTris;

    void OnDrawGizmos()
    {
        if (GetComponent<MeshFilter>())
            if (GetComponent<MeshFilter>().mesh)
            {
                var mf = GetComponent<MeshFilter>();

                if (mf.mesh.vertices.Length > 0)
                {
                    for (int i = 0; i < mf.mesh.vertices.Length; i++)
                    {
                        if (showVertexNumbers)
                            Handles.Label(mf.mesh.vertices[i], i.ToString());

                        Gizmos.color = Color.green;

                        if (showNormals)
                            Gizmos.DrawRay(mf.mesh.vertices[i], mf.mesh.normals[i] * normalGizmoLength);
                    }

                    if (drawTris)
                    {
                        for (int i = 0; i < mf.mesh.triangles.Length; i += 3)
                        {
                            int v0 = mf.mesh.triangles[i];
                            int v1 = mf.mesh.triangles[i + 1];
                            int v2 = mf.mesh.triangles[i + 2];

                            Gizmos.DrawLine(mf.mesh.vertices[v0], mf.mesh.vertices[v1]);
                            Gizmos.DrawLine(mf.mesh.vertices[v1], mf.mesh.vertices[v2]);
                            Gizmos.DrawLine(mf.mesh.vertices[v2], mf.mesh.vertices[v0]);

                            Gizmos.DrawSphere(mf.mesh.vertices[v0], 1);
                        }
                    }
                }
            }
    }
}
#endif