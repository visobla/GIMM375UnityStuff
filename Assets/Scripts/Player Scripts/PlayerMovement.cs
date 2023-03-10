using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float movementSpeed;

    public float groundDrag;

    [Header("Jump")]
    public float jumpPower;
    public float jumpCooldown;
    public float airMultiplier;
    public int maxJumpCount = 2;
    public int jumpsRemaining = 0;
    public float rayDown = 0.02f;


    [Header("Key Binds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Checking")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    public TMP_Text _groundInfo;
    Vector3 moveDirection;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }


    private void Update()
    {
        //Sends a ray down to check if the player is grounded or not
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + rayDown, whatIsGround);


        PlayerInput();
        MaxSpeed();

        if (grounded)
        {
            rb.drag = groundDrag;
            
            jumpsRemaining = maxJumpCount;
            _groundInfo.text = "Grounded";
        }
        else
        {
            rb.drag = 0;
            _groundInfo.text = "Not Grounded";
        }
        
    }

    private void Delay()
    {

    }

    private void FixedUpdate()
    {
        
        MovePlayer();
    }

    //Checks player pressing controlls
    private void PlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(jumpKey) && (jumpsRemaining > 0))
        {
            grounded = false;
            jumpsRemaining -= 1;
            Jump();
        }


    }

    //Moves the player based on player input
    private void MovePlayer()
    {
        //Finds the movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if(grounded)
            rb.AddForce(moveDirection.normalized * movementSpeed * 10f, ForceMode.Force);

        // in air
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * movementSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void MaxSpeed()
    {
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limits the velocity if it is over move speed cap
        if(flatVelocity.magnitude > movementSpeed)
        {
            Vector3 limitVelocity = flatVelocity.normalized * movementSpeed;
            rb.velocity = new Vector3(limitVelocity.x, rb.velocity.y, limitVelocity.z);
        }
    }

    

    private void Jump()
    {
        // reset the y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpPower, ForceMode.Impulse);
    }

}
