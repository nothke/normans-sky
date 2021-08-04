using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainControl : MonoBehaviour {

	public static MainControl e;
    void Awake() { e = this; }

    public Text loading;

    public Camera introCamera;
    public Camera shipCamera;

    public GameObject go;

    IEnumerator Start()
    {
        introCamera.enabled = true;
        shipCamera.enabled = false;

        go.SetActive(false);

        loading.text = "";

        yield return null;

        SectorUniverse.e.CreateAllSectors();
        loading.text = "Creating Universe..";

        yield return null;

        SectorUniverse.e.GeneratePhysical();
        loading.text = "Creating universe..";

        yield return null;

        loading.text = "Initializing Systems..";

        yield return null;

        loading.text = "";

        //yield return new WaitForSeconds(10);

        yield return null;

        go.SetActive(true);

        introCamera.enabled = false;
        shipCamera.enabled = true;
    }
}