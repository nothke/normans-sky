using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using UnityEngine.InputSystem;

public class TogglePixelate : MonoBehaviour
{
    public GameObject letterBoxL;
    public GameObject letterBoxR;
    public PixelBoy pixelBoy;

    void Update()
    {
        if (Keyboard.current.ctrlKey.isPressed && 
            Keyboard.current.altKey.isPressed && 
            Keyboard.current.lKey.wasPressedThisFrame)
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
