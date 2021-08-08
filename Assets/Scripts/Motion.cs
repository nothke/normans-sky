
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
    public float shiftRange = 1000;
    public float translateForce = 100000;
    public float jumpForce = 1000;
    public float rotationForce = 10;

    private float originalAngularDrag;
    private float originalDrag;

    public PlanetEntity currentPlanet;

    public ParticleSystem speedticle;

    public Vector3d curRealPosition;
    public Vector3d originOffset;

    public int curSectorX;
    public int curSectorY;
    public float sectorSize = 5000;

    void Awake() { e = this; }

    Rigidbody rb;

    public float velocity;
    public float altitude;

    public Camera worldCamera;

    public float airDensity;
    float aeroFactor;

    public AudioSource rcsAudio;
    public AudioSource mainEngineAudio;
    public AudioSource hyperAudio;

    // Audio
    float rcsAudioVelo;
    float mainEngineVelo;
    float hyperVelo;

    // Input
    Vector3 forceInput;
    Vector3 torqueInput;
    bool hyperInput;
    bool brakeInput;

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
        if (originOffset == Vector3d.zero)
        {
            originOffset = new Vector3d(transform.position);
        }
        else
            originOffset += transform.position;

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

        SectorUniverse.e.offset -= relativePosition;
    }

    private void Update()
    {
        forceInput = new Vector3(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical"),
            Input.GetAxis("Forward"));

        torqueInput = new Vector3(
            Input.GetAxis("Pitch"),
            Input.GetAxis("Yaw"),
            Input.GetAxis("Roll"));

        hyperInput = Input.GetKey(KeyCode.LeftControl);
        brakeInput = Input.GetKey(KeyCode.Space);
    }

    void FixedUpdate()
    {
        velocity = rb.velocity.magnitude;
        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);

        Vector3 pos = transform.position;

        altitude = !currentPlanet ? Mathf.Infinity :
            Vector3.Distance(pos, currentPlanet.transform.position) - currentPlanet.radius;

        airDensity = !currentPlanet ? 0 : 1 - (altitude / currentPlanet.atmosphereHeight);

        float forwardSpeed = Mathf.Max(0, localVelocity.z);

        if (currentPlanet && currentPlanet.atmosphere && airDensity > 0 && velocity > 0)
        {
            aeroFactor = Vector3.Dot(transform.forward, rb.velocity.normalized);
            aeroFactor *= aeroFactor;

            float aerodynamicEffect = 0.5f;

            aerodynamicEffect *= airDensity;

            var newVelocity = Vector3.Lerp(
                rb.velocity,
                transform.forward * forwardSpeed,
                aeroFactor * forwardSpeed * aerodynamicEffect * Time.deltaTime);

            rb.velocity = newVelocity;
        }

        // Fog effects
        if (worldCamera)
        {
            if (airDensity > 0)
            {
                RenderSettings.fog = true;
                RenderSettings.fogColor = currentPlanet.atmosphereColor;

                worldCamera.backgroundColor = Color.Lerp(Color.black, currentPlanet.atmosphereColor, airDensity);
                RenderSettings.fogDensity = (airDensity * airDensity) / 100;
            }
            else
            {
                worldCamera.backgroundColor = Color.black;
                RenderSettings.fog = false;
                airDensity = 0;
            }
        }

        else RenderSettings.fog = false;

        // Movement

        float hyperTarget = 0;

        float speedFactor = Mathf.Clamp(velocity / 10, 0, 1);
        //Debug.Log(speedFactor);

        float forceMult = 0;
        if (hyperInput)
        {
            rb.angularDrag = 5;

            if (airDensity == 0)
            {
                forceMult = translateForce * 10;

                if (forceInput.z > 0)
                    hyperTarget = 1;
            }
        }
        else
        {
            float airFactor = Mathf.Clamp(1 - airDensity, 0.25f, 1);
            rb.angularDrag = originalAngularDrag * (1 + airDensity) * (1 + speedFactor);
            forceMult = translateForce * airFactor;
        }

        // Airbrake / spacebrake
        rb.drag = brakeInput ? 1 : originalDrag;

        Vector3 relForce = forceInput;
        relForce.y *= 1 + airDensity * 2;

        forceMult /= 60.0f;
        rb.AddRelativeForce(relForce * forceMult);

        float torqueMult = rotationForce * (1 + 5 * airDensity) * (1 + speedFactor) / 60.0f;

        rb.AddRelativeTorque(torqueInput * torqueMult, ForceMode.Acceleration);

        UpdateShift();

        // Audio

        float rcsTarget = 0;
        float mainEngineTarget = 0;

        if (torqueInput != Vector3.zero ||
            forceInput != Vector3.zero)
        {
            if (airDensity == 0)
                rcsTarget = 1;
        }

        if (forceInput.z != 0)
            mainEngineTarget = 1;

        rcsAudio.volume = Mathf.SmoothDamp(rcsAudio.volume, rcsTarget, ref rcsAudioVelo, 0.1f);
        mainEngineAudio.volume = Mathf.SmoothDamp(mainEngineAudio.volume, mainEngineTarget, ref mainEngineVelo, 0.1f);
        hyperAudio.volume = Mathf.SmoothDamp(hyperAudio.volume, hyperTarget, ref hyperVelo, 0.1f);

        if (airDensity < 0.5f)
            speedticle.SetEmissionRate(Mathf.Clamp(velocity - 40, 0, 200));
        else
            speedticle.SetEmissionRate(0);
    }

    void UpdateShift()
    {
        curRealPosition = originOffset + transform.position;

        if (transform.position.x > shiftRange ||
            transform.position.z > shiftRange ||
            transform.position.x < -shiftRange ||
            transform.position.z < -shiftRange)
        {
            ShiftOrigin();
        }

        float sep = SectorUniverse.e.sectorSeparation;

        Vector3 normalizedSectorPos = new Vector3(
            (float)curRealPosition.x / sep,
            (float)curRealPosition.y / sep,
            (float)curRealPosition.z / sep);

        normalizedSectorPos -= SectorUniverse.e.currentSector;

        //Debug.Log(normalizedSectorPos);

        const float extent = 0.5f;

        if (normalizedSectorPos.x < -extent)
            SectorUniverse.e.Move(-1, 0, 0, true);
        else if (normalizedSectorPos.x > extent)
            SectorUniverse.e.Move(1, 0, 0, true);

        if (normalizedSectorPos.z < -extent)
            SectorUniverse.e.Move(0, 0, -1, true);
        else if (normalizedSectorPos.z > extent)
            SectorUniverse.e.Move(0, 0, 1, true);

        if (normalizedSectorPos.y < -extent)
            SectorUniverse.e.Move(0, -1, 0, true);
        else if (normalizedSectorPos.y > extent)
            SectorUniverse.e.Move(0, 1, 0, true);
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
        if (!debugGUI)
            return;

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

    #endregion
}
