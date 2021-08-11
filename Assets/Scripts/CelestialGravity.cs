/// Notes:
/// 
/// In reality gravitational constant G is
/// G = 6.674 × 10^−11 Nm2/kg2
/// and mass of the earth is
/// earthMass = 5.972 × 10^24 kg
/// for precision and simplicity we will use Standard gravitational parameter for earth
/// μ = 398600.4418 km/s2
/// 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CelestialGravity : MonoBehaviour
{
    // TODO: Use bodies' mass
    public float bodyMass = 100000;

    // TODO: Why is there 2 constants??
    public float gravitationalConstant = 0.000006f;

    // used as a replacement for gravitational constant
    public const float GRAVITY_CONSTANT = 600;
    public const float RANGE_FORCE_THRESHOLD = 10;

    //public static float μ = 398600.4418f; // in km/s2

    [HideInInspector]
    public float force;

    float distanceThreshold;

    private void Start()
    {
        distanceThreshold = GetThresholdDistance(bodyMass);
    }

    void OnDrawGizmosSelected()
    {
        distanceThreshold = GetThresholdDistance(bodyMass);

        Gizmos.DrawWireSphere(transform.position, distanceThreshold);
    }

    // Update is called once per frame
    public static float GetForceMagnitude(float bodyEarthMasses, float rbMassKilos, float squareDistance)
    {
        return (GRAVITY_CONSTANT * bodyEarthMasses * rbMassKilos) / (squareDistance);
    }

    public static float GetAcceleration(float bodyEarthMasses, float squareDistance)
    {
        return (GRAVITY_CONSTANT * bodyEarthMasses) / (squareDistance);
    }

    public static float CalculateVelocityForCircularOrbit(float bodyEarthMasses, float squareDistance)
    {
        float r = Mathf.Sqrt(squareDistance);
        return (bodyEarthMasses * GRAVITY_CONSTANT) / r;
        //return Mathf.Sqrt(μ / r);
    }

    public static float GetThresholdDistance(float bodyMass) // TODO: make it not work in every frame
    {
        float k = bodyMass * GRAVITY_CONSTANT;
        return Mathf.Sqrt(k / RANGE_FORCE_THRESHOLD);
    }

    float RangeSqr { get { return distanceThreshold * distanceThreshold; } }

    void Update()
    {
        if (Motion.e)
        {
            Vector3 motionDiff = transform.position - Motion.e.transform.position;
            if ((motionDiff).sqrMagnitude < RangeSqr)
                Motion.e.currentBody = GetComponent<PlanetEntity>();
        }
    }

    private void FixedUpdate()
    {
        if (BodyManager.e)
        {
            foreach (Rigidbody rb in BodyManager.e.rigidbodies)
            {
                Vector3 direction = transform.position - rb.position;
                float rSqr = (direction).sqrMagnitude;

                force = (gravitationalConstant * rb.mass * bodyMass) / rSqr;

                if (rSqr < RangeSqr)
                {
                    ApplyGravityForce(rb);
                }
            }
        }
    }

    public void ApplyGravityForce(Rigidbody rb)
    {
        Vector3 direction = transform.position - rb.position;
        float rSqr = (direction).sqrMagnitude;


        if (rSqr < RangeSqr)
        {
            Debug.Log($"Applying force {GetAcceleration(bodyMass, rSqr)} to {rb.name} to ");
            force = GetAcceleration(bodyMass, rSqr);
            rb.AddForce(force * direction.normalized / 60, ForceMode.Acceleration);
        }
    }
}