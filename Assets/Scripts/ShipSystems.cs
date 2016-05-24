using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShipSystems : MonoBehaviour
{

    public static ShipSystems e;
    void Awake() { e = this; }

    public float voltage;
    public float maxAllowedVoltage = 0.9f;
    public float minTresholdVoltage = 0.1f;
    public float voltageSmoothTime = 0.1f;
    public float voltageSmoothSpeed = 1;

    float normalVoltage = 0.5f;
    float load = 0;

    float targetVoltage;
    float voltageVelo;

    public Slider voltageSlider;

    public float fuelFlow;
    public Transform fuelFlowIndicator;

    bool[] enginesOn = new bool[3];

    void Update()
    {
        voltage = Mathf.SmoothDamp(voltage, targetVoltage, ref voltageVelo, voltageSmoothTime, voltageSmoothSpeed);
        voltage = Mathf.Clamp01(voltage);

        if (voltageSlider) voltageSlider.value = voltage;
        if (fuelFlowIndicator) fuelFlowIndicator.Rotate(Vector3.forward, fuelFlow);

        fuelFlow = 0;

        if (fuelOn)
        {
            fuelFlow = 1;
        }

        if (batteryOn)
        {
            targetVoltage = normalVoltage + load;

            if (fuelOn && pumpOn)
            {
                fuelFlow = 2000;
            }

            for (int i = 0; i < enginesOn.Length; i++)
                if (enginesOn[i]) fuelFlow = 3000;
        }
        else
            targetVoltage = 0;


    }

    public void AddVoltageIntPercent(int percent)
    {
        AddVoltage(0.01f * percent);
    }

    public void AddVoltage(float value)
    {
        load += value;
        targetVoltage = Mathf.Clamp01(targetVoltage);
    }

    bool batteryOn;
    public void SetBattery(int i) { batteryOn = i == 0 ? false : true; }

    bool fuelOn;
    public void SetFuel(int i)
    {
        fuelOn = i == 0 ? false : true;

        if (i == 0)
            for (int j = 0; j < enginesOn.Length; j++)
                enginesOn[j] = false;
    }

    bool pumpOn;
    public void SetPump(int i)
    {
        if (i == 0)
        {
            pumpOn = false;
            AddVoltage(0.1f);
        }
        else
        {
            pumpOn = true;
            AddVoltage(-0.1f);
        }
    }

    public void EngineIgnition(int i)
    {
        Debug.Log("Engine " + i + ": Attempting ignition");

        if (fuelFlow > 100)
        {
            enginesOn[i] = true;
            AddLoad(-0.2f, 3);
            Debug.Log("Engine " + i + ": Ignition success");
        }
        else
            Debug.Log("Engine " + i + ": Ignition failed");
    }

    public void AddLoad(float value, float time)
    {
        AddVoltage(value);
        StartCoroutine(LoadCo(value, time));
    }

    IEnumerator LoadCo(float value, float time)
    {
        yield return new WaitForSeconds(time);

        AddVoltage(-value);
    }

    public void SetFuelFlow(int value)
    {
        fuelFlow = 0.01f * value;
    }
}