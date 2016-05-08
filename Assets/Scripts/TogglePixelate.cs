using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TogglePixelate : MonoBehaviour
{
    public GameObject letterBoxL;
    public GameObject letterBoxR;
    public PixelBoy pixelBoy;

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.L))
            Toggle();
    }

    public void Toggle()
    {
        bool on = !pixelBoy.enabled;

        letterBoxL.SetActive(on);
        letterBoxR.SetActive(on);
        pixelBoy.enabled = on;
    }
}
