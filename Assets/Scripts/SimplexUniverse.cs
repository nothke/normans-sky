using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimplexUniverse : MonoBehaviour
{
    public static SimplexUniverse e;

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
    public float maxPlanetRangeMult = 50;

    [Header("Generator Prefabs")]
    public bool generate = false;
    public GameObject starPrefab;
    public GameObject planetPrefab;


    SimplexNoiseGenerator simplex;

    public bool updateNames;
    public string[] namePrefixes;
    public string[] nameSuffixes;

    void Awake()
    {
        e = this;
    }

    void Start()
    {
        CreateAllSectors();

        GeneratePhysical();
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
        public int x;
        public int y;
        public int z;

        public bool hasSystem;

        public string name;

        public Vector3 orbitNormal;

        public float starSize; // incomplete
        public Vector3 starPostion;
        public Color starColor;

        public float[] planetOrbits;
        public Vector3[] planetPositions;
        public Color[] planetColors;

        public StarEntity[] star;
    }

    void CreateSimplex()
    {
        if (simplex == null) simplex = new SimplexNoiseGenerator("42");
    }

    Sector[,,] sectors;

    void CreateAllSectors()
    {
        sectorRange = sectorRadius * 2 + 1;

        sectors = new Sector[sectorRange, sectorRange, sectorRange];

        for (int x = 0; x < sectorRange; x++)
        {
            for (int y = 0; y < sectorRange; y++)
            {
                for (int z = 0; z < sectorRange; z++)
                {
                    sectors[x, y, z] = CreateSector(curSectorX + x, curSectorY + y, curSectorZ + z);

                }
            }
        }
    }

    void Move(int byX, int byY, int byZ)
    {
        curSectorX += byX;
        curSectorY += byY;
        curSectorZ += byZ;

        ClearAllSectors();
        CreateAllSectors();

        GeneratePhysical();
    }

    void ClearAllPhysical()
    {
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
    }

    void GeneratePhysical()
    {
        if (!generate) return;

        if (!starPrefab || !planetPrefab) return;

        ClearAllPhysical();

        foreach (var sector in sectors)
        {
            if (sector.hasSystem)
            {
                //Gizmos.color = Color.gray;
                //Gizmos.DrawWireCube(GetSectorMidPos(sector.x, sector.y, sector.z), Vector3.one * sectorSeparation);
                //Gizmos.color = sector.starColor * 3;

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

                        sector.star[0].planets[i] = planetGO.GetComponent<PlanetEntity>();
                        if (!sector.star[0].planets[i]) Debug.LogWarning("PlanetEntity not found");
                    }
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

    Sector CreateSector(int x, int y, int z)
    {
        CreateSimplex();

        Sector s = new Sector();
        s.x = x;
        s.y = y;
        s.z = z;



        float value = simplex.coherentNoise(x, y, z, octaves, multiplier, amplitude, lacunarity, persistence);
        value += 0.5f;

        Random.seed = ("ad" + x + "_" + y + "_" + z).GetHashCode();

        if (value < systemProbability) // has no solar system
            return s;

        // has a solar system:

        if (skipZero && s.x == 0 && s.y == 0 && s.z == 0)
            return s;

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

        float orbitRadius = 0;

        for (int i = 0; i < planetsNum; i++)
        {
            orbitRadius += Random.Range(minPlanetRange, minPlanetRange * 2 * (i + 1));
            s.planetOrbits[i] = orbitRadius;

            Vector3 pos = RandomPointOnPlane(s.orbitNormal, orbitRadius);

            s.planetPositions[i] = s.starPostion + pos;
            s.planetColors[i] = planetColorGradient.Evaluate(Random.value);
        }

        return s;
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

    Vector3 GetSectorMidPos(int x, int y, int z)
    {
        Vector3 relativeSector = new Vector3(x - curSectorX, y - curSectorY, z - curSectorZ);
        relativeSector -= Vector3.one * (0.5f * sectorRange - 0.5f);

        return relativeSector * sectorSeparation;
    }

    Vector3 GetSectorStartPos(Sector s)
    {
        return GetSectorStartPos(s.x, s.y, s.z);
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

        return;

        if (sectors == null)
        {
            CreateAllSectors();
        }


        if (curSectorX != prevCurX || curSectorY != prevCurY || curSectorZ != prevCurZ)
        {
            CreateAllSectors();

            prevCurX = curSectorX;
            prevCurY = curSectorY;
            prevCurZ = curSectorZ;
        }
    }

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
                    Gizmos.color = Color.gray;
                    Gizmos.DrawWireCube(GetSectorMidPos(sector.x, sector.y, sector.z), Vector3.one * sectorSeparation);
                }

                if (previewSystems)
                {
                    Gizmos.color = sector.starColor * 3;

                    Vector3 sectorStartPos = GetSectorStartPos(sector);

                    Gizmos.DrawSphere(sectorStartPos + sector.starPostion, starPreviewRadius);

                    UnityEditor.Handles.Label(sectorStartPos + sector.starPostion, sector.name);

                    if (previewPlanets && sector.planetPositions.Length > 0)
                    {
                        for (int i = 0; i < sector.planetPositions.Length; i++)
                        {
                            Gizmos.color = sector.planetColors[i];
                            Gizmos.DrawSphere(sectorStartPos + sector.planetPositions[i], planetPreviewRadius);


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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
            Move(1, 0, 0);
    }
}
