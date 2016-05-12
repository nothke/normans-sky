using UnityEngine;
using System.Collections;

public class TerrainBlitter : MonoBehaviour
{

    public RenderTexture rt;
    //public RenderTexture destRT;
    public Material blitMat;

    public Texture2D startTexture;

    public Color colorToBlit;

    public Shader startShader;

    public Shader perlin;
    public Shader inverter;
    //public Shader gain;
    public Shader setColor;
    public Shader addColor;
    //public Shader multColor;
    public Shader step;

    void Start()
    {
        SetTexture(startTexture);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            BlitColor(colorToBlit);

        if (Input.GetKeyDown(KeyCode.I))
            BlitInverter();

        if (Input.GetKeyDown(KeyCode.A))
            BlitRandomColor();

        if (Input.GetKeyDown(KeyCode.RightShift))
            TestSequence();
    }

    void TestSequence()
    {
        StartBlit();

        SetTexture(startTexture);

        BlitPerlin(25);
        BlitPerlin(50);
        BlitStep(4);
        BlitPerlin(100);

        BlitStep(8);

        BlitPerlin(200);
        BlitPerlin(300);
        BlitPerlin(400);

        

        BlitRandomColor();
        BlitInverter();
        BlitRandomColor();

        EndBlit();
    }

    void BlitRandomColor()
    {
        BlitAddColor(Random.ColorHSV());

    }


    // BLIT OPERATIONS



    void SetTexture(Texture2D texture)
    {
        blitMat.shader = startShader;
        blitMat.mainTexture = texture;

        Graphics.Blit(texture, rt);




    }

    void BlitPerlin(float frequency)
    {
        blitMat.shader = perlin;
        blitMat.SetFloat("_NoiseFrequency", frequency);
        blitMat.SetVector("_Offset", Random.insideUnitSphere * 1000);
        Blit();
    }

    void BlitColor(Color color)
    {
        blitMat.shader = setColor;
        blitMat.color = color;
        Blit();
    }

    void BlitAddColor(Color color)
    {
        blitMat.shader = addColor;
        blitMat.mainTexture = rt;
        blitMat.color = color;
        Blit();
    }

    /*
    void BlitMultColor(Color color)
    {
        blitMat.shader = multColor;
        blitMat.color = color;
        Blit();
    }*/

    void BlitInverter()
    {
        blitMat.shader = inverter;
        Blit();
    }

    /*
    void BlitGain(float amount)
    {
        blitMat.shader = gain;
        blitMat.mainTexture = rt;
        blitMat.SetFloat("_Gain", amount);

        Blit();
    }*/

    void BlitStep(int steps)
    {
        blitMat.shader = step;
        blitMat.SetFloat("_Step", steps);

        Blit();
    }

    RenderTexture rtTemp;

    float tTest;

    void Blit()
    {
        if (rtTemp)
            BlitTemp();
        else
        {
            Debug.Log("rtTemp was null");
            StartBlit();
            BlitTemp();
            EndBlit();
        }

        /*
        rtTemp = RenderTexture.GetTemporary(rt.width, rt.height);

        Graphics.Blit(rt, rtTemp, testMaterial);

        Graphics.Blit(rtTemp, rt);

        RenderTexture.ReleaseTemporary(rtTemp);*/
    }

    void BlitTemp()
    {
        Graphics.Blit(rt, rtTemp, blitMat);
        Graphics.Blit(rtTemp, rt);
    }

    void StartBlit()
    {
        tTest = Time.realtimeSinceStartup; // TEST TIMER
        rtTemp = RenderTexture.GetTemporary(rt.width, rt.height);
    }

    void EndBlit()
    {
        //Graphics.Blit(rtTemp, rt);

        RenderTexture.ReleaseTemporary(rtTemp);
        rtTemp = null;

        Debug.Log("Blit ended at: " + (Time.realtimeSinceStartup - tTest)); // TEST TIMER
    }
}
