using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VectorExtensions;

public class SectorUniverse : MonoBehaviour
{
    public static SectorUniverse e;

    public int curSectorX;
    public int curSectorY;
    public int curSectorZ;

    public float sectorSeparation = 10;

    public bool preview = true;
    public bool previewSectors = true;
    public bool previewSystems = true;
    public bool previewPlanets = true;
    public bool drawOrbits = false;
    public bool previewNames = false;
    public Color orbitColor = Color.white;

    [Range(1, 10)]
    public int sectorRadius = 3;

    int sectorRange = 10;

    public float systemProbability = 0.5f;

    public float starPreviewRadius = 1;
    public float planetPreviewRadius = 0.5f;

    public bool skipZero = true;

    [Header("Simplex properties")]
    public int octaves = 3;
    public int multiplier = 25;
    public float amplitude = 1;
    public float lacunarity = 2;
    public float persistence = 0.9f;

    [Header("System properties")]
    public int maxPlanets = 10;
    public float minPlanetRange = 5;
    public float minPlanetSeparation = 5;
    public float nextPlanetPower = 2;
    public float maxPlanetRangeMult = 50;

    public float minPlanetRadius = 500;
    public float maxPlanetRadius = 2000;

    public float gasGiantThreshold = 1500;

    [Header("Generator Prefabs")]
    public bool generate = false;
    public bool generateRings;
    public GameObject starPrefab;
    public GameObject planetPrefab;


    SimplexNoiseGenerator simplex;

    public bool updateNames;
    public string[] namePrefixes;
    public string[] nameSuffixes;

    public bool initOnStart;

    void Awake()
    {
        e = this;
    }

    void Start()
    {
        if (initOnStart)
        {
            CreateAllSectors();

            GeneratePhysical();
        }
    }

    void UpdateNames()
    {
        string path = Application.dataPath + "/PlanetNames.txt";

        namePrefixes = SReader.GetLines(path, "PREFIXES");
        nameSuffixes = SReader.GetLines(path, "SUFFIXES");
    }

    public Gradient starColorGradient;
    public Gradient planetColorGradient;

    public class Sector
    {
        public Vector3i coordinate;

        public bool hasSystem;

        public string name;

        public Vector3 orbitNormal;

        public float starSize; // incomplete
        public Vector3 starPostion;
        public Color starColor;

        public float[] planetOrbits;
        public float[] planetRadii;
        public Vector3[] planetPositions;
        public Color[] planetColors;

        public StarEntity[] star;
        public StarEntity[] newStar;
        public bool flagDestroy;
    }

    void CreateSimplex()
    {
        if (simplex == null) simplex = new SimplexNoiseGenerator("42");
    }

    Sector[,,] sectors;

    int GetSectorRange()
    {
        return sectorRadius * 2 + 1;
    }

    public void CreateAllSectors()
    {
        sectorRange = GetSectorRange();

        sectors = new Sector[sectorRange, sectorRange, sectorRange];

        for (int x = 0; x < sectorRange; x++)
        {
            for (int y = 0; y < sectorRange; y++)
            {
                for (int z = 0; z < sectorRange; z++)
                {
                    sectors[x, y, z] = CreateNewSector(curSectorX - sectorRadius + x, curSectorY - sectorRadius + y, curSectorZ - sectorRadius + z);

                }
            }
        }
    }

    void Move(int byX, int byY, int byZ)
    {
        SmartMovePhysical(byX, byY, byZ);

        /*
        ClearAllSectors();
        CreateAllSectors();

        GeneratePhysical();
        */
    }

    bool CoordinateIsInsideBounds(Vector3i coord)
    {
        Vector3i currentSector = GetCurSector();

        if (coord >= currentSector - Vector3i.one * sectorRadius &&
            coord <= currentSector + Vector3i.one * sectorRadius)
            return true;

        return false;
    }

    void SmartMovePhysical(int byX, int byY, int byZ)
    {
        Vector3i by = new Vector3i(byX, byY, byZ);

        // PASS 1 - rearrange and flag
        foreach (var sector in sectors)
        {
            if (CoordinateIsInsideBounds(sector.coordinate - by))
            {
                // if next sector is inside bounds, transfer this system into the next sector..
                Sector nextSector = GetSectorFromWorldCoord(sector.coordinate - by);
                nextSector.newStar = sector.star;

                // ..and move it if it has any star
                ShiftSystem(nextSector.newStar, -by);
            }
            else
            {
                // else flag system for destruction
                sector.flagDestroy = true;
            }
        }

        // change current sector
        curSectorX += byX;
        curSectorY += byY;
        curSectorZ += byZ;

        // PASS 2 - destroy and construct
        foreach (var sector in sectors)
        {
            if (sector.flagDestroy)
            {
                DestroySystem(sector.star);

                sector.flagDestroy = false;
            }

            sector.star = null;

            // now set new coordinates
            sector.coordinate = sector.coordinate + by;

            // overwrite star with ones that are transfered
            if (sector.newStar != null)
                sector.star = sector.newStar;

            sector.newStar = null;

            // and recreate sector data
            CreateSector(sector);

            // if sector has no physical star, create new one
            if (sector.star == null)
                GeneratePhysical(sector);
        }
    }

