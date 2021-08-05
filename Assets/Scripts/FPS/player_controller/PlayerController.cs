using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{

    public float walkSpeed = 6;
    public float runSpeed = 10;
    public float gravity = 20;
    public float jumpHeight = 2;
    public bool canJump = true;
    private bool isRunning = false;
    private bool isGrounded = false;

    public float groundingRayLength = 0.7f;

    public bool IsRunning
    {
        get { return isRunning; }
    }

    new Rigidbody rigidbody;

    Rigidbody groundRigidbody;
    Vector3 groundHitPoint;

    Vector3 groundRigidbodyVelocity;

    bool wantsToJump;

    void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        rigidbody = GetComponent<Rigidbody>();

        rigidbody.freezeRotation = true;
        rigidbody.useGravity = false;
    }

    void FixedUpdate()
    {
        float inputSpeed = !isRunning ? walkSpeed : runSpeed;

        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Forward"));

        if (isGrounded)
        {
            Vector3 groundVelocity = groundRigidbody ?
                groundRigidbody.GetPointVelocity(groundHitPoint)
                : Vector3.zero;

            // calculate how fast it should be moving
            Vector3 targetVelocity = input * inputSpeed;
            targetVelocity = transform.TransformDirection(targetVelocity);

            // apply a force that attempts to reach our target velocity
            Vector3 velocity = rigidbody.velocity;
            Vector3 velocityChange = (targetVelocity - velocity) + groundVelocity;

            // kill vertical velocity change
            Vector3 tVel = transform.InverseTransformDirection(velocityChange);
            tVel.y = 0;
            velocityChange = transform.TransformDirection(tVel);

            rigidbody.AddForce(velocityChange * 60, ForceMode.Acceleration);

            // jump
            if (wantsToJump && canJump)
            {
                rigidbody.velocity += transform.up * jumpHeight;// new Vector3(velocity.x, Mathf.Sqrt(2 * jumpHeight * gravity), velocity.z);
                isGrounded = false;
                wantsToJump = false;
            }
        }
        else
        {
            Vector3 force = input;

            force = Camera.main.transform.TransformDirection(force);

            rigidbody.AddForce(force, ForceMode.Acceleration);
        }
    }

    void Update()
    {
        // check if the player is touching a surface below them
        CheckGrounded();

        isRunning = isGrounded && Input.GetKey(KeyCode.LeftShift);

        if (Input.GetButtonDown("Jump"))
            wantsToJump = true;
    }

    void CheckGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, groundingRayLength))
        {
            isGrounded = true;

            groundHitPoint = hit.point;
            groundRigidbody = hit.rigidbody;
        }
    }
}
