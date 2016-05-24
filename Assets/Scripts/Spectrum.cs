using UnityEngine;
using System.Collections;

public class Spectrum : MonoBehaviour
{

    public static Spectrum e;
    void Awake() { e = this; }

    public bool regen;

    public Texture2D sourceTexture;
    Texture2D texture;
    public Material material;

    public float perlinFrequency = 1;

    public RenderTexture rt;

    void OnValidate()
    {
        if (regen)
        {
            //regen = false;

            CreateSpectrum();
        }
    }

    void Update()
    {
        GetSampleFromRT();
        CreateSpectrum();
    }

    public Color sampleColor = Color.white;

    Texture2D renderTex;

    void GetSampleFromRT()
    {
        renderTex = new Texture2D(rt.width, rt.height);

        RenderTexture.active = rt;
        renderTex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        renderTex.Apply();

        sampleColor = renderTex.GetPixel(rt.width / 2, rt.height);
    }

    void CreateSpectrum()
    {
        if (!texture)
        {
            texture = new Texture2D(64, 1, TextureFormat.ARGB32, false);
        }

        Color[] pixels = new Color[64];
        Color[] sourcePixels = sourceTexture.GetPixels();

        float randX = Random.Range(0.1f, 10000);
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = sampleColor * sourcePixels[i] * Mathf.PerlinNoise(randX, i * perlinFrequency);
        }

        texture.SetPixels(pixels);
        texture.Apply(true);


        material.mainTexture = texture;
    }
}