    void ShiftSystem(StarEntity[] stars, Vector3i by)
    {
        if (stars == null) return;

        Vector3 moveBy = (Vector3)by * sectorSeparation;

        foreach (var star in stars)
        {
            star.transform.position += moveBy;

            foreach (var planet in star.planets)
                planet.transform.position += moveBy;
        }
    }

    Sector GetSectorFromWorldCoord(Vector3i coord)
    {
        Vector3i localCoord = coord + Vector3i.one * sectorRadius - GetCurSector();

        return sectors[localCoord.x, localCoord.y, localCoord.z];
    }

    void ClearAllPhysical()
    {
        float tTest = Time.realtimeSinceStartup; // TEST TIMER

        foreach (var sector in sectors)
        {
            if (!sector.hasSystem) continue;

            if (sector.star == null) continue;

            if (sector.star.Length == 0) continue;

            for (int i = 0; i < sector.star.Length; i++)
            {
                if (sector.star[i].planets.Length > 0) continue;

                for (int j = 0; j < sector.star[i].planets.Length; j++)
                {
                    Destroy(sector.star[i].planets[j].gameObject);
                }

                Destroy(sector.star[i].gameObject);
            }
        }

        Debug.Log("Clear all physical: " + (Time.realtimeSinceStartup - tTest)); // TEST TIMER

    }

    IEnumerator FrameAfter()
    {

        float tTest = Time.realtimeSinceStartup; // TEST TIMER

        yield return null;

        Debug.Log("Next frame: " + (Time.realtimeSinceStartup - tTest)); // TEST TIMER

    }

    public void GeneratePhysical()
    {
        if (!generate) return;

        if (!starPrefab || !planetPrefab) return;

        ClearAllPhysical();

        float tTest = Time.realtimeSinceStartup; // TEST TIMER

        foreach (var sector in sectors)
        {
            GeneratePhysical(sector);
        }

        Debug.Log("Generate physical: " + (Time.realtimeSinceStartup - tTest)); // TEST TIMER

        StartCoroutine(FrameAfter());
    }

    void GeneratePhysical(Sector sector)
    {
        if (sector.hasSystem)
        {
            Vector3 sectorStartPos = GetSectorStartPos(sector);

            GameObject starGO = Instantiate(starPrefab, sectorStartPos + sector.starPostion, Quaternion.identity) as GameObject;
            if (Motion.e) Motion.e.chunks.Add(starGO.transform);
            // material color?

            // if not a nebula generate only one star
            sector.star = new StarEntity[1];
            sector.star[0] = starGO.GetComponent<StarEntity>();

            if (sector.planetPositions.Length > 0)
            {
                sector.star[0].planets = new PlanetEntity[sector.planetPositions.Length];

                for (int i = 0; i < sector.planetPositions.Length; i++)
                {
                    GameObject planetGO = Instantiate(planetPrefab, sectorStartPos + sector.planetPositions[i], Quaternion.identity) as GameObject;
                    if (Motion.e) Motion.e.chunks.Add(planetGO.transform);

                    PlanetEntity planet = planetGO.GetComponent<PlanetEntity>();

                    planet.radius = sector.planetRadii[i];

                    if (planet.radius < gasGiantThreshold)
                    {
                        planet.type = PlanetEntity.Type.GasGiant;

                        if (generateRings)
                            if (Random.value < 0.5f)
                                planet.GetComponent<RingMaker>().enabled = true;
                    }

                    sector.star[0].planets[i] = planet;
                    if (!sector.star[0].planets[i]) Debug.LogWarning("PlanetEntity not found");
                }
            }
        }
    }

    void ClearAllSectors()
    {
        foreach (Sector sector in sectors)
        {
            if (!sector.hasSystem) continue;

            if (sector.star == null) Debug.LogWarning("Stars are null");

            foreach (var star in sector.star)
            {
                foreach (var planet in star.planets)
                {
                    if (planet)
                        Destroy(planet.gameObject);
                }

                Destroy(star.gameObject);
            }
        }
    }

    Sector CreateNewSector(int x, int y, int z)
    {
        CreateSimplex();

        Sector s = new Sector();

        s.coordinate = new Vector3i(x, y, z);

        CreateSector(s);

        return s;
    }

