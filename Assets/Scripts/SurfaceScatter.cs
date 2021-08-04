using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceScatter : MonoBehaviour
{

    public GameObject prefab;
    public int num = 1000;

    public float radius;

    void Start()
    {
        for (int i = 0; i < num; i++)
        {
            Vector3 pos = transform.position + Random.onUnitSphere * radius;
            Vector3 normal = (pos - transform.position).normalized;
            Vector3 randForward = Vector3.ProjectOnPlane(Random.onUnitSphere, normal);
            Quaternion rot = Quaternion.LookRotation(randForward, normal);
            GameObject go = Instantiate(prefab, pos, rot) as GameObject;
        }
    }
}
