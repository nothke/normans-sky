using UnityEngine;

public class Kick : MonoBehaviour
{
    public Vector3 velocity;

    void Start()
    {
        GetComponent<Rigidbody>().velocity = velocity;
    }
}
