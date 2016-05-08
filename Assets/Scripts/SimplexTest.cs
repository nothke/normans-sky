using UnityEngine;
using System.Collections;

public class SimplexTest : MonoBehaviour
{

    SimplexNoiseGenerator simplex;

    public int offsetX = 0;
    public int offsetY = 0;
    public int offsetZ = 0;

    public int cubeSize = 10;

    public float pointSep = 1;
    public float rayScale = 1;

    void Start()
    {
        simplex = new SimplexNoiseGenerator("42");
    }

    public int octaves = 1;
    public int multiplier = 25;
    public float amplitude = 0.5f;
    public float lacunarity = 2;
    public float persistence = 0.9f;

    void OnDrawGizmosSelected()
    {

        if (simplex == null) simplex = new SimplexNoiseGenerator("42");

        for (int x = 0; x < cubeSize; x++)
        {
            for (int y = 0; y < cubeSize; y++)
            {
                for (int z = 0; z < cubeSize; z++)
                {
                    //float value = simplex.noise(offsetX + x, offsetY + y, offsetZ + z);
                    float value = simplex.coherentNoise(offsetX + x, offsetY + y, offsetZ + z, octaves, multiplier, amplitude, lacunarity, persistence);

                    value += 0.5f;

                    if (value < 0) continue;



                    Gizmos.color = Color.HSVToRGB(value, 1, 1);

                    if (value > 1) Gizmos.color = Color.white;

                    Vector3 pos = new Vector3(x * pointSep, y * pointSep, z * pointSep);
                    Gizmos.DrawSphere(pos, value * rayScale);
                }
            }
        }

    }
    

}
