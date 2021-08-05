using UnityEngine;
using System.Collections;

public class CelestialGravity : MonoBehaviour
{

    public float planetMass;
    public float gravitationalConstant = 0.000006f;
    public float gravityRange;

    public bool drawGizmos;

    [HideInInspector]
    public float force;

    void OnDrawGizmosSelected()
    {
        if (!drawGizmos)
            return;

        Gizmos.DrawWireSphere(transform.position, gravityRange);
    }

    // Update is called once per frame
    void Update()
    {
        if (!Motion.e)
        {
            enabled = false;
            return;
        }

        Vector3 motionDiff = transform.position - Motion.e.transform.position;
        if ((motionDiff).sqrMagnitude < gravityRange * gravityRange)
            Motion.e.currentPlanet = GetComponent<PlanetEntity>();

        if (BodyManager.e)
        {
            foreach (Rigidbody rb in BodyManager.e.rigidbodies)
            {
                Vector3 direction = transform.position - rb.position;
                float rSqr = (direction).sqrMagnitude;

                force = (gravitationalConstant * rb.mass * planetMass) / rSqr;

                if (rSqr < gravityRange * gravityRange)
                {
                    rb.AddForce(force * direction.normalized * Time.deltaTime);
                }
            }
        }
    }
}