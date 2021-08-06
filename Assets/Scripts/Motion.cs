
// if your project doesn't use these features, just comment out the defines:
#define TimedTrailRenderer

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using SkyUtils;

public class Motion : MonoBehaviour
{
    public static Motion e;

    public List<Transform> chunks = new List<Transform>();
    public float shiftRange = 10;
    public float translateForce = 100000;
    private float force;
    public float jumpForce = 1000;
    public float rotationForce = 10;

    private float originalAngularDrag;
    private float originalDrag;

    public PlanetEntity currentPlanet;

    public ParticleSystem speedticle;

    public Vector3d curRealPosition;
    public Vector3d realPosition;

    public int curSectorX;
    public int curSectorY;
    public float sectorSize = 5000;

    void Awake() { e = this; }

    Rigidbody rb;

    public float velocity;
    public float altitude;

    public static void AddChunk(Transform chunk)
    {
        if (e)
            e.chunks.Add(chunk);
    }

    public static void RemoveChunk(Transform chunk)
    {
        if (e)
            e.chunks.Remove(chunk);
    }

    void Start()
    {
        ShiftOrigin();

        originalAngularDrag = GetComponent<Rigidbody>().angularDrag;
        originalDrag = GetComponent<Rigidbody>().drag;

        rb = GetComponent<Rigidbody>();
    }

    void ShiftOrigin()
    {
        // Add current position to realPosition
        if (realPosition == Vector3d.zero)
        {
            realPosition = new Vector3d(transform.position);
        }
        else
            realPosition += transform.position;

        Vector3 relativePosition = transform.position;

        //Debug.Log("Real position: " + realPosition.x + ", " + realPosition.y + ", scene position: " + transform.position);

        foreach (Transform chunk in chunks)
        {
            if (chunk)
                chunk.position -= relativePosition;
            else
                Debug.LogWarning("Chunk is missing");
        }

        transform.position -= relativePosition;

#if TimedTrailRenderer
        ShiftTrailVertices(-relativePosition);
#endif
    }

    public Camera worldCamera;

    public float airDensity;
    float aeroFactor;

    public AudioSource rcsAudio;
    public AudioSource mainEngineAudio;
    public AudioSource hyperAudio;

    public float rcsAudioVelo;
    public float rcsAudioSmooth;

    public float mainEngineVelo;
    public float hyperVelo;


    void Update()
    {
        velocity = rb.velocity.magnitude;
        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);

        altitude = 100000;

        if (currentPlanet)
            altitude = Vector3.Distance(transform.position, currentPlanet.transform.position) - currentPlanet.radius;

        if (currentPlanet && currentPlanet.atmosphere)
        {
            if (worldCamera)
            {
                if (altitude < currentPlanet.atmosphereHeight)
                {
                    Vector3 normalDir = Vector3.Normalize(transform.position - currentPlanet.transform.position);

                    float ForwardSpeed = Mathf.Max(0, localVelocity.z);

                    // "Aerodynamic" calculations. This is a very simple approximation of the effect that a plane
                    // will naturally try to align itself in the direction that it's facing when moving at speed.
                    // Without this, the plane would behave a bit like the asteroids spaceship!
                    if (velocity > 0)
                    {
                        // compare the direction we're pointing with the direction we're moving:
                        aeroFactor = Vector3.Dot(transform.forward, rb.velocity.normalized);
                        // multipled by itself results in a desirable rolloff curve of the effect
                        aeroFactor *= aeroFactor;

                        float aerodynamicEffect = 0.5f;

                        aerodynamicEffect *= airDensity;

                        // Finally we calculate a new velocity by bending the current velocity direction towards
                        // the the direction the plane is facing, by an amount based on this aeroFactor
                        var newVelocity = Vector3.Lerp(rb.velocity, transform.forward * ForwardSpeed,
                                                       aeroFactor * ForwardSpeed * aerodynamicEffect * Time.deltaTime);
                        rb.velocity = newVelocity;

                        // also rotate the plane towards the direction of movement - this should be a very small effect, but means the plane ends up
                        // pointing downwards in a stall
                        //rb.rotation = Quaternion.Slerp(rb.rotation,
                        //Quaternion.LookRotation(rb.velocity, normalDir),
                        //aerodynamicEffect * Time.deltaTime);
                    }

                    RenderSettings.fog = true;
                    RenderSettings.fogColor = currentPlanet.atmosphereColor;

                    float percent = 1 - ((altitude / currentPlanet.atmosphereHeight));
                    airDensity = percent;

                    worldCamera.backgroundColor = Color.Lerp(Color.black, currentPlanet.atmosphereColor, percent);
                    RenderSettings.fogDensity = (percent * percent) / 100;
                }
                else
                {
                    worldCamera.backgroundColor = Color.black;
                    RenderSettings.fog = false;
                    airDensity = 0;
                }
            }
        }
        else RenderSettings.fog = false;

        // Movement

        float hyperTarget = 0;

        float speedFactor = Mathf.Clamp(velocity / 10, 0, 1);
        //Debug.Log(speedFactor);

        if (Input.GetKey(KeyCode.LeftControl))
        {
            rb.angularDrag = 5;

            if (airDensity == 0)
            {
                force = translateForce * 10;

                if (Input.GetAxis("Forward") > 0)
                    hyperTarget = 1;
            }
        }
        else
        {
            float airFactor = Mathf.Clamp(1 - airDensity, 0.25f, 1);


            rb.angularDrag = originalAngularDrag * (1 + airDensity) * (1 + speedFactor);


            force = translateForce * airFactor;

            //Debug.Log(airFactor);
        }



