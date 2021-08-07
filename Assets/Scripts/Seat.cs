using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : MonoBehaviour
{
    public Transform eyePivot;

    public ShipController controller;

    public void Board(Transform t)
    {
        t.parent = eyePivot;
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;

        if (controller)
            controller.enabled = true;
    }

    public void Disembark()
    {
        if (controller)
            controller.enabled = false;
    }
}
