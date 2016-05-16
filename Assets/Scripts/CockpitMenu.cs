using UnityEngine;
using System.Collections;

public class CockpitMenu : MonoBehaviour
{

    public static CockpitMenu e;
    void Awake() { e = this; }

    public class CockpitSegment
    {
        

    }

    public void Update()
    {




    }

    void ReadInput()
    {
        if (Input.GetKeyDown(KeyCode.Keypad8)) // TODO: convert keys to buttons
            Navigate(1, 0);
        if (Input.GetKeyDown(KeyCode.Keypad2))
            Navigate(-1, 0);
        if (Input.GetKeyDown(KeyCode.Keypad4))
            Navigate(0, -1);
        if (Input.GetKeyDown(KeyCode.Keypad6))
            Navigate(0, 1);
    }

    public void Navigate(int up, int right)
    {



    }
}