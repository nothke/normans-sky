using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartKick : MonoBehaviour {

    public Vector3 acceleration;

	void Start () {
        GetComponent<Rigidbody>().velocity = acceleration;
	}
}
