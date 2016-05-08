using UnityEngine;
using System.Collections;

public static class ParticleUtils
{

    public static void SetEmissionRate(this ParticleSystem particleSystem, float rate)
    {
        ParticleSystem.EmissionModule em = particleSystem.emission;
        ParticleSystem.MinMaxCurve rCurve = new ParticleSystem.MinMaxCurve(rate);
        em.rate = rCurve;
    }

    // needs testing
    public static void SetShapeAngle(this ParticleSystem particleSystem, float angle)
    {
        ParticleSystem.ShapeModule shape = particleSystem.shape;
        shape.angle = angle;
    }


}
