using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{

    public Transform orientation;
    public Transform player;
    public Transform cameraObj;
    public float rotationSpeed = 1f;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;

    public Vector3 currentCheckpoint;

    public List<Vector3> previousPositions = new List<Vector3>();
    public GameObject pastPlayer;
    public GameObject pastPlayerTarget;
    bool recordMovement = true;
    GameObject pastPlayerInstance;
    GameObject pastPlayerTargetInstance;

    Rigidbody rb;
    Animator anim;
    public float jumpHeightApex = 2f;
    public float jumpDuration = 1f;
    public float downwardsGravityMultiplier = 1f;

    public float speed = 1.0f;
    public float maxSpeed = 5.0f;
    public float groundDrag;

    public bool isJumping = false;
    public bool isGrounded;
    public bool allowDoubleJump = false;

    bool doubleJumped = false;

    float gravity;
    float initialJumpVelocity;
    float jumpStartTime;
    Vector3 targetVelocity;
    public float groundDistance = 1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        gravity = -2 * jumpHeightApex / (jumpDuration * jumpDuration);
        initialJumpVelocity = Mathf.Abs(gravity) * jumpDuration;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentCheckpoint = new Vector3(0, 0.25f, -0.118f);
    }

    private void Update()
    {
        // Rotate orientation
        Vector3 viewDirection = transform.position - new Vector3(cameraObj.position.x, transform.position.y, cameraObj.position.z);
        orientation.forward = viewDirection.normalized;

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
         
        if(moveDirection != Vector3.zero)
        {
            transform.forward = Vector3.Slerp(transform.forward, moveDirection.normalized, Time.deltaTime * rotationSpeed);
            anim.SetBool("isMoving", true);
        }
        else
        {
            anim.SetBool("isMoving", false);
        }

        // Move player
        MovePlayer();
        ControlSpeed();

        // Ground check
        isGrounded = Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), Vector3.down, groundDistance + 0.1f);
        Debug.DrawRay(transform.position + new Vector3(0, 1, 0), Vector3.down * (groundDistance + 0.1f), Color.red);

        anim.SetFloat("PosX", rb.velocity.x);
        anim.SetFloat("PosY", rb.velocity.z);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isGrounded && allowDoubleJump && !doubleJumped)
            {
                doubleJumped = true;
                anim.SetBool("isJumping", true);
                StartJump();
            }
            else if (isGrounded)
            {
                doubleJumped = false;
                anim.SetBool("isJumping", true);
                StartJump();
            }
        }

        if (isGrounded && !isJumping)
        {
            anim.SetBool("isJumping", false);
        }

        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }

        // Respawn player if y is too low or if R is pressed
        if (transform.position.y < -5 || Input.GetKeyDown(KeyCode.R))
        {
            ResetLevel();
        }

        // Save previous positions if recordMovement is true
        if (recordMovement)
        {
            previousPositions.Add(transform.position);
        }

        // Repeat past movement
        if (Input.GetKeyDown(KeyCode.E) && anim.GetBool("isJumping") == false && anim.GetBool("isMoving") == false && recordMovement)
        {
            StartCoroutine(RepeatPastMovement());
        }

    }

    void MovePlayer()
    {
        // Calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        rb.AddForce(moveDirection * speed * 10f, ForceMode.Force);
    }

    void ControlSpeed()
    {
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        // Limit velocity if needed
        if (flatVelocity.magnitude > maxSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * maxSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }
    }

    void ResetLevel()
    {
        rb.velocity = Vector3.zero;
        transform.position = currentCheckpoint;
        StopAllCoroutines();

        if (pastPlayerInstance != null) Destroy(pastPlayerInstance);
        if (pastPlayerTargetInstance != null) Destroy(pastPlayerTargetInstance);

        previousPositions.Clear();
        recordMovement = true;
    }

    IEnumerator RepeatPastMovement()
    {
        anim.SetTrigger("repeatHistory");
        recordMovement = false;

        yield return new WaitForSeconds(1.1f);

        // Create past player
        pastPlayerInstance = Instantiate(pastPlayer, currentCheckpoint, transform.rotation);

        // Create past player target
        pastPlayerTargetInstance = Instantiate(pastPlayerTarget, transform.position, transform.rotation);

        // Remove duplicate positions from the beginning of the list
        for (int i = 0; i < previousPositions.Count; i++)
        {
            if (previousPositions[i] == currentCheckpoint)
            {
                previousPositions.RemoveAt(i);
            }
            else
            {
                break;
            }
        }

        // Repeat past movement
        for (int i = 0; i < previousPositions.Count; i++)
        {
            pastPlayerInstance.transform.position = previousPositions[i];

            yield return new WaitForSeconds(0.01f);
        }

        // Destroy past player
        Destroy(pastPlayerInstance);

        // Destroy past player target
        Destroy(pastPlayerTargetInstance);

        previousPositions.Clear();
        recordMovement = true;
    }

    void FixedUpdate()
    {

        // Jumping

        if (isJumping)
        {
            rb.AddForce(Vector3.up * gravity, ForceMode.Acceleration);

            if (Time.time - jumpStartTime >= jumpDuration)
            {
                isJumping = false;
            }
        }
        else
        {
            rb.AddForce(Vector3.down * -gravity * downwardsGravityMultiplier, ForceMode.Acceleration);
        }
    }

    void StartJump()
    {
        // Recalculate gravity and initial velocity in case they were changed in the inspector
        gravity = -2 * jumpHeightApex / (jumpDuration * jumpDuration);
        initialJumpVelocity = Mathf.Abs(gravity) * jumpDuration;

        isJumping = true;
        jumpStartTime = Time.time;
        rb.velocity = Vector3.up * initialJumpVelocity;
    }
}