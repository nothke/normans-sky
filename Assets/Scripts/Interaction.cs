using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    Camera _cam;
    Camera cam { get { if (!_cam) _cam = GetComponent<Camera>(); return _cam; } }

    Seat interactable;

    public void UpdateInput(bool interactDown)
    {
        if (interactDown)
            if (interactable)
                Player.e.Board(interactable);
    }

    private void FixedUpdate()
    {
        Transform camT = cam.transform;

        RaycastHit hit;
        if (Physics.Raycast(camT.position, camT.forward, out hit, Mathf.Infinity))
        {
            {
                interactable = hit.collider.gameObject.GetComponent<Seat>();
            }
        }
    }
}
