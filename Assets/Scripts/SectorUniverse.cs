using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Nothke.Collections;

public class SectorUniverse : MonoBehaviour
{
    public static SectorUniverse e;

    #region Inspector vars

    public Vector3Int currentSector;

    [Range(1, 10)]
    public int sectorRadius = 3;
    public float sectorSeparation = 10;

    [Header("Simplex properties")]
    public int octaves = 3;
    public int multiplier = 25;
    public float amplitude = 1;
    public float lacunarity = 2;
    public float persistence = 0.9f;

    [Header("System properties")]
    public float systemProbability = 0.5f;
    public int maxPlanets = 10;
    public float minPlanetRange = 5;
    public float minPlanetSeparation = 5;
    public float nextPlanetPower = 2;
    public float maxPlanetRangeMult = 50;

    public float minPlanetRadius = 500;
    public float maxPlanetRadius = 2000;

    public float gasGiantThreshold = 1500;

    public bool skipZero = true;

    public Gradient starColorGradient;
    public Gradient planetColorGradient;

    [Header("Generator Prefabs")]
    public bool generate = false;
    public bool generateRings;
    public GameObject starPrefab;
    public GameObject planetPrefab;

    public bool updateNames;
    public string[] namePrefixes;
    public string[] nameSuffixes;

    public bool initOnStart;

    [Header("Preview")]

    public bool preview = true;
    public bool previewSectors = true;
    public bool previewSystems = true;
    public bool previewPlanets = true;
    public bool drawOrbits = false;
    public bool previewNames = false;
    public Color orbitColor = Color.white;

    public float starPreviewRadius = 1;
    public float planetPreviewRadius = 0.5f;

    #endregion

    #region Private vars

    public class Sector
    {
        public Vector3Int coordinate;

        public bool hasSystem;

        public string name;

        public Vector3 orbitNormal;

        public float starSize; // incomplete
        public Vector3 starPostion;
        public Color starColor;

        public struct Planet
        {
            public float orbitRadius;
            public float radius;
            public Vector3 position;
            public Color color;
        }

        public List<Planet> planets;

        public StarEntity[] star;
        public StarEntity[] newStar;
        public bool flagDestroy;

        public Sector(int maxPlanets)
        {
            planets = new List<Planet>(maxPlanets);
        }
    }

    SimplexNoiseGenerator simplex;
    int sectorRange = -1;

    Sector[,,] sectors;

    GameObjectPool starPool;
    GameObjectPool planetPool;

    #endregion

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

    void CreateGeneratorIfNeeded()
    {
        if (simplex == null)
            simplex = new SimplexNoiseGenerator("42");
    }

    int GetSectorRange()
    {
        return sectorRadius * 2 + 1;
    }

    public void CreateAllSectors()
    {
        sectorRange = GetSectorRange();

        sectors = new Sector[sectorRange, sectorRange, sectorRange];

        Vector3Int radiusVector = new Vector3Int(sectorRadius, sectorRadius, sectorRadius);

        for (int x = 0; x < sectorRange; x++)
        {
            for (int y = 0; y < sectorRange; y++)
            {
                for (int z = 0; z < sectorRange; z++)
                {

                    sectors[x, y, z] = CreateNewSector(currentSector - radiusVector + new Vector3Int(x, y, z));
                    //sectors[x, y, z] = CreateNewSector(curSectorX - sectorRadius + x, curSectorY - sectorRadius + y, curSectorZ - sectorRadius + z);

                }
            }
        }
    }

    void Move(int byX, int byY, int byZ)
    {
        Move(new Vector3Int(byX, byY, byZ));
    }

    void Move(Vector3Int by)
    {
        SmartMovePhysical(by);

        /*
        ClearAllSectors();
        CreateAllSectors();

        GeneratePhysical();
        */
    }

    bool CoordinateIsInsideBounds(Vector3Int coord)
    {
        BoundsInt bounds = new BoundsInt(
            -sectorRadius, -sectorRadius, -sectorRadius,
            sectorRange, sectorRange, sectorRange);

        return bounds.Contains(coord - currentSector);
    }

