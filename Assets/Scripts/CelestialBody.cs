using UnityEngine;
using System.Collections;

public class CelestialBody : MonoBehaviour
{
    new public string name = "Unnamed";

    public static float worldScale = 0.1f;
    public static float baseRadius = 10;

    public float radius = 30; // m
    public float density = 5500; // kg / m3
    public float mass;

    public Transform body;

    [HideInInspector]
    public CelestialGravity gravity;

    public void Start()
    {
        if (gameObject.GetComponent<CelestialGravity>())
            gravity = gameObject.GetComponent<CelestialGravity>();
        else
            gravity = gameObject.AddComponent<CelestialGravity>();

        OnGenerate();
    }

    public virtual void OnGenerate()
    {

    }

    public virtual void OnValidate()
    {
        mass = CalculateMass(radius, density);

        UpdateRadii();
    }

    public static float CalculateMass(float radius, float density)
    {
        radius *= worldScale;
        float volume = 1.33333f * Mathf.PI * radius * radius * radius;

        return density * volume;
    }

    public virtual void UpdateRadii()
    {
        if (body)
            body.localScale = Vector3.one * radius / baseRadius;
    }
}
