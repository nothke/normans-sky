using UnityEngine;
using System.Collections;

public class PlanetEntity : CelestialBody
{
    public bool random;

    public bool atmosphere;
    public Color atmosphereColor;

    public float atmosphereHeight = 50;

    public Transform atmoSphereSphere;

    public enum Type { Rocky, GasGiant };
    public Type type = Type.Rocky;

    public bool rain;
    public bool thunder;
    public bool snow;

    void OnDrawGizmos()
    {
        if (atmosphere)
        {
            Gizmos.color = atmosphereColor;
            Gizmos.DrawWireSphere(transform.position, radius + atmosphereHeight);
        }
    }

    public override void OnGenerate()
    {
        base.OnGenerate();

        if (random)
        {
            atmosphere = Random.value > 0.5f;
            atmosphereColor = Random.ColorHSV(0, 1, 1, 1, 1, 1);
        }

        Material atmoMat = atmoSphereSphere.GetComponent<Renderer>().material;

        //atmoMat.SetColor("_RimColor", atmosphereColor);
        atmoMat.SetColor("_InnerColor", atmosphereColor * 0.1f);

        if (!atmosphere)
        {
            Destroy(atmoSphereSphere.gameObject);
            atmoSphereSphere = null;
        }
    }

    public override void OnValidate()
    {
        base.OnValidate();

        if (atmosphere)
            if (atmoSphereSphere != null)
                if (!atmoSphereSphere && atmoSphereSphere.gameObject != null)
                    atmoSphereSphere.localScale = Vector3.one * (radius + atmosphereHeight) * worldScale;
    }
}
