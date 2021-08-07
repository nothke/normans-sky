using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player e;
    void Awake() { e = this; }

    public Interaction interaction;
    public PlayerController controller;
    public Camera cam;
    public Transform walkerCameraPivot;
    public ShipController ship;

    Seat seat;

    private void Update()
    {
        if (!seat)
            interaction.UpdateInput(Input.GetKeyDown(KeyCode.F));
        else
        {
            if (Input.GetKeyDown(KeyCode.F))
                Disembark();
        }
    }

    public void Board(Seat seat)
    {
        controller.gameObject.SetActive(false);
        seat.Board(cam.transform);

        this.seat = seat;
    }

    public void Disembark()
    {
        seat.Disembark();

        controller.gameObject.SetActive(true);

        cam.transform.parent = walkerCameraPivot;
        cam.transform.localPosition = Vector3.zero;
        cam.transform.localRotation = Quaternion.identity;

        controller.gameObject.transform.position = seat.transform.position + seat.transform.up;

        seat = null;
    }
}
