using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    Rigidbody _rb;
    Rigidbody rb { get { if (!_rb) _rb = GetComponent<Rigidbody>(); return _rb; } }

    public float forceMult = 1000;
    public float torqueMult = 1000;

    void Start()
    {

    }

    private void FixedUpdate()
    {
        Vector3 trnInput = new Vector3(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), Input.GetAxis("Forward"));
        Vector3 rotInput = new Vector3(Input.GetAxis("Pitch"), Input.GetAxis("Roll"), Input.GetAxis("Yaw"));

        rb.AddForce(trnInput.x * transform.up * forceMult);
        rb.AddForce(trnInput.y * transform.right * forceMult);
        rb.AddForce(trnInput.z * transform.forward * forceMult);

        //float torqueMult = rotationForce * (1 + 5 * airDensity) * (1 + speedFactor) * Time.deltaTime;
        //float torqueMult = 

        rb.AddTorque(
            rotInput.x * transform.right * torqueMult +
            rotInput.y * transform.forward * torqueMult +
            rotInput.z * transform.up * torqueMult, 
            ForceMode.Acceleration);

    }

    void Update()
    {

    }
}
