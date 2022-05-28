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
    public Transform groundCollider;

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
    public bool isSprint;

    //not used
    public float Multiplier;

    //Power of our jump
    public Vector3 JumpForce;

    public Vector3 SlideJump;

    //Change our scale for the crouch
    public Vector3 crouchScale, posChange;

    //limit our speed in air.
    public float InAirSpeed;

    //our speed while sliding
    public float slidingSpeed;

    //our increase in speed for sliding also know as a DELTA
    public float slideInc;

    //our speed in air while sliding
    public float AirSlideSpeed;

    //our speed which we sprint
    public float sprintSpeed;

    //default air sprint speed
    public float AirSprintSpeed;
    
    //the true max speed to reset it back to
    public float trueMaxSpeed;

    //our sprint speed
    public float maxsprint;


    //first frame where we initialize things
    private void Start()
    {

        //setting our scale to our default
        crouchScale = new Vector3(0.5f, 0.5f, 0.5f);

        //make our local scale = to the crouch scale
        player.localScale += crouchScale;
        
        //making our variable = to our rigidbody
        rb = this.GetComponent<Rigidbody>();

        //setting our cursor mode so we cant see it and its locked in the middle of our screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    //happens every frame
    private void Update()
    {
        //used for gaining inputs
        InputSystem();
        //used for looking around
        Look();
        //always making the scale = to our crouch scale
        player.localScale = crouchScale;

    }

    //putting all our inputs here so they are called every frame and we can refernece them
    void InputSystem()
    {
        //movement
        MoveX = Input.GetAxisRaw("Horizontal");
        MoveY = Input.GetAxisRaw("Vertical");

        //mouse
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
    }

    //physics update used by our rigidbody for smoothness
    void FixedUpdate()
    {
        //our move method, paramaters are our movement so we can reference easier
        Move(MoveX, MoveY);
    }

    //our move method
    private void Move(float x, float y)
    {
        //making our character move
        rb.AddForce(moveSpeed * 5 * y * orientation.transform.forward);
        rb.AddForce(moveSpeed * 5 * x * orientation.transform.right);

        //making our movement snappy, so if your not touching you will stop or if diagnol you wont go as fast
        SluggishMove(x, y);

        //setting the max speed and clamping it
        if (isGrounded == true && moveSpeed > maxSpeed)
        {
            //clamps our velocity to the maxspeed
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
        }

        //our jump method
        if (isGrounded == true && Input.GetKey(KeyCode.Space))
        {
            //our jump method so we start our jump
            StartJump();
        }
        //if were not jumping anymore
        else
        {
            //we end our jump with this method
            EndJump();
        }
        //used for sliding
        if (Input.GetKey(KeyCode.LeftControl))
        {
            //start our slide method
            StartCrouch();

        }
        //if were not sliding use this method and end it
        else
        {
            EndCrouch();
        }

        //sprinting 
        if (isGrounded && Input.GetKey(KeyCode.LeftShift))
        {
            //setting our current speed to our pre-defined sprint speed and showing we are sprinting with isSprint
            moveSpeed = sprintSpeed;
            isSprint = true;
        }
        //if not then movespeed is = to our true speed and were not sprinting
        else
        {
            //make movespeed same as truemaxspeed
            moveSpeed = trueMaxSpeed;
            //sprinting is no longer.
            isSprint = false;
        }
    }

    //making our movement snappy and passing in our movement parameters
    void SluggishMove(float x, float y)
    {
        //if we have no input our velocity will = 0 except for jumping
        if (x == 0 && y == 0 && isGrounded == true)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
        //if not then its just equal to our current velocity
        else
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z);
        }
        //if both are pressed then we can limit diagnol movement speed
        if (x != 0 && y != 0)
        {
            //checks if were sprinting so we can divide as needed
            if (isSprint)
            {
                rb.velocity = new Vector3(rb.velocity.x / 6, rb.velocity.y, rb.velocity.z / 6);
            }
            //default diagnol speed
            else
            {
                rb.velocity = new Vector3(rb.velocity.x / 2, rb.velocity.y, rb.velocity.z / 2);
            }
            
            
        }
       
        
    }


    //our jump method
    void StartJump()
    {
        //if were sliding we have our own jump
        if (isSlide)
        {
            //which is the slidejump also its seperate from normal jump
            rb.AddForce(SlideJump);
        }
        //or else its gonna be a default jump
        else
        {
            //adding our default jump power
            rb.AddForce(JumpForce);
        }
        

    }


    //ending our jump
    void EndJump()
    {
        //if were in air then our velocity is divided by our airspeed and multiplied my a multiplier variable this is our default
        if (!isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x / InAirSpeed * Multiplier, rb.velocity.y, rb.velocity.z / InAirSpeed);
        }
        //if we are sliding then set it to our sliding air speed variable
        if (isGrounded == false && isSlide == true)
        {
            rb.velocity = new Vector3(rb.velocity.x / AirSlideSpeed * Multiplier, rb.velocity.y, rb.velocity.z / AirSlideSpeed);
        }
        //or just keep it to our current velocity
        else
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z);
        }

    }


    //start our crouch method
    void StartCrouch()
    {
        //set sliding to true, set our maxspeed to our sliding speed, and change our crouch scale.
        isSlide = true;
        maxSpeed = slidingSpeed;
        crouchScale = new Vector3(0.5f, 0.3f, 0.5f);

        //not used for rn
        posChange = new Vector3(0, 0.3f, 0);
    }


    //ends our crouch
    void EndCrouch()
    {
        //we are no longer sliding set our speed back to normal and our scale too
        isSlide = false;
        maxSpeed = maxTrueSpeed;
        crouchScale = new Vector3(0.5f, 0.5f, 0.5f);
    }

    //this is a pretty basic look fucntion, and is definatly the thing im least happy with,
    private void Look()
    {
        //sets our variables to our mousex/y axis and multiply by our sens,
        mouseY = Input.GetAxis("Mouse Y") * MSens * Time.deltaTime;
        mouseX = Input.GetAxis("Mouse X") * MSens * Time.deltaTime;


        //set rotation variables to the previous variables
        XRotation -= mouseY;
        YRotation += mouseX;

        //clamp it so we cant rotate super far down or up
        XRotation = Mathf.Clamp(XRotation, -90f, 90f);

        //quaternions make me go die ong. but we set our cameras rotation = to our mousex and y variables, then roate it, then change orientation so we can move in that direction
        Camera.localRotation = Quaternion.Euler(XRotation, YRotation, 0f);
        Camera.Rotate(Vector3.up * mouseX);
        orientation.localRotation = Quaternion.Euler(0, YRotation, 0);
    }


    //if we collide with an object
    private void OnCollisionEnter(Collision collision)
    {
        //and the objects layer is 8 (or our ground layer)
        if (collision.gameObject.layer == 8)
        {
            //we are on the ground
            isGrounded = true;
        }
        //or else were not
        else
        {
            //and we are touching smoething thats not ground
            Debug.Log("fail");
        }

    }


    //when we exit the ground,
    private void OnCollisionExit(Collision collision)
    {
        //we are no longer grounded and set this to false
        isGrounded = false;
    }

}
