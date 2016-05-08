using UnityEngine;
using System.Collections;

public class CelestialGravity : MonoBehaviour
{

    public float planetMass;
    public float gravitationalConstant = 0.000006f;
    public float gravityRange;

    public bool drawGizmos;

    Rigidbody[] sceneRigidbodies;

    [HideInInspector]
    public float force;

    void OnDrawGizmosSelected()
    {
        if (!drawGizmos)
            return;

        Gizmos.DrawWireSphere(transform.position, gravityRange);
    }

    // Use this for initialization
    void Start()
    {

        sceneRigidbodies = FindObjectsOfType(typeof(Rigidbody)) as Rigidbody[];
    }

    // Update is called once per frame
    void Update()
    {
        if (!Motion.e)
        {
            enabled = false;
            return;
        }

        Vector3 motionDir = transform.position - Motion.e.transform.position;
        if ((motionDir).sqrMagnitude < gravityRange * gravityRange)
            Motion.e.currentPlanet = GetComponent<PlanetEntity>();

        foreach (Rigidbody rb in sceneRigidbodies)
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
