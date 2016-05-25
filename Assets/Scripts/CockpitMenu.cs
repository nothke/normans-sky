using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class CockpitMenu : MonoBehaviour
{

    public static CockpitMenu e;
    void Awake() { e = this; }

    public float switchTime = 0.5f;
    public RectTransform cockpitCanvas;
    public Transform cameraTransform;
    public float cameraTurnAngle = 10;

    bool useCameraLook = true;

    public Transform[] engineGauges;

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
                TerminalMoveCursor(-by);
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
                TerminalEnter();
                break;
            default:
                break;
        }
    }

    public Transform selector;
    int sysSelAt;
    public HUDSwitch[] sysSwitches;

    int curX, curY;

    public void SystemsSelector(int by)
    {
        sysSelAt = Repeat(sysSelAt - by, sysSwitches.Length - 1);
        //Debug.Log(sysSelAt);

        HUDSwitch s = sysSwitches[sysSelAt];
        selector.position = s.transform.position;
    }

    #region Start and Update

    void Start()
    {
        InitTerminalPages();

        DisplayTerminalPage(terminalPages[0]);
    }

    public void Update()
    {
        ReadInput();

        UpdateGauges();
    }

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
        {
            Enter();
        }
    }

    void UpdateGauges()
    {
        for (int i = 0; i < ShipSystems.e.engines.Length; i++)
        {
            float angle = 180 - ShipSystems.e.engines[i].power * 270;

            engineGauges[i].localEulerAngles = Vector3.forward * angle;
        }
    }





    #endregion

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



    //--------
    // NORMAN
    //--------

    public class TerminalPage
    {
        public enum Type { Menu, Record, Update, Terminator };
        public Type type;
        public string name;
        public string next;
        public string[] lines;

        public TerminalPage(string name, Type type, string[] lines, string next)
        {
            this.name = name;
            this.type = type;
            this.lines = lines;
            this.next = next;
        }
    }

    List<TerminalPage> terminalPages = new List<TerminalPage>();

    public void InitTerminalPages()
    {
        terminalPages.Add(new TerminalPage(
            "INTRO",
            TerminalPage.Type.Record,
            new string[]
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
            },
            "MAIN"));

        // MAIN MENU

        terminalPages.Add(new TerminalPage(
            "MAIN",
            TerminalPage.Type.Menu,
            new string[]
            {
                "BODY INFO",
                "MISSION",
                "INTRO",
                "CONTROLS",
                "MANUAL",
                "CHECKLISTS",
                "CREDITS"
            },
            ""));

        // MANUAL

        terminalPages.Add(new TerminalPage(
            "MANUAL",
            TerminalPage.Type.Menu,
            new string[]
            {
                "STARTUP",
                "FLIGHT",
                "SYSTEMS",
                "NAVIGATION",
                "MAIN"
            },
            ""));

        terminalPages.Add(new TerminalPage(
            "STARTUP",
            TerminalPage.Type.Record,
            new string[]
            {
                "Startup Procedure\nThis manual will show you how to perform a start up sequence",
                "The whole start up is handled in the system module which is second on the right of the cockpit",
                "First, we need to turn main batteries on, it is the BATT switch",
                "The voltage indicator on the top right should now show peak voltage, at the top of green bar",
                "If it does not, please refer to the electrics manual",
                "Now we need to start the primary engines. Begin by letting the fuel in",
                "As the fuel valve is open, you should notice a tiny flow on the flow spinner at the low center",
                "But the flow is not enough for the engines to pick up the pressure. Turn the PUMP on",
                "Now the pump is forcing the fuel and you can see the flow ind is spinning fast",
                "Also notice that running the pump has increased load on the battery and lowered the voltage",
                "Now we can begin ignition. On the right of the engine panel are 3 buttons, press one of the 3",
                "As the engine is being started, you can see the voltage surge, it takes a lot of power",
                "Once the engine has started, it is pulling enough fuel by itself that the pump is no longer required",
                "You can turn off the pump and then start the other 2 engines"

            },
            "MANUAL"));

        terminalPages.Add(new TerminalPage(
            "ELECTRICS",
            TerminalPage.Type.Record,
            new string[]
            {
                "REC",
                "Electrics manual\nThis manual will explain the electrics system on the ship",
                "Intro: There are 2 power sources on the ship, primary and backup cell",
                "The primary power source on the ship is the fuel cell aka battery running on unobtainium",
                "In the event of a primary battery failure, a backup cell will activate..",
                "..and power critical elements like this terminal and life support",
                "There are no other generators in this version of the ship",

                "To turn the primary power source, use the switch titled BATT in the systems module",
                "Once it is active the voltage meter on the top right will indicate current voltage",


                "Load:\nEvery device running using the onboard electricity is applying a load",
                "..and the voltage will drop",
                "If the voltage drops to the red zone, the power will automatically turn off..",
                "..to prevent devices from operating at a not designated voltage",
                "..and will resume if the voltage returns to the optimal zone",
                "In the case of overvoltage, the fuse will blow",
                "So, manage the devices properly to keep the voltage at optimium levels..",
                "And keep an eye on the voltage meter",
                "End of electrics manual"
            },
            "MANUAL"));




        // CHECKLISTS

        terminalPages.Add(new TerminalPage(
            "CHECKLISTS",
            TerminalPage.Type.Menu,
            new string[]
            {
                "START UP",
                "SHUT DOWN",
                "APPROACH",
                "MAIN"
            },
            ""));

        terminalPages.Add(new TerminalPage(
            "START UP",
            TerminalPage.Type.Record,
            new string[]
            {
                "Systems:\n" +
                "1.BATT  -on\n" +
                "2.FUEL  -on\n" +
                "3.PUMP  -on\n" +
                "4.ENGIN1-on\n" +
                "5.PUMP  -off\n" +
                "6.ENGIN2-on\n" +
                "7.ENGIN3-on\n"
            },
            "CHECKLISTS"));

        terminalPages.Add(new TerminalPage(
            "SHUT DOWN",
            TerminalPage.Type.Record,
            new string[]
            {
                "Systems:\n" +
                "1.FUEL  -cut\n" +
                "1.BATT  -off\n"
            },
            "CHECKLISTS"));

        // EXCEPTION

        terminalPages.Add(new TerminalPage(
            "EXCEPTION",
            TerminalPage.Type.Record,
            new string[]
            {
                "\n888888888888\n\nUnknown or not implemented feature\n\n888888888888",
            },
            "MAIN"));

        // SPECIAL

        terminalPages.Add(new TerminalPage(
            "BODY INFO",
            TerminalPage.Type.Update,
            null,
            "MAIN"));
    }

    public Text normanText;

    TerminalPage currentTerminalPage;

    public void DisplayTerminalPage(string pageKey)
    {
        DisplayTerminalPage(GetTerminalPage(pageKey));
    }

    public void DisplayTerminalPage(TerminalPage tp)
    {
        if (tp == null)
        {
            DisplayErrorPage(); //TODO
            return;
        }

        currentTerminalPage = tp;

        if (tp.type == TerminalPage.Type.Menu)
        {
            menuCursorPos = 0;
            DisplayMenu(tp.lines);
        }

        if (tp.type == TerminalPage.Type.Record)
            NextRecordPage();
    }

    public TerminalPage GetTerminalPage(string key)
    {
        foreach (var t in terminalPages)
        {
            if (t.name == key)
                return t;
        }

        Debug.Log("Unknown Page");

        return null;
    }

    void DisplayErrorPage()
    {
        if (GetTerminalPage("EXCEPTION") != null)
            DisplayTerminalPage("EXCEPTION");
        else
            Debug.LogError("EXCEPTION page doesn't exist but MUST!");
    }

    [System.Obsolete]
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



    void DisplayMenu(string[] menuLines)
    {
        StopAllCoroutines(); // DIRTY

        string str = "";

        for (int i = 0; i < menuLines.Length; i++)
        {
            if (i == menuCursorPos)
                str += "8";
            else
                str += " ";

            str += menuLines[i] + "\n";
        }

        WriteText(str);

        //StartCoroutine(MenuSelectLoop(menuLines));
    }

    /*
    IEnumerator MenuSelectLoop(string[] menuLines)
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

            if (pressEnter)
            {
                string menu = mainMenuLines[cursorPos];

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

    }*/

    //public enum ScreenType { Menu, Record, Update }

    /*
    void TerminalUpdate()
    {

        string[] nextLines = null;

        // navigate up

        // navigate down

        if (pressEnter)
        {
            ScreenType screenType = ScreenType.Menu;

            switch (screenType)
            {
                case ScreenType.Menu:
                    DisplayMenu(nextLines);
                    break;
                case ScreenType.Record:
                    PlayRecord(manualLines);
                    break;
                case ScreenType.Update:
                    StartCoroutine(BodyInfo());
                    break;
                default:
                    break;
            }
        }
    }*/

    //ScreenType nextScreenType = ScreenType.Record;
    //string[] nextScreenLines;
    //string[] currentScreenLines;

    void TerminalMoveCursor(int by)
    {
        if (currentTerminalPage.type != TerminalPage.Type.Menu)
            return;

        menuCursorPos += by;

        int maxPos = currentTerminalPage.lines.Length - 1;
        menuCursorPos = Repeat(menuCursorPos, maxPos);

        // refresh terminal
        DisplayMenu(currentTerminalPage.lines);
    }

    [System.Obsolete("Use TermnalMoveCursor instead")]
    void MoveCursor(int by)
    {
        menuCursorPos += by;

        int maxPos = 6;

        if (menuCursorPos > maxPos)
            menuCursorPos = 0;

        if (menuCursorPos < 0)
            menuCursorPos = maxPos;
    }

    int atRecordPage;

    void TerminalEnter()
    {
        switch (currentTerminalPage.type)
        {
            case TerminalPage.Type.Menu:
                DisplayTerminalPage(currentTerminalPage.lines[menuCursorPos]);
                menuCursorPos = 0;
                break;
            case TerminalPage.Type.Record:
                NextRecordPage();
                break;
            case TerminalPage.Type.Update:
                StartCoroutine(BodyInfo());
                break;
            case TerminalPage.Type.Terminator:
                DisplayTerminalPage(terminalPages[1]); // DIRTY - pointing to main menu
                break;
            default:
                break;
        }
    }

    void NextRecordPage()
    {
        TerminalPage tp = currentTerminalPage;

        if (atRecordPage >= tp.lines.Length)
        {
            atRecordPage = 0;
            DisplayTerminalPage(GetTerminalPage(tp.next));
            return;
        }

        WriteText(tp.lines[atRecordPage]);
        atRecordPage++;
    }

    [System.Obsolete]
    void DisplayMenu()
    {
        StopAllCoroutines();

        string str = "";

        WriteText(str);

        StartCoroutine(MenuLoop());
    }

    [System.Obsolete]
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


            yield return null;
        }
        yield return null;
    }

    [System.Obsolete]
    IEnumerator ReturnToMainMenu()
    {
        yield return null;

        while (!Input.GetKeyDown(KeyCode.KeypadEnter) && !Input.GetKeyDown(KeyCode.Return))
        {
            yield return null;
        }

        DisplayMenu();
    }

    //bool pressEnter;

    int menuCursorPos;



    string displayStr;



    IEnumerator BodyInfo()
    {
        //StartCoroutine(ReturnToMainMenu());

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

    [System.Obsolete]
    void Credits()
    {
        StartCoroutine(ReturnToMainMenu());

        WriteText("Made by:\nIvan Notaros Nothke\n\nMusic by\nMartin Kvale\n\nShader mage\nLeon Denise");
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

    int Repeat(int value, int max)
    {
        if (value < 0) value = max;
        if (value > max) value = 0;

        return value;
    }
}