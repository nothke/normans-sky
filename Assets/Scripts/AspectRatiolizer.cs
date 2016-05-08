using UnityEngine;
using System.Collections;

public class AspectRatiolizer : MonoBehaviour
{
#if !UNITY_EDITOR
    void Start()
    {
        if (!Screen.fullScreen)
            Screen.SetResolution(Screen.height, Screen.height, false);
    }
#endif
}
