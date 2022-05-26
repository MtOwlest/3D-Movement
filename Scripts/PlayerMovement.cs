//made by me cuz im stupid and stupid, this is probably not the best movement but you will cope.
//please kill me i just got off school and i spent 9+ hours on this,
using System;
using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    //The Players rigidbody
    Rigidbody rb;

    //Move up and sideways
    public float MoveX;
    public float MoveY;

    //Mouse up and sideways
    float mouseY;
    float mouseX;

    //Used for the Look() Method
    float XRotation = 0;

    //i think its used?
    float x;
    float y;

    //Transforms for the player, camera, and the orientation.
    public Transform Camera;
    public Transform orientation;
    public Transform player;

    //Sensitivity and the rotation of the Y-Axis
    public float MSens;
    float YRotation = 0;

    //Movement speeds
    public float moveSpeed;
    public float maxSpeed;
    public float maxTrueSpeed;

    //check if guilty or nah, (its 2am help)
    public bool isGrounded;
    public bool isJump;
    public bool isSlide;

    //not used
    public float Multiplier;

    //Power of our jump
    public Vector3 JumpForce;

    //Change our scale for the crouch
    public Vector3 crouchScale, posChange;

    //limit our speed in air.
    public float InAirSpeed;

    //our speed while sliding
    public float slidingSpeed;

    //our increase in speed for sliding also know as a DELTA
    public float slideInc;

    public float AirSlideSpeed;
    
    private void Start()
    {

        crouchScale = new Vector3(0.5f, 0.5f, 0.5f);

        player.localScale += crouchScale;

        rb = this.GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
    }

    private void Update()
    {
        InputSystem();
        Look();
        player.localScale = crouchScale;

    }
    
    void InputSystem()
    {
        //movement
        MoveX = Input.GetAxisRaw("Horizontal");
        MoveY = Input.GetAxisRaw("Vertical"); 
        

        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
    }
    void FixedUpdate()
    {
        Move(MoveX, MoveY);
    }

     
    private void Move(float x, float y)
    { 
        
        rb.AddForce(moveSpeed * 2 * y * orientation.transform.forward);
        rb.AddForce(moveSpeed * 2 * x * orientation.transform.right);

        SluggishMove(x, y);

        if (isGrounded == true && moveSpeed > maxSpeed)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
        }


        if (isGrounded == true && Input.GetKey(KeyCode.Space))
        {
            StartJump();
        }
        else
        {
            EndJump();
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            StartCrouch();
            
        }
        else
        {
            EndCrouch();
        }

    }

    void SluggishMove(float x, float y)
    {

        if (x == 0 && y == 0 && isGrounded == true)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
        else
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z);
        }


    }

    void StartJump()
    {
        rb.AddForce(JumpForce);
    }

    void EndJump()
    {
        if (!isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x / InAirSpeed * Multiplier, rb.velocity.y, rb.velocity.z / InAirSpeed);
        }
        if (isGrounded == false && isSlide == true)
        {
            rb.velocity = new Vector3(rb.velocity.x / AirSlideSpeed * Multiplier, rb.velocity.y, rb.velocity.z / AirSlideSpeed);
        }
        else
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z);
        }
       
    }

    void StartCrouch()
    {
        isSlide = true;
        maxSpeed = slidingSpeed;
        crouchScale = new Vector3(0.5f, 0.3f, 0.5f);
    }

    void EndCrouch()
    {
        isSlide = false;
        maxSpeed = maxTrueSpeed;
        crouchScale = new Vector3(0.5f, 0.5f, 0.5f);
    }
    private void Look()
    {
        mouseY = Input.GetAxis("Mouse Y") * MSens * Time.deltaTime;
        mouseX = Input.GetAxis("Mouse X") * MSens * Time.deltaTime;

        
        
        XRotation -= mouseY;
        YRotation += mouseX;


        XRotation = Mathf.Clamp(XRotation, -90f, 90f);

        Camera.localRotation = Quaternion.Euler(XRotation, YRotation, 0f);
        Camera.Rotate(Vector3.up * mouseX);
        orientation.localRotation = Quaternion.Euler(0, YRotation, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 8)
        {
            isGrounded = true;
        }
        else
        {
            Debug.Log("fail");
        }
        
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;   
    }

}