        if (Input.GetKey(KeyCode.Space))
            GetComponent<Rigidbody>().drag = 1;
        else
            GetComponent<Rigidbody>().drag = originalDrag;

        GetComponent<Rigidbody>().AddForce(Input.GetAxis("Vertical") * transform.up * Time.deltaTime * force * (1 + airDensity * 2));
        GetComponent<Rigidbody>().AddForce(Input.GetAxis("Horizontal") * transform.right * Time.deltaTime * force);
        GetComponent<Rigidbody>().AddForce(Input.GetAxis("Forward") * transform.forward * Time.deltaTime * force);



        float torqueMult = rotationForce * (1 + 5 * airDensity) * (1 + speedFactor) * Time.deltaTime;
        //Debug.Log(torqueMult);

        GetComponent<Rigidbody>().AddTorque(Input.GetAxis("Roll") * transform.forward * torqueMult, ForceMode.Acceleration);
        GetComponent<Rigidbody>().AddTorque(Input.GetAxis("Yaw") * transform.up * torqueMult, ForceMode.Acceleration);
        GetComponent<Rigidbody>().AddTorque(Input.GetAxis("Pitch") * transform.right * torqueMult, ForceMode.Acceleration);

        float rcsTarget = 0;
        float mainEngineTarget = 0;


        if (Input.GetAxis("Roll") != 0 || Input.GetAxis("Yaw") != 0 || Input.GetAxis("Pitch") != 0 || Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            //rb.angularDrag = 1;

            if (airDensity == 0)
                rcsTarget = 1;
        }


        if (Input.GetAxis("Forward") != 0)
            mainEngineTarget = 1;

        rcsAudio.volume = Mathf.SmoothDamp(rcsAudio.volume, rcsTarget, ref rcsAudioVelo, 0.1f);
        mainEngineAudio.volume = Mathf.SmoothDamp(mainEngineAudio.volume, mainEngineTarget, ref mainEngineVelo, 0.1f);
        hyperAudio.volume = Mathf.SmoothDamp(hyperAudio.volume, hyperTarget, ref hyperVelo, 0.1f);

        if (airDensity < 0.5f)
            speedticle.SetEmissionRate(Mathf.Clamp(velocity - 40, 0, 200));
        else
            speedticle.SetEmissionRate(0);

        //DebugAxes();




        UpdateShift();

        //Output();
    }

    void UpdateShift()
    {
        curRealPosition = realPosition + transform.position;

        if (transform.position.x > shiftRange ||
            transform.position.z > shiftRange ||
            transform.position.x < -shiftRange ||
            transform.position.z < -shiftRange)
        {
            ShiftOrigin();
        }
    }


#if TimedTrailRenderer
    public TimedTrailRenderer[] trails;

    void ShiftTrailVertices(Vector3 by)
    {
        foreach (var trail in trails)
        {
            if (trail.points.Count == 0)
                return;

            foreach (var point in trail.points)
            {
                TimedTrailRenderer.Point p = point as TimedTrailRenderer.Point;
                p.position += by;
            }
        }
    }
#endif

    #region unused

    void DebugAxes()
    {
        Debug.Log(Input.GetAxis("Vertical") + ", " +
                  Input.GetAxis("Horizontal") + ", " +
                  Input.GetAxis("Forward") + ", " +
                  Input.GetAxis("Roll") + ", " +
                  Input.GetAxis("Yaw") + ", " +
                  Input.GetAxis("Pitch"));
    }

    public bool debugGUI = true;

    public GUIStyle guiStyle;
    public float guiH = 20;

    void OnGUI()
    {
        float h = guiH;

        GUILayout.BeginArea(new Rect(5, 5, 1000, 1000), guiStyle);

        GUILayout.Label("real x: " + curRealPosition.x.ToString("F2"), guiStyle);
        GUILayout.Label("real y: " + curRealPosition.y.ToString("F2"), guiStyle);
        GUILayout.Label("real z: " + curRealPosition.z.ToString("F2"), guiStyle);

        GUILayout.Space(5);

        if (SectorUniverse.e != null)
        {
            SectorUniverse su = SectorUniverse.e;

            GUILayout.Label("Sector " + su.currentSector.x + ", " + su.currentSector.y + ", " + su.currentSector.z, guiStyle);
        }

        GUILayout.Space(5);

        if (currentPlanet)
        {
            GUILayout.Label("Body: " + currentPlanet.name, guiStyle);

            //string type = currentPlanet
            //GUILayout.Label("Type: " + currentPlanet.name, guiStyle);


            GUILayout.Label("radius: " + currentPlanet.radius, guiStyle);
            GUILayout.Label("atmosH: " + currentPlanet.atmosphereHeight, guiStyle);
            GUILayout.Label("gravF:  " + currentPlanet.gravity.force.ToString("F2"), guiStyle);
            GUILayout.Label("airDns: " + airDensity.ToString("F2"), guiStyle);
        }
        else
            GUILayout.Label("Body: None in range", guiStyle);

        GUILayout.EndArea();
    }

    void Output()
    {
        /*
        if (xText) xText.text = realPosition.x.ToString();
        if (yText) yText.text = transform.position.y.ToString();
        if (zText) zText.text = realPosition.y.ToString();

        if (sxText) sxText.text = transform.position.x.ToString();
        if (syText) syText.text = transform.position.y.ToString();
        if (szText) szText.text = transform.position.z.ToString();

        if (speedText) speedText.text = velocity.ToString();
        */
    }
    #endregion
}
