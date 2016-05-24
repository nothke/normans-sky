using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class CockpitMenu : MonoBehaviour
{

    public static CockpitMenu e;
    void Awake() { e = this; }

    public float switchTime = 0.5f;
    public RectTransform cockpitCanvas;
    public Transform cameraTransform;
    public float cameraTurnAngle = 10;

    bool useCameraLook = true;

    public Transform engine1Gauge;
    public Transform engine2Gauge;
    public Transform engine3Gauge;

    public Transform cockpitHUD;

    public enum Module
    {
        Cockpit,
        Scope,
        Systems,
        CrewRacks,
        CargoBay,
        Something,
        Navigation,
        NormanTerminal,
    }

    public Module activeModule;

    public void NavigateModule(int by)
    {
        switch (activeModule)
        {
            case Module.Cockpit:
                // y base = 3
                cockpitHUD.DOLocalMoveY(cockpitHUD.localPosition.y + by * 5, 0.1f);
                break;
            case Module.Scope:
                break;
            case Module.Systems:
                SystemsSelector(by);
                break;
            case Module.CrewRacks:
                break;
            case Module.CargoBay:
                break;
            case Module.Something:
                break;
            case Module.Navigation:
                break;
            case Module.NormanTerminal:
                break;
            default:
                break;
        }
    }

    void Enter()
    {
        switch (activeModule)
        {
            case Module.Cockpit:
                break;
            case Module.Scope:
                break;
            case Module.Systems:
                sysSwitches[sysSelAt].Toggle();
                break;
            case Module.CrewRacks:
                break;
            case Module.CargoBay:
                break;
            case Module.Something:
                break;
            case Module.Navigation:
                break;
            case Module.NormanTerminal:
                break;
            default:
                break;
        }
    }


    public class Systems
    {

    }

    public Transform selector;
    int sysSelAt;
    public HUDSwitch[] sysSwitches;

    public void SystemsSelector(int by)
    {
        sysSelAt = Repeat(sysSelAt + by, sysSwitches.Length - 1);
        //Debug.Log(sysSelAt);

        HUDSwitch s = sysSwitches[sysSelAt];
        selector.position = s.transform.position;
    }

    int Repeat(int value, int max)
    {
        if (value < 0) value = max;
        if (value > max) value = 0;

        return value;
    }

    public void Update()
    {
        ReadInput();

        UpdateGauges();
    }

    void UpdateGauges()
    {
        float engine = Motion.e.mainEngineAudio.volume;
        float angle = 180 - engine * 270;

        engine1Gauge.localEulerAngles = Vector3.forward * angle;
        engine2Gauge.localEulerAngles = Vector3.forward * angle;
        engine3Gauge.localEulerAngles = Vector3.forward * angle;
    }

    int curX, curY;

    void ReadInput()
    {
        if (Input.GetKeyDown(KeyCode.Keypad4))
            NavigateToSegment(1);
        if (Input.GetKeyDown(KeyCode.Keypad6))
            NavigateToSegment(-1);

        if (Input.GetKeyDown(KeyCode.Keypad8))
            NavigateModule(1);
        if (Input.GetKeyDown(KeyCode.Keypad2))
            NavigateModule(-1);

        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
            Enter();
    }

    int activeModuleInt;

    public void NavigateToSegment(int right)
    {
        curX += right;
        cockpitCanvas.DOMoveX(curX * 64, switchTime, false).SetEase(Ease.InOutCubic);
        if (useCameraLook) cameraTransform.DOLocalRotate(-Vector3.up * curX * cameraTurnAngle, switchTime).SetEase(Ease.InOutCubic);

        activeModuleInt -= right;
        if (activeModuleInt > 7) activeModuleInt = 0;
        if (activeModuleInt < 0) activeModuleInt = 7;

        activeModule = (Module)activeModuleInt;
    }

    void Start()
    {
        //StartCoroutine(PlayRecord(introRecord));
    }

    public Text normanText;

    string[] introRecord =
    {
        "88888888888JLOINNJUUUTTXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXCVXCVXCBBBBBBBBBBBOXOXOXOXOOXOXOXOXOXXXXXXXXXXXXX",
        "Hello\nI am Norman your onboard assistant\nHere to serve you\n\nEnter to continue",
        "Due to limited memory space and parsing in a harsh environment of space",
        "If this is your first flight I will take through an introduction so please read carefully",
        "I warmly recommend using keypad for navigating the ship\nTry it out and return to continue",
        "Standardly keys 4 and 6 will take you to other modules while 8 and 2 is used for selection",
        "But its not yet time to get comfy and fly right away\nMake sure to read the manual first",
        "The main menu is on the next page\nUse 8 and 2 to select and enter to access"
            //"Got comfy\nThe menu is on the next page\nCheck the manual and you can now fly"
    };

    IEnumerator PlayRecord(string[] record)
    {
        if (record == null) { Debug.LogWarning("Record is null"); yield break; }
        if (record.Length == 0) { Debug.LogWarning("Record is empty"); yield break; }

        for (int i = 0; i < record.Length; i++)
        {
            WriteText(record[i]);

            yield return null;
            while (!Input.GetKeyDown(KeyCode.KeypadEnter) && !Input.GetKeyDown(KeyCode.Return))
                yield return null;
        }

        DisplayMenu();
    }

    IEnumerator MenuLoop()
    {
        yield return null;
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                MoveCursor(1);
                DisplayMenu();
                break;
            }

            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                MoveCursor(-1);
                DisplayMenu();
                break;
            }

            if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
            {
                string menu = menus[cursorPos];

                switch (menu)
                {
                    case "body info": StartCoroutine(BodyInfo()); break;
                    case "play intro": StartCoroutine(PlayRecord(introRecord)); break;
                    case "credits": Credits(); break;
                    default: StartCoroutine(BodyInfo()); break;
                }

                yield break;
            }

            yield return null;
        }
        yield return null;
    }

    int cursorPos;

    void MoveCursor(int by)
    {
        cursorPos += by;

        int maxPos = menus.Length - 1;

        if (cursorPos > maxPos)
            cursorPos = 0;

        if (cursorPos < 0)
            cursorPos = maxPos;
    }

    string displayStr;

    IEnumerator ReturnToMenuLoop()
    {
        yield return null;

        while (!Input.GetKeyDown(KeyCode.KeypadEnter) && !Input.GetKeyDown(KeyCode.Return))
        {
            yield return null;
        }

        DisplayMenu();
    }

    IEnumerator BodyInfo()
    {
        StartCoroutine(ReturnToMenuLoop());

        while (true)
        {
            displayStr = "";

            //GUILayout.Label("real x: " + curRealPosition.x.ToString("F2"), guiStyle);
            //GUILayout.Label("real y: " + curRealPosition.y.ToString("F2"), guiStyle);
            //GUILayout.Label("real z: " + curRealPosition.z.ToString("F2"), guiStyle);

            //    GUILayout.Label("Sector " + su.curSectorX + ", " + su.curSectorY + ", " + su.curSectorZ, guiStyle);

            PlanetEntity currentPlanet = Motion.e.currentPlanet;

            if (currentPlanet)
            {
                displayStr += "Body: " + currentPlanet.name;

                //string type = currentPlanet
                //GUILayout.Label("Type: " + currentPlanet.name, guiStyle);


                displayStr += "\nradius " + currentPlanet.radius;
                displayStr += "\natmosH " + currentPlanet.atmosphereHeight;
                displayStr += "\ngravF  " + currentPlanet.gravity.force.ToString("F2");
                displayStr += "\nairDns " + Motion.e.airDensity.ToString("F2");
            }
            else
                displayStr += "Body: None in range";

            WriteText(displayStr);

            yield return new WaitForSeconds(5);

        }
    }

    void Credits()
    {
        StartCoroutine(ReturnToMenuLoop());

        WriteText("Made by:\nIvan Notaros Nothke\n\nMusic by\nMartin Kvale\n\nShader mage\nLeon Denise");
    }

    string[] menus = {
        "body info",
        "plane",
        "mission",
        "play intro",
        "controls",
        "manual",
        "credits"};

    void DisplayMenu()
    {
        StopAllCoroutines();

        string str = "";

        for (int i = 0; i < menus.Length; i++)
        {
            if (i == cursorPos)
                str += "8";
            else
                str += " ";

            str += menus[i] + "\n";
        }

        WriteText(str);

        StartCoroutine(MenuLoop());
    }

    void WriteText(string inStr)
    {
        normanText.DOKill();
        normanText.text = "";
        normanText.DOText(Sanitize(inStr), 0.3f, true, ScrambleMode.None, " ").SetEase(Ease.Linear);
    }

    string Sanitize(string inStr)
    {
        return inStr.ToUpper();
        //return inStr.ToUpper().Replace(" ", "    ");
    }
}