    void CreateSector(Sector s)
    {
        int x = s.coordinate.x;
        int y = s.coordinate.y;
        int z = s.coordinate.z;

        var randState = Random.state;

        float value = simplex.coherentNoise(x, y, z, octaves, multiplier, amplitude, lacunarity, persistence);
        value += 0.5f;

        Random.InitState(("ad" + x + "_" + y + "_" + z).GetHashCode());

        if (value < systemProbability) // has no solar system
            return;

        // has a solar system:
        if (skipZero && s.coordinate == Vector3i.zero)
            return;

        s.hasSystem = true;

        s.name = SReader.GetRandom(namePrefixes) + SReader.GetRandom(nameSuffixes);
        s.starPostion = new Vector3(Random.value, Random.value, Random.value) * sectorSeparation;

        s.starSize = (value - 0.5f) * 2;

        s.starColor = starColorGradient.Evaluate(s.starSize);

        s.orbitNormal = Random.insideUnitSphere.normalized;

        int planetsNum = Random.Range(0, maxPlanets);
        s.planetOrbits = new float[planetsNum];
        s.planetPositions = new Vector3[planetsNum];
        s.planetColors = new Color[planetsNum];
        s.planetRadii = new float[planetsNum];

        float orbitRadius = minPlanetRange;

        for (int i = 0; i < planetsNum; i++)
        {
            orbitRadius += Random.Range(minPlanetSeparation, minPlanetSeparation * (Mathf.Pow(i + 1, nextPlanetPower)));
            s.planetOrbits[i] = orbitRadius;

            Vector3 pos = RandomPointOnPlane(s.orbitNormal, orbitRadius);

            s.planetPositions[i] = s.starPostion + pos;
            s.planetRadii[i] = Random.Range(minPlanetRadius, maxPlanetRadius);
            s.planetColors[i] = planetColorGradient.Evaluate(Random.value);
        }

        Random.state = randState;
    }

    void DestroySystem(StarEntity[] stars)
    {
        if (stars == null) return;

        if (stars.Length == 0) return;

        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i].planets.Length > 0)
                for (int j = 0; j < stars[i].planets.Length; j++)
                {
                    //if (stars[i].planets[i]) // shouldn't happen
                    Destroy(stars[i].planets[j].gameObject);
                }

            Destroy(stars[i].gameObject);
        }
    }

    Vector3i GetCurSector()
    {
        return new Vector3i(curSectorX, curSectorY, curSectorZ);
    }


    private Vector3 RandomPointOnPlane(Vector3 normal, float radius)
    {
        Vector3 randomPoint;

        do
        {
            randomPoint = Vector3.Cross(Random.insideUnitSphere, normal);
        } while (randomPoint == Vector3.zero);

        randomPoint.Normalize();
        randomPoint *= radius;

        return randomPoint;
    }

    Vector3 GetSectorMidPos(Sector s)
    {
        return GetSectorMidPos(s.coordinate.x, s.coordinate.y, s.coordinate.z);
    }

    Vector3 GetSectorMidPos(int x, int y, int z)
    {
        Vector3 relativeSector = new Vector3(x - curSectorX, y - curSectorY, z - curSectorZ);
        //relativeSector -= Vector3.one * (0.5f * sectorRange - 0.5f); // OLD calc

        return relativeSector * sectorSeparation;
    }

    Vector3 GetSectorStartPos(Sector s)
    {
        return GetSectorStartPos(s.coordinate.x, s.coordinate.y, s.coordinate.z);
    }

    Vector3 GetSectorStartPos(int x, int y, int z)
    {
        return GetSectorMidPos(x, y, z) - (Vector3.one * sectorSeparation / 2);
    }

    int prevCurX, prevCurY, prevCurZ;

    void OnValidate()
    {
        CreateAllSectors();

        if (updateNames == true)
        {
            UpdateNames();
            updateNames = false;
        }

        /*
        if (curSectorX != prevCurX || curSectorY != prevCurY || curSectorZ != prevCurZ)
        {
            CreateAllSectors();

            prevCurX = curSectorX;
            prevCurY = curSectorY;
            prevCurZ = curSectorZ;
        }*/
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!preview)
            return;

        if (sectors == null)
            return;

        UnityEditor.Handles.color = orbitColor;

        foreach (var sector in sectors)
        {
            Gizmos.color = Color.gray;

            if (sector.hasSystem)
            {
                if (previewSectors)
                {
                    Gizmos.color = new Color(1, 1, 1, 0.1f);
                    Gizmos.DrawWireCube(GetSectorMidPos(sector), Vector3.one * sectorSeparation);
                }

                if (previewSystems)
                {
                    Gizmos.color = sector.starColor * 3;

                    Vector3 sectorStartPos = GetSectorStartPos(sector);

                    Gizmos.DrawSphere(sectorStartPos + sector.starPostion, starPreviewRadius);

                    if (previewNames)
                        UnityEditor.Handles.Label(sectorStartPos + sector.starPostion, sector.name);

                    if (previewPlanets && sector.planetPositions.Length > 0)
                    {
                        for (int i = 0; i < sector.planetPositions.Length; i++)
                        {
                            Gizmos.color = sector.planetColors[i];
                            Gizmos.DrawSphere(sectorStartPos + sector.planetPositions[i], sector.planetRadii[i]); // planetPreviewRadius

                            if (drawOrbits)
                            {
                                UnityEditor.Handles.DrawWireDisc(sectorStartPos + sector.starPostion, sector.orbitNormal, sector.planetOrbits[i]);
                            }
                        }
                    }
                }
            }
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one * sectorSeparation);
    }
#endif

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
            Move(0, 1, 0);
    }
#endif
}
