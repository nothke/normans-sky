///
/// Makes STUFF RANDOMLY SHAKE
/// by Nothke
/// 
/// what is this?:
/// uses perlin noise to shake stuff around it's local origin
/// mostly used for cameras, but anything else will do too
/// it can also use read from a RIGIDBODY to simulate head movement in a car eg.
/// or use speed of a rigidbody to add more random shake
/// 
/// 
/// how to:
/// attach this to an object and parent it to a "root", it will shake around
/// add noise steps for random shakes, or set up physics for motion
/// you can easily modify masterMult and masterScale from other scripts to increase shake effect
/// 
/// it's pretty much still WIP!



using UnityEngine;

public class Shake : MonoBehaviour
{

    public Transform t; // if none set, it will use the transform from this gameObject

    public NoiseStep[] noiseSteps;

    public float masterMult = 1;
    public float masterScale = 1;

    [System.Serializable]
    public class PhysicsSettings
    {
        public Rigidbody targetRigidbody; // read speed and acceleration from this rigidbody for reactive effects

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

        //public Transform seatPosition;
    }

    public PhysicsSettings physics;

    [System.Serializable]
    public class Limits // limits can be used to clamp the movement, so that for eg. head doesn't go out of the cockpit
    {
        public bool bypass; // if true limits won't be used
        public Vector2 x = new Vector2(-0.3f, 0.3f);
        public Vector2 y = new Vector2(-0.3f, 0.3f);
        public Vector3 z = new Vector3(-0.3f, 0.3f);
    }

    public Limits limits;

    Vector3 velocity;
    Vector3 lastVelocity;
    Vector3 smoothVelocity;

    public bool iterativeVelocity;

    [System.Serializable]
    public class NoiseStep
    {
        public bool enabled = true;
        public float scale;
        public float height;
        public enum Usage { Idle, Speed, SurfaceType };

        public bool rotation;

        public Usage usage;

        public float min;
        public float max;
        
        [HideInInspector]
        public float seekTime;
    }

    Transform pivot;

    void Start()
    {
        if (t == null)
            t = transform;

        pivot = transform.parent;
    }
    

    void FixedUpdate()
    {
        Vector3 tPos = new Vector3();
        Vector3 tRot = new Vector3();

        float xPerlin = 0;
        float yPerlin = 0;

        foreach (NoiseStep noiseStep in noiseSteps)
        {
            if (!noiseStep.enabled)
                continue;

            noiseStep.seekTime += Time.fixedDeltaTime * noiseStep.scale * masterScale;

            xPerlin = (-0.5f + Mathf.PerlinNoise(1000 + noiseStep.seekTime, 0)) * noiseStep.height * masterMult;
            yPerlin = (-0.5f + Mathf.PerlinNoise(0, 1000 + noiseStep.seekTime)) * noiseStep.height * masterMult;

            #region physics
            if (noiseStep.usage == NoiseStep.Usage.Speed)
            {
                float velocityMult = Mathf.Clamp01((velocity.magnitude - noiseStep.min) / (noiseStep.max - noiseStep.min));

                xPerlin *= velocityMult * 0.1f; // velocityMult;
                yPerlin *= velocityMult;
            }
            #endregion

            if (!noiseStep.rotation)
            {
                tPos.x += xPerlin;
                tPos.y += yPerlin;
            }
            else
            {
                tRot.z += xPerlin;
            }
        }

        #region physics
        // physics effects

        //Vector3 camPosWorld = Vector3.zero;

        Rigidbody tRb = physics.targetRigidbody;

        if (tRb)
        {
            if (!iterativeVelocity)
            {
                velocity = tRb.GetRelativePointVelocity(pivot.localPosition);
                velocity = tRb.transform.InverseTransformDirection(velocity);
            }

            //velocity = Vector3.SmoothDamp(lastVelocity, velocity, ref smoothVelocity, velocitySmoothing);

            velocity.x = Mathf.SmoothDamp(lastVelocity.x, velocity.x, ref smoothVelocity.x, physics.sidewaysSmoothing);
            velocity.y = Mathf.SmoothDamp(lastVelocity.y, velocity.y, ref smoothVelocity.y, physics.verticalSmoothing);
            velocity.z = Mathf.SmoothDamp(lastVelocity.z, velocity.z, ref smoothVelocity.z, physics.forwardSmoothing);

            Vector3 difference = lastVelocity - velocity;
            Vector3 acceleration = difference / Time.deltaTime;

            lastVelocity = velocity;

            //Quaternion rotationQ = 

            Vector3 rotationVector = Vector3.forward * acceleration.x * physics.forwardRotationMult +
                Vector3.up * acceleration.y * physics.verticalRotationMult +
                Vector3.left * acceleration.z * physics.sidewaysRotationMult;

            Vector3 linearVector = Vector3.forward * acceleration.z * physics.forwardMult +
                Vector3.up * acceleration.y * physics.verticalMult +
                Vector3.left * acceleration.x * physics.sidewaysMult;

            //camPosWorld += acceleration * accelerationMult;

            tPos += linearVector * physics.accelerationMult;

            tRot += rotationVector;
        }
        #endregion

        // apply data to transform
        t.localPosition = tPos;

        // clamp to limits
        if (!limits.bypass)
            t.localPosition = Clamp(t.localPosition, limits.x, limits.y, limits.z);

        t.localEulerAngles = tRot * masterMult;
    }

    Vector3 Clamp(Vector3 v, Vector2 xRange, Vector2 yRange, Vector2 zRange)
    {
        v.x = Mathf.Clamp(v.x, xRange.x, xRange.y);
        v.y = Mathf.Clamp(v.y, yRange.x, yRange.y);
        v.z = Mathf.Clamp(v.z, zRange.x, zRange.y);

        return v;
    }

}