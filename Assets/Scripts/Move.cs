using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour
{

    public float speed = 60;

    void Update()
    {
        Vector3 inputVector = Vector3.ClampMagnitude((Vector3.right * Input.GetAxis("Horizontal") + Vector3.up * Input.GetAxis("Vertical")), 1);
        transform.Translate(inputVector * Time.deltaTime * speed);
    }
}
