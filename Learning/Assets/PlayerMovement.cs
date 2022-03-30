using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public float maximumSpeed;
    public float rotationSpeed;
    public float jumpSpeed;
    public float jumpGracePeriod;
    public float slopeLimit;
    
    private Animator animator;
    private CharacterController characterController;
    private float ySpeed;
    private float? lastGroundedTime;
    private float? lastJumpPressedTime;

    private Vector3 hitNormal = Vector3.zero;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        
        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        float v = Mathf.Clamp01(movementDirection.magnitude);
        float mSpeed = v / 2;

        if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            mSpeed = v;
        }
        animator.SetFloat("InputMagnitude", mSpeed, 0.0f, Time.deltaTime);
        movementDirection.Normalize();

        ySpeed += Physics.gravity.y * Time.deltaTime;

        if(characterController.isGrounded) 
        {
            lastGroundedTime = Time.time;
        }

        if(Input.GetButtonDown("Jump"))
        {
            lastJumpPressedTime = Time.time;
        }

        if(Time.time - lastGroundedTime <= jumpGracePeriod) 
        {
            ySpeed = -0.5f;
            if (Time.time - lastJumpPressedTime <= jumpGracePeriod) 
            {
                ySpeed = jumpSpeed;
                lastGroundedTime = null;
                lastJumpPressedTime = null;
            }
        }

        Vector3 velocity = movementDirection * mSpeed * maximumSpeed;
        velocity.y = ySpeed;

        characterController.Move(velocity * Time.deltaTime);

        var isGrounded = (Vector3.Angle (Vector3.up, hitNormal) <= slopeLimit);

        if(movementDirection != Vector3.zero) 
        {
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        } 
    }

     void OnControllerColliderHit (ControllerColliderHit hit) {
        hitNormal = hit.normal;
    }
}