    void SmartMovePhysical(Vector3Int by)
    {
        // PASS 1 - Move stars and release out of bounds stars
        foreach (var sector in sectors)
        {
            if (CoordinateIsInsideBounds(sector.coordinate - by))
            {
                // Transfer this star into the next sector..
                Sector nextSector = GetSectorFromWorldCoord(sector.coordinate - by);
                nextSector.newStar = sector.star;

                // ..and move it if it has any star
                ShiftSystem(nextSector.newStar, -by);
            }
            else
            {
                // if the sector is out of bounds release the star back into the pool
                ReleaseStarSystem(sector.star);
            }
        }

        // change current sector
        currentSector += by;

        // PASS 2 - Construct new sectors
        foreach (var sector in sectors)
        {
            sector.star = null;

            sector.coordinate += by;

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

    void ShiftSystem(StarEntity[] stars, Vector3Int by)
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

    Sector GetSectorFromWorldCoord(Vector3Int coord)
    {
        Vector3Int localCoord = coord + Vector3Int.one * sectorRadius - currentSector;

        return sectors[localCoord.x, localCoord.y, localCoord.z];
    }

    void ClearAllPhysical()
    {
        foreach (var sector in sectors)
        {
            if (sector.star != null && sector.star.Length > 0)
                ReleaseStarSystem(sector.star);
        }
    }

    IEnumerator FrameAfter()
    {

        float tTest = Time.realtimeSinceStartup; // TEST TIMER

        yield return null;

        Debug.Log("Next frame: " + (Time.realtimeSinceStartup - tTest)); // TEST TIMER

    }

    public void GeneratePhysical()
    {
        if (!generate)
            return;

        if (!starPrefab || !planetPrefab)
        {
            Debug.LogError("Star or Planet prefabs are missing");
            return;
        }

        int sectorCount = sectorRange * sectorRange * sectorRange;
        starPool = new GameObjectPool(starPrefab, sectorCount);
        planetPool = new GameObjectPool(planetPrefab, sectorCount * maxPlanets);

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

            GameObject starGO = starPool.GetGO();
            starGO.transform.position = sectorStartPos + sector.starPostion;

            if (Motion.e) Motion.e.chunks.Add(starGO.transform);

            // TODO: material color

            // if not a nebula generate only one star
            if (sector.star == null)
                sector.star = new StarEntity[1];

            sector.star[0] = starGO.GetComponent<StarEntity>();

            StarEntity star = sector.star[0];

            if (sector.planets.Count > 0)
            {
                if (star.planets == null)
                    star.planets = new List<PlanetEntity>(maxPlanets);

                star.planets.Clear();

                for (int i = 0; i < sector.planets.Count; i++)
                {
                    GameObject planetGO = planetPool.GetGO();
                    planetGO.transform.position = sectorStartPos + sector.planets[i].position;

                    if (Motion.e)
                        Motion.e.chunks.Add(planetGO.transform);

                    PlanetEntity planet = planetGO.GetComponent<PlanetEntity>();

                    Debug.Assert(planet, "PlanetEntity not found on planetPrefab");

                    planet.radius = sector.planets[i].radius;

                    if (planet.radius < gasGiantThreshold)
                    {
                        planet.type = PlanetEntity.Type.GasGiant;

                        if (generateRings)
                            if (Random.value < 0.5f)
                                planet.GetComponent<RingMaker>().enabled = true;
                    }

                    Debug.Assert(star.planets != null, "Planets are null");

                    star.planets.Add(planet);
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

    Sector CreateNewSector(Vector3Int coord)
    {
        CreateGeneratorIfNeeded();

        Sector s = new Sector(maxPlanets);
        s.coordinate = coord;

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

        if (skipZero && s.coordinate == Vector3Int.zero)
            return;

        if (value < systemProbability) // has no solar system
        {
            s.hasSystem = false;
            return;
        }
        else s.hasSystem = true;

        s.name = SReader.GetRandom(namePrefixes) + SReader.GetRandom(nameSuffixes);
        s.starPostion = new Vector3(Random.value, Random.value, Random.value) * sectorSeparation;

        s.starSize = (value - 0.5f) * 2;

        s.starColor = starColorGradient.Evaluate(s.starSize);

        s.orbitNormal = Random.insideUnitSphere.normalized;

        int planetsNum = Random.Range(0, maxPlanets);

        float orbitRadius = minPlanetRange;

        for (int i = 0; i < planetsNum; i++)
        {
            orbitRadius += Random.Range(minPlanetSeparation, minPlanetSeparation * (Mathf.Pow(i + 1, nextPlanetPower)));

            Vector3 pos = RandomPointOnPlane(s.orbitNormal, orbitRadius);

            s.planets.Add(new Sector.Planet()
            {
                orbitRadius = orbitRadius,
                position = s.starPostion + pos,
                radius = Random.Range(minPlanetRadius, maxPlanetRadius), // TODO: Should be relative to type? Gas giants should be bigger
                color = planetColorGradient.Evaluate(Random.value)
            });
        }

        Random.state = randState;
    }

    void ReleaseStarSystem(StarEntity[] stars)
    {
        if (stars == null || stars.Length == 0) return;

        for (int i = 0; i < stars.Length; i++)
        {
            var star = stars[i];

            foreach (var planet in star.planets)
            {
                planetPool.Release(planet.gameObject);
            }

            starPool.Release(star.gameObject);

            stars[i] = null;
        }
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
        Vector3 relativeSector = new Vector3(x - currentSector.x, y - currentSector.y, z - currentSector.z);
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

    void OnValidate()
    {
        CreateAllSectors();

        if (updateNames == true)
        {
            UpdateNames();
            updateNames = false;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!preview)
            return;

        if (sectors == null)
            return;

        sectorRange = GetSectorRange();

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

                    if (previewPlanets && sector.planets.Count > 0)
                    {
                        for (int i = 0; i < sector.planets.Count; i++)
                        {
                            var p = sector.planets[i];

                            Gizmos.color = p.color;
                            Gizmos.DrawSphere(sectorStartPos + p.position,
                                p.radius * planetPreviewRadius);

                            if (drawOrbits)
                            {
                                UnityEditor.Handles.DrawWireDisc(
                                    sectorStartPos + sector.starPostion,
                                    sector.orbitNormal,
                                    p.orbitRadius);
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
