
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

    public LayerMask worldLayer;

    public List<Transform> chunks = new List<Transform>();
    public float shiftRange = 10;
    public float translateForce = 100000;
    private float force;
    public float jumpForce = 1000;
    public float rotationForce = 10;


    /*
    public Text speedText;
    public Text xText;
    public Text yText;
    public Text zText;
    public Text sxText;
    public Text syText;
    public Text szText;
    */

    private float originalAngularDrag;
    private float originalDrag;

    public CelestialBody currentBody;

    public ParticleSystem speedticle;
    public ParticleSystem rainParticles;

    /*
    public struct PositionDouble
    {
        public double x, y;

        public PositionDouble(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public PositionDouble(Vector3 v3)
        {
            this.x = v3.x;
            this.y = v3.z;
        }

        public void AddPosition(Vector3 v3)
        {
            this.x += v3.x;
            this.y += v3.z;
        }
    }*/

    //public PositionDouble realPosition;

    public Vector3d curRealPosition;
    public Vector3d realPosition;

    public int curSectorX;
    public int curSectorY;
    public float sectorSize = 5000;

    /*
    GameObject[] GetAllObjects()
    {
        return GameObject.FindObjectOfType(typeof(GameObject)) as GameObject[];
    }*/

    void Awake() { e = this; }

    public Rigidbody rb;

    public float velocity;
    public float altitude;

    void Start()
    {
        ShiftOrigin();

        originalAngularDrag = GetComponent<Rigidbody>().angularDrag;
        originalDrag = GetComponent<Rigidbody>().drag;

        rb = GetComponent<Rigidbody>();
        CelestialGravity.sceneRigidbodies.Add(rb);
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

    public void GetAllObjects()
    {
        GameObject[] gos = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];

        Transform[] transforms = Transform.FindObjectsOfType(typeof(Transform)) as Transform[];
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

    float bodySqrDistance;
    float bodyVSpeed;
    float bodyHSpeed;

    void Update()
    {

        


        velocity = rb.velocity.magnitude;
        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);


        if (CockpitMenu.e)
        {
            Ray proxRay = new Ray(transform.position, rb.velocity);

            Debug.DrawRay(proxRay.origin, rb.velocity);

            if (velocity > 10 && Physics.Raycast(proxRay, velocity * 2, worldLayer))
            {
                CockpitMenu.e.DisplayWarning("PROX");
            }
            else CockpitMenu.e.EndWarning("PROX");
        }

        altitude = 100000;



        SetClosestBody();

        if (currentBody)
        {
            altitude = Vector3.Distance(transform.position, currentBody.transform.position) - currentBody.radius;
            Vector3 bodyNormal = currentBody.transform.position - transform.position;
            bodySqrDistance = bodyNormal.sqrMagnitude;
            bodyVSpeed = Vector3.Dot(bodyNormal.normalized, rb.velocity);
            bodyHSpeed = Vector3.ProjectOnPlane(rb.velocity, bodyNormal).magnitude;
        }

        PlanetEntity currentPlanet = currentBody as PlanetEntity;

        if (currentPlanet && currentPlanet.atmosphere)
        {
            if (worldCamera)
            {
                //float altitude = Vector3.Distance(transform.position, currentPlanet.transform.position) - currentPlanet.radius;

                rainParticles.transform.parent.LookAt(currentPlanet.transform.position);





                if (altitude < currentPlanet.atmosphereHeight)
                {
                    Vector3 normalDir = Vector3.Normalize(transform.position - currentBody.transform.position);

                    //var localVelocity = transform.InverseTransformDirection(rb.velocity);
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

                    rainParticles.SetEmissionRate(airDensity * 4000);

                    worldCamera.backgroundColor = Color.Lerp(Color.black, currentPlanet.atmosphereColor, percent);
                    RenderSettings.fogDensity = (percent * percent) / 100;

                    //Debug.Log(altitude + " " + altitude / currentPlanet.atmosphereHeight);

                    //rb.drag = airDensity;
                }
                else
                {
                    worldCamera.backgroundColor = Color.black;
                    RenderSettings.fog = false;
                    airDensity = 0;
                    rainParticles.SetEmissionRate(0);
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

                if (Input.GetKey(KeyCode.LeftShift))
                    force = translateForce * 100;

                if (Input.GetAxis("Forward") > 0)
                    hyperTarget = 1;
            }
        }
        else
        {
            float airFactor = Mathf.Clamp(1 - airDensity, 0.25f, 1);
            rb.angularDrag = originalAngularDrag * (1 + airDensity) * (1 + speedFactor);
            force = translateForce * airFactor;
        }


        if (Input.GetKey(KeyCode.Space))
            rb.drag = 1;
        else
            rb.drag = originalDrag;

        Vector3 trnInput = new Vector3(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), Input.GetAxis("Forward"));
        Vector3 rotInput = new Vector3(Input.GetAxis("Pitch"), Input.GetAxis("Roll"), Input.GetAxis("Yaw"));

        if (ShipSystems.e.totalEnginePower > 0)
        {

            float forceMult = force * Time.deltaTime;
            float upExtraMult = 1 + airDensity * 2;

            rb.AddForce(trnInput.x * transform.up * forceMult * upExtraMult);
            rb.AddForce(trnInput.y * transform.right * forceMult);
            rb.AddForce(trnInput.z * transform.forward * forceMult);

            float torqueMult = rotationForce * (1 + 5 * airDensity) * (1 + speedFactor) * Time.deltaTime;

            rb.AddTorque(rotInput.x * transform.right * torqueMult, ForceMode.Acceleration);
            rb.AddTorque(rotInput.y * transform.forward * torqueMult, ForceMode.Acceleration);
            rb.AddTorque(rotInput.z * transform.up * torqueMult, ForceMode.Acceleration);


            float rcsTarget = 0;
            float mainEngineTarget = 0;

            ShipSystems.e.FireRCS(trnInput.z, trnInput.y, trnInput.x, rotInput.y, rotInput.x, rotInput.z);

            if (trnInput != Vector3.zero || rotInput != Vector3.zero)
            {
                if (airDensity == 0)
                    rcsTarget = 1;
            }

            float mainEngineThrottle = Mathf.Clamp01(Input.GetAxis("Forward"));

            ShipSystems.e.SetMainEngineTargetPower(mainEngineThrottle);

            if (Input.GetAxis("Forward") != 0)
                mainEngineTarget = 1;

            // engine audios
            rcsAudio.volume = Mathf.SmoothDamp(rcsAudio.volume, rcsTarget, ref rcsAudioVelo, 0.1f);
            mainEngineAudio.volume = Mathf.SmoothDamp(mainEngineAudio.volume, mainEngineTarget, ref mainEngineVelo, 0.1f);
            hyperAudio.volume = Mathf.SmoothDamp(hyperAudio.volume, hyperTarget, ref hyperVelo, 0.1f);
        }

        if (airDensity < 0.5f)
            speedticle.SetEmissionRate(Mathf.Clamp(velocity - 40, 0, 200));
        else
            speedticle.SetEmissionRate(0);





        UpdateShift();

        //DebugAxes();
        //Output();
    }

    public List<CelestialBody> closeBodies = new List<CelestialBody>();

    public void AddBody(CelestialBody p)
    {
        if (closeBodies.Contains(p)) return;
        else closeBodies.Add(p);
    }

    public void RemoveBody(CelestialBody p)
    {
        closeBodies.Remove(p);

        closeBodies.RemoveAll(item => item == null);
    }

    public void SetClosestBody()
    {
        float sqrDist = Mathf.Infinity;

        if (closeBodies.Count == 0)
        {
            currentBody = null;
        }
        else
        {
            foreach (var body in closeBodies)
            {
                float dist = (body.transform.position - transform.position).magnitude;

                if (dist < sqrDist)
                    currentBody = body;
            }
        }
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

            GUILayout.Label("Sector " + su.curSectorX + ", " + su.curSectorY + ", " + su.curSectorZ, guiStyle);
        }

        GUILayout.Space(5);

        if (currentBody)
        {
            GUILayout.Label("Body: " + currentBody.name, guiStyle);

            PlanetEntity currentPlanet = currentBody as PlanetEntity;

            if (currentPlanet != null)
            {
                string type = currentPlanet.type == PlanetEntity.Type.Rocky ? "Rocky Planet" : "Gas Giant";
                GUILayout.Label("Type: " + type, guiStyle);
            }
            else
            {
                GUILayout.Label("Type: Star", guiStyle);
            }

            //string type = currentPlanet
            //GUILayout.Label("Type: " + currentPlanet.name, guiStyle);

            GUILayout.Label("radius: " + currentBody.radius, guiStyle);
            if (currentPlanet != null) GUILayout.Label("atmosH: " + currentPlanet.atmosphereHeight.ToString("F2"), guiStyle);
            //GUILayout.Label("gravF:  " + currentBody.gravity.force.ToString("F2"), guiStyle);
            if (currentPlanet != null) GUILayout.Label("airDns: " + airDensity.ToString("F2"), guiStyle);
            GUILayout.Label("altitd: " + altitude.ToString("F2"), guiStyle);
            GUILayout.Label("distnc: " + Mathf.Sqrt(bodySqrDistance).ToString("F2"), guiStyle);
            GUILayout.Label("vSpeed: " + bodyVSpeed.ToString("F2"), guiStyle);
            GUILayout.Label("hSpeed: " + bodyHSpeed.ToString("F2"), guiStyle);
            float orbitHV = CelestialGravity.CalculateVelocityForCircularOrbit(1, bodySqrDistance); // TODO: change to earth masses
            GUILayout.Label("orbitV: " + orbitHV.ToString("F2"), guiStyle);

            float surfGrav = CelestialGravity.GetAcceleration(currentBody.mass, currentBody.radius * currentBody.radius);
            GUILayout.Label("srfAcc: " + surfGrav.ToString("F2"), guiStyle);
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
