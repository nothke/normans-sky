using UnityEngine;
using System.Collections;

public class AdvancedCameraShake : MonoBehaviour
{

    new Transform camera;
    public Transform seatPosition;
    public Rigidbody targetRigidbody;

    public NoiseStep[] noiseSteps;

    public float masterMult = 1;

    public float accelerationMult = 0.1f;

    public float velocitySmoothing = 0.1f;

    public float forwardSmoothing = 0.1f;
    public float sidewaysSmoothing = 0.2f;
    public float verticalSmoothing = 0.05f;

    public float forwardMult = 1f;
    public float sidewaysMult = 1f;
    public float verticalMult = 1f;

    public float forwardRotationMult = 0;
    public float sidewaysRotationMult = 0;
    public float verticalRotationMult = 0;

    public Vector2 xLimits = new Vector2(-0.3f, 0.3f);
    public Vector2 yLimits = new Vector2(-0.3f, 0.3f);
    public Vector3 zLimits = new Vector3(-0.3f, 0.3f);


    Vector3 velocity;
    Vector3 lastVelocity;
    Vector3 smoothVelocity;

    public bool iterativeVelocity;

    [System.Serializable]
    public class NoiseStep
    {
        public float scale;
        public float height;
        public enum Usage { Idle, Speed, SurfaceType };
        public Usage usage;

        public float min;
        public float max;
    }



    void Start()
    {
        //camera = Main.camera.transform;
        camera = Camera.main.transform;
    }

    void FixedUpdate()
    {
        Vector3 camPos = new Vector3();
        Vector3 camRot = new Vector3();

        float xPerlin = 0;
        float yPerlin = 0;

        foreach (NoiseStep noiseStep in noiseSteps)
        {
            xPerlin = (-0.5f + Mathf.PerlinNoise(Time.time * noiseStep.scale, 0)) * noiseStep.height;
            yPerlin = (-0.5f + Mathf.PerlinNoise(0, Time.time * noiseStep.scale)) * noiseStep.height;

            if (noiseStep.usage == NoiseStep.Usage.Speed)
            {
                float velocityMult = Mathf.Clamp01((velocity.magnitude - noiseStep.min) / (noiseStep.max - noiseStep.min));
                //Debug.Log(velocityMult);

                xPerlin *= velocityMult * 0.1f; // velocityMult;
                yPerlin *= velocityMult;
            }

            camPos.x += xPerlin;
            camPos.y += yPerlin;
        }

        // physics effects

        Vector3 camPosWorld = Vector3.zero;

        if (targetRigidbody)
        {
            if (!iterativeVelocity)
            {
                velocity = targetRigidbody.GetRelativePointVelocity(seatPosition.localPosition);
                velocity = targetRigidbody.transform.InverseTransformDirection(velocity);
                //Debug.Log(velocity);
            }

            //velocity = Vector3.SmoothDamp(lastVelocity, velocity, ref smoothVelocity, velocitySmoothing);

            velocity.x = Mathf.SmoothDamp(lastVelocity.x, velocity.x, ref smoothVelocity.x, sidewaysSmoothing);
            velocity.y = Mathf.SmoothDamp(lastVelocity.y, velocity.y, ref smoothVelocity.y, verticalSmoothing);
            velocity.z = Mathf.SmoothDamp(lastVelocity.z, velocity.z, ref smoothVelocity.z, forwardSmoothing);

            Vector3 difference = lastVelocity - velocity;
            Vector3 acceleration = difference / Time.deltaTime;

            lastVelocity = velocity;

            Vector3 rotationVector = Vector3.forward * acceleration.x * forwardRotationMult +
                Vector3.up * acceleration.y * verticalRotationMult +
                Vector3.left * acceleration.z * sidewaysRotationMult;

            Vector3 linearVector = Vector3.forward * acceleration.z * forwardMult +
                Vector3.up * acceleration.y * verticalMult +
                Vector3.left * acceleration.x * sidewaysMult;

            //camPosWorld += acceleration * accelerationMult;

            camPos += linearVector * accelerationMult;

            camRot += rotationVector;
        }

        // clamp position to limits so the camera doesn't clip the car
        //camPos = Clamp(camPos, xLimits, yLimits, zLimits);

        // apply data to camera
        camera.localPosition = camPos * masterMult;
        //camera.position += camPosWorld;

        camera.localPosition = Clamp(camera.localPosition, xLimits, yLimits, zLimits);
        camera.localEulerAngles = camRot * masterMult;
    }

    Vector3 Clamp(Vector3 v, Vector2 xRange, Vector2 yRange, Vector2 zRange)
    {
        v.x = Mathf.Clamp(v.x, xRange.x, xRange.y);
        v.y = Mathf.Clamp(v.y, yRange.x, yRange.y);
        v.z = Mathf.Clamp(v.z, zRange.x, zRange.y);

        return v;
    }

}