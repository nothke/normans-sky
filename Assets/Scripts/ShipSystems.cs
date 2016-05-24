using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

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

    public AudioSource workingAudio;

    //bool[] enginesOn = new bool[3];
    //float[] enginesPower = new float[3];

    public class Engine
    {
        public bool isOn;
        public float power = 0;
        public float targetPower = 0;
        public float powerRefVelo;

        public static float idlePower = 0.1f;
        public static float smoothingTime = 0.1f;
    }

    public Engine[] engines = new Engine[3];

    public GameObject[] rcsLeds;

    public enum RCSMotion { Forward, Back, Left, Right, Up, Down, RollLeft, RollRight, PitchUp, PitchDown, YawLeft, YawRight }

    public void FireRCS(float forward, float vertical, float horizontal, float roll, float pitch, float yaw)
    {
        if (fuelFlow < 2500)
            return;

        List<int> indices = new List<int>();

        if (forward > 0) indices.AddRange(GetRCSIndices(RCSMotion.Forward));
        if (forward < 0) indices.AddRange(GetRCSIndices(RCSMotion.Back));
        if (vertical > 0) indices.AddRange(GetRCSIndices(RCSMotion.Up));
        if (vertical < 0) indices.AddRange(GetRCSIndices(RCSMotion.Down));
        if (horizontal < 0) indices.AddRange(GetRCSIndices(RCSMotion.Left));
        if (horizontal > 0) indices.AddRange(GetRCSIndices(RCSMotion.Right));
        if (roll > 0) indices.AddRange(GetRCSIndices(RCSMotion.RollRight));
        if (roll < 0) indices.AddRange(GetRCSIndices(RCSMotion.RollLeft));
        if (pitch < 0) indices.AddRange(GetRCSIndices(RCSMotion.PitchDown));
        if (pitch > 0) indices.AddRange(GetRCSIndices(RCSMotion.PitchUp));
        if (yaw < 0) indices.AddRange(GetRCSIndices(RCSMotion.YawLeft));
        if (yaw > 0) indices.AddRange(GetRCSIndices(RCSMotion.YawRight));

        // first turn off all leds
        foreach (var led in rcsLeds)
            led.SetActive(false);

        // now turn on those active
        for (int i = 0; i < indices.Count; i++)
            rcsLeds[indices[i]].SetActive(true);
    }

    void SetRCSLedColor(Color col, bool activate)
    {
        foreach (var led in rcsLeds)
        {
            led.SetActive(activate);
            led.GetComponent<Image>().color = col;
        }
    }

    int[] GetRCSIndices(RCSMotion rcsMotion)
    {
        int[] i = null;

        switch (rcsMotion)
        {
            case RCSMotion.Forward:
                i = new int[] { 13 }; break;
            case RCSMotion.Back:
                i = new int[] { 12 }; break;
            case RCSMotion.Left:
                i = new int[] { 4, 10 }; break;
            case RCSMotion.Right:
                i = new int[] { 1, 7 }; break;
            case RCSMotion.Up:
                i = new int[] { 2, 5, 8, 11 }; break;
            case RCSMotion.Down:
                i = new int[] { 0, 3, 6, 9 }; break;
            case RCSMotion.RollLeft:
                i = new int[] { 0, 6, 5, 11 }; break;
            case RCSMotion.RollRight:
                i = new int[] { 2, 8, 3, 9 }; break;
            case RCSMotion.PitchUp:
                i = new int[] { 2, 5, 6, 9 }; break;
            case RCSMotion.PitchDown:
                i = new int[] { 0, 3, 8, 11 }; break;
            case RCSMotion.YawLeft:
                i = new int[] { 4, 7 }; break;
            case RCSMotion.YawRight:
                i = new int[] { 1, 10 }; break;
        }

        return i;
    }

    void Start()
    {
        SetBattery(0);

        for (int i = 0; i < engines.Length; i++)
            engines[i] = new Engine();
    }

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

        bool power = batteryOn && voltage > minTresholdVoltage;

        if (batteryOn) targetVoltage = normalVoltage + load;
        else targetVoltage = 0;

        if (power)
        {
            // BATTERY DEPENDANT STUFF HERE:

            if (fuelOn && pumpOn)
                fuelFlow = 2000;


        }

        foreach (var engine in engines)
        {
            if (!engine.isOn) continue;

            fuelFlow = 3000;

            engine.power = Mathf.SmoothDamp(engine.power, engine.targetPower, ref engine.powerRefVelo, Engine.smoothingTime);
        }
    }

    public void SetMainEngineTargetPower(float value)
    {
        value = Mathf.Lerp(Engine.idlePower, 1, value);

        foreach (var engine in engines)
            if (engine.isOn)
                engine.targetPower = value;
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

    public void SetBattery(int i)
    {
        batteryOn = i == 0 ? false : true;

        if (batteryOn) workingAudio.DOFade(1, 0.2f);
        else
            workingAudio.DOFade(0, 0.2f);
    }

    bool fuelOn;

    public void SetFuel(int i)
    {
        fuelOn = i == 0 ? false : true;

        if (i == 0)
            KillEngines();
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

    public void SetAvionics(int i)
    {
        if (i == 0)
            AddVoltage(0.03f);
        else
            AddVoltage(-0.03f);
    }

    public void EngineIgnition(int i)
    {
        Debug.Log("Engine " + i + ": Attempting ignition");

        if (fuelFlow > 100)
        {
            engines[i].isOn = true;
            SetRCSLedColor(Color.green, false);
            AddLoad(-0.2f, 3);
            Debug.Log("Engine " + i + ": Ignition success");
        }
        else
            Debug.Log("Engine " + i + ": Ignition failed");
    }

    public void KillEngines()
    {
        foreach (var engine in engines)
        {
            engine.isOn = false;
            SetRCSLedColor(Color.red, true);
        }
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