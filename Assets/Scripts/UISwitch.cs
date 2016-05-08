using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class UISwitch : MonoBehaviour
{

    //public Transform[] switches;
    int currentSwitch = -1;
    public RectTransform[] switches;
    public Texture2D[] LUTs;

    public Camera mainCamera;
    ColorCorrectionLookup ccl;

    void Start()
    {
        ccl = mainCamera.GetComponent<ColorCorrectionLookup>();
    }

    void Update()
    {
        for (int i = 0; i < 7; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString())) SwitchTo(i);
        }
    }

    void SwitchTo(int i)
    {
        if (currentSwitch > -1)
        {
            switches[currentSwitch].GetComponent<Animator>().Play(0, 0, 1); // for setting starting normalized time
            switches[currentSwitch].GetComponent<Animator>().SetFloat("speed", -1);
        }

        if (currentSwitch == i)
        {
            currentSwitch = -1;
            return;
        }

        currentSwitch = i;

        switches[currentSwitch].GetComponent<Animator>().Play(0, 0, 0); // for setting starting normalized time
        switches[currentSwitch].GetComponent<Animator>().SetFloat("speed", 1);

        ccl.Convert(LUTs[i], LUTs[i].name);
    }
}
