using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CelestialGravity : MonoBehaviour
{

    public float bodyMass = 100000;
    //public float bodyEarthMasses = 1;
    //public float gravitationalConstant = 0.000006f;

    //static float gravityRange = 2000;

    public bool drawGizmos;

    // in reality gravitational constant G is
    // G = 6.674 × 10^−11 Nm2/kg2
    // and mass of the earth is
    // earthMass = 5.972 × 10^24 kg

    // for precision and simplicity we will use Standard gravitational parameter for earth
    // μ = 398600.4418 km/s2
    // and calculate other masses using this equation

    // used as a replacement for gravitational constant
    public static float gravityMult = 600;
    public static float forceThreshold = 10;

    //public static float μ = 398600.4418f; // in km/s2

    [HideInInspector]
    public float force;

    public float gravAt;

    void OnDrawGizmosSelected()
    {
        if (!drawGizmos)
            return;

        gravAt = GetThresholdDistance(bodyMass);

        //Gizmos.DrawWireSphere(transform.position, GetThresholdDistance(bodyMass));
        Gizmos.DrawWireSphere(transform.position, gravAt);

    }

    public static float GetForceMagnitude(float bodyEarthMasses, float rbMassKilos, float squareDistance)
    {
        return (gravityMult * bodyEarthMasses * rbMassKilos) / (squareDistance);
    }

    public static float GetAcceleration(float bodyEarthMasses, float squareDistance)
    {
        return (gravityMult * bodyEarthMasses) / (squareDistance);
    }

    public static float CalculateVelocityForCircularOrbit(float bodyEarthMasses, float squareDistance)
    {
        float r = Mathf.Sqrt(squareDistance);
        return (bodyEarthMasses * gravityMult) / r;
        //return Mathf.Sqrt(μ / r);
    }

    public static float GetThresholdDistance(float bodyMass) // TODO: make it not work in every frame
    {
        float k = bodyMass * gravityMult;
        return Mathf.Sqrt(k / forceThreshold);
    }

    bool isIn;
    bool wasIn;

    float RangeSqr { get { return gravAt * gravAt; } }

    void Update()
    {
        if (Motion.e)
        {
            // check if ship is within range of this planet
            Vector3 motionDir = transform.position - Motion.e.transform.position;

            if ((motionDir).sqrMagnitude < RangeSqr)
            {
                if (!wasIn)
                    Motion.e.AddBody(GetComponent<CelestialBody>());

                wasIn = true;
            }
            else
            {
                if (wasIn)
                    Motion.e.RemoveBody(GetComponent<CelestialBody>());

                wasIn = false;
            }


            // make it the current planet
            //Motion.e.currentPlanet = GetComponent<PlanetEntity>();
        }

        if (Game.e)
        {
            foreach (Rigidbody rb in Game.e.rigidbodies)
            {
                ApplyGravityForce(rb);
            }
        }
    }

    public void ApplyGravityForce(Rigidbody rb)
    {
        Vector3 direction = transform.position - rb.position;
        float rSqr = (direction).sqrMagnitude;

        if (rSqr < RangeSqr)
        {
            force = GetAcceleration(bodyMass, rSqr);
            rb.AddForce(force * direction.normalized * Time.deltaTime, ForceMode.Acceleration);
        }
    }
}
