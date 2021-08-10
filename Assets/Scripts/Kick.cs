using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kick : MonoBehaviour
{
    Rigidbody _rb;
    Rigidbody rb { get { if (!_rb) _rb = GetComponent<Rigidbody>(); return _rb; } }

    public Vector3 force = new Vector3(0, 0, 1000);

    void Start()
    {
        rb.AddForce(force, ForceMode.VelocityChange);
    }
}
