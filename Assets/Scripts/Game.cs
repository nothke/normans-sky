using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game e;

    [System.NonSerialized] public List<Rigidbody> rigidbodies;

    void Awake()
    {
        e = this;
        rigidbodies = new List<Rigidbody>();

        rigidbodies.AddRange(FindObjectsOfType<Rigidbody>());
    }
}
