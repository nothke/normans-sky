using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpacecraftUI : MonoBehaviour {

    public Text speedText;
    public Slider altitudeSlider;

    Motion spacecraft;

    public float rectTestFloat;

    public float altitudeMaxValue = 1;

	void Start () {
        spacecraft = Motion.e;
	}
	
	void Update () {
        speedText.text = Mathf.FloorToInt(spacecraft.velocity).ToString();
        altitudeSlider.maxValue = altitudeMaxValue;
        altitudeSlider.value = spacecraft.altitude;
	}
}
