using UnityEngine;
using System.Collections;

public class RenderDepthTexture : MonoBehaviour
{

    public DepthTextureMode depthMode = DepthTextureMode.Depth;

    void Start()
    {
        GetComponent<Camera>().depthTextureMode = depthMode;
    }

}
