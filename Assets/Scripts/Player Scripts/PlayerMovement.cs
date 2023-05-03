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
    public int jumpsUsed = 0;
    public float rayDown = 0.02f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMulitplier = 2f;
    
    [Header("Key Binds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Slopes")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    public float rayDownSlope = 0.03f;

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

    [Header("Animation")]
    [SerializeField] private Animator animator;
    private bool isJumping;
    private bool isGrounded;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource soundSource;
    [SerializeField] private AudioClip jump;
    [SerializeField] private AudioClip run;
    [SerializeField] private AudioClip land;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        audioSource = GetComponent<AudioSource>();
        audioSource.Play(0);
    }

    private void Update()
    {
        //Sends a ray down to check if the player is grounded or not
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + rayDown, whatIsGround);


        PlayerInput();
        MaxSpeed();


        if (grounded)
        {
            animator.SetBool("IsGrounded", true);
            isGrounded = true;
            animator.SetBool("IsJumping", false);
            isJumping = false;
            animator.SetBool("IsFalling", false);
            rb.drag = groundDrag;

            //jumpsUsed = maxJumpCount;
            _groundInfo.text = "Grounded";
        }
        else
        {
            animator.SetBool("IsGrounded", false);
            isGrounded = false;

            if (isJumping)
            {
                animator.SetBool("IsFalling", true);
            }
            //Sends the player falling down at a certain speed and makes player jump higher if holding jump key
            if (rb.velocity.y < 0)
            {
                rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            //makes player jump lower if pressed rather then held
            else if (rb.velocity.y > 0 && !Input.GetKey(jumpKey))
            {
                rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMulitplier - 1) * Time.deltaTime;
            }
            rb.drag = 0;
            _groundInfo.text = "Not Grounded";
        }

        if (horizontalInput != 0 || verticalInput != 0)
        {
            if (grounded && rb.velocity.magnitude > 0.1f)
            {
                audioSource.pitch = (Random.Range(0.95f, 1.05f));
                audioSource.UnPause();
                animator.SetBool("IsMoving", true);
                if (!audioSource.isPlaying)
                {
                    audioSource.PlayOneShot(run);
                }
            }
            else
            {
                audioSource.Pause();
                animator.SetBool("IsMoving", false);
            }
        }
        else
        {
            audioSource.Pause();
            animator.SetBool("IsMoving", false);
        }
    }

    private void FixedUpdate()
    { 
        MovePlayer();
    }

    //bool IsGrounded()
    //{
    //    return Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + rayDown, whatIsGround);
    //}

    //Checks player pressing controlls
    private void PlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(jumpKey) && (jumpsUsed < maxJumpCount))
        {
            soundSource.pitch = (Random.Range(0.65f, 2f));
            soundSource.PlayOneShot(jump);
            animator.SetBool("IsJumping", true);
            isJumping = true;
            //if(!grounded && jumpsUsed >= maxJumpCount) return;
            //Checks if jumps Used is Equal to 0  then starts the Coroutine
            if (jumpsUsed == 0) StartCoroutine(WaitForLanding());
            //grounded = false;
            jumpsUsed++;
            Jump();
        }
    }

    private IEnumerator WaitForLanding()
    {
        //Waits till you are in air
        yield return new WaitUntil(() => !grounded);
        //Waits till you have landed again
        yield return new WaitUntil(() => grounded);
        soundSource.pitch = (Random.Range(0.6f, 1.3f));
        soundSource.PlayOneShot(land);
        jumpsUsed = 0;
    }

    //Moves the player based on player input
    private void MovePlayer()
    {
        
        //Finds the movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on slope
        if (OnSlope())
        {
            grounded = true;
            rb.AddForce(GetSlopeMoveDirection() * movementSpeed * 20f, ForceMode.Force);
            Debug.Log("Im on a slope");
            _groundInfo.text = "Slope";
            //Stops you from bouncing on a slope
            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 60f, ForceMode.Force);
        }

        // on ground
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * movementSpeed * 10f, ForceMode.Force);
        }
        // in air
        else if (!grounded)
        {
            audioSource.Pause();
            rb.AddForce(moveDirection.normalized * movementSpeed * 10f * airMultiplier, ForceMode.Force);
        }
        //rb.useGravity = !OnSlope();
    }

    //Need to fix freefall velocity resetting at 35 speed
    private void MaxSpeed()
    {
        // Limits speed on slopes
        if (OnSlope())
        {
            if (rb.velocity.magnitude > movementSpeed)
                rb.velocity = rb.velocity.normalized * movementSpeed;
        }

        // Limits speed on ground or in the air
        else
        {
            Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limits the velocity if it is over move speed cap
            if (flatVelocity.magnitude > movementSpeed)
            {
                Vector3 limitVelocity = flatVelocity.normalized * movementSpeed;
                rb.velocity = new Vector3(limitVelocity.x, rb.velocity.y, limitVelocity.z);
            }
        }
        
    }

    private void Jump()
    {
        // reset the y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpPower, ForceMode.Impulse);
    }

    private bool OnSlope()
    {
        //sends Ray down to check slopes
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + rayDownSlope))
        {
            //Calculates angle of slope
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

}
