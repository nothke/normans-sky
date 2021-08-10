using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyManager : MonoBehaviour
{
    public static BodyManager e;

    public List<Rigidbody> rigidbodies;

    void Awake()
    {
        e = this;

        rigidbodies = new List<Rigidbody>();
        rigidbodies.AddRange(FindObjectsOfType<Rigidbody>());
    }
}
