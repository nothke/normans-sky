using UnityEngine;
using System.Collections;

public class Universe : MonoBehaviour
{

    public float starRange = 20000;
    public float systemRange = 2000;
    public int numOfStars = 300;
    public int minBodies = 1;
    public int maxBodies = 4;

    public Gradient starColor;

    public int curSectorX;
    public int curSectorY;

    public class StarSystem
    {
        PlanetEntity[] planets;

    }



    public GameObject starPrefab;
    public GameObject bodyPrefab;

    [System.Serializable]
    public class Duplet
    {
        public Color primColor;
        public Color secColor;
    }

    public Duplet[] duplets;

    public bool newMethod;

    SimplexNoiseGenerator simplex;

    void Start()
    {
        if (newMethod)
        {
            simplex = new SimplexNoiseGenerator("42");

            return;
        }

        for (int i = 0; i < numOfStars; i++)
        {
            GameObject gos = Instantiate(starPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            gos.transform.position = Random.insideUnitSphere * starRange;

            Vector3 systemDir = Random.onUnitSphere;

            Motion.AddChunk(gos.transform);

            int numOfBodies = Random.Range(minBodies, maxBodies);

            for (int j = 0; j < numOfBodies; j++)
            {
                GameObject go = Instantiate(bodyPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                Vector3 position = gos.transform.position + Random.insideUnitSphere * systemRange;

                go.transform.position = Vector3.ProjectOnPlane(position, systemDir);

                int randDup = Random.Range(0, duplets.Length);

                go.GetComponent<RandomVertexPaint>().rC1 = duplets[randDup].primColor;
                go.GetComponent<RandomVertexPaint>().rC2 = duplets[randDup].secColor;

                Motion.AddChunk(go.transform);
            }
        }
    }

    public void CreateSector(int x, int y, int z)
    {
        if (simplex == null) simplex = new SimplexNoiseGenerator("42");


        int seed = ("ad" + x + "_" + y + "_" + z).GetHashCode();

        //Random.seed = seed;

        float value = simplex.coherentNoise(x, y, z);



        //Debug.Log("(" + x + ", " + y + ", " + z + "), " + value);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
            CreateSector(Random.Range(0, 10), 4, 4);
    }
